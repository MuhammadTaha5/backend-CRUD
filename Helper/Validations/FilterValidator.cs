using System.Globalization;
using System.Reflection;
using StudentManagement.DTOs;
using StudentManagement.Helper.Enums;
namespace StudentManagement.Helper.Validations
{

    public static class FilterValidator
    {
        private static readonly FilterOperator[] StringOperators =
            { FilterOperator.Eq, FilterOperator.Neq, FilterOperator.Contains };

        private static readonly FilterOperator[] ComparableOperators =
            { FilterOperator.Eq, FilterOperator.Neq, FilterOperator.Gt,
              FilterOperator.Gte, FilterOperator.Lt, FilterOperator.Lte };

        private static readonly FilterOperator[] DefaultOperators =
            { FilterOperator.Eq, FilterOperator.Neq };

        public static FilterValidationResult Validate<T>(FilterCriteria filter, string[] filterableProperties)
        {
            // Step 1: operator must be a defined enum value
            if (!Enum.IsDefined(typeof(FilterOperator), filter.Operator))
                return Fail($"Operator '{filter.Operator}' is not a recognized filter operator.");

            // Step 2: property must be in the allow-list
            string? matchedName = filterableProperties.FirstOrDefault(p =>
                string.Equals(p, filter.PropertyName, StringComparison.OrdinalIgnoreCase));

            if (matchedName == null)
                return Fail($"Property '{filter.PropertyName}' is not filterable.");

            // Step 3: property must actually exist on the model
            PropertyInfo? prop = typeof(T).GetProperty(matchedName,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            if (prop == null)
                return Fail($"Property '{filter.PropertyName}' does not exist on {typeof(T).Name}.");

            Type? propType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
            bool isString = propType == typeof(string);
            bool isComparable = !isString && typeof(IComparable).IsAssignableFrom(propType);

            // Step 4: operator must be compatible with the property's type
            FilterOperator[] allowedOps = isString ? StringOperators
                                         : isComparable ? ComparableOperators
                                         : DefaultOperators;

            if (Array.IndexOf(allowedOps, filter.Operator) < 0)
                return Fail($"Operator '{filter.Operator}' is not valid for property '{filter.PropertyName}' ({propType.Name}).");

            // Step 5: value must convert to the property's type
            if (isString)
                return Ok(prop, filter.Value);

            if (!TryConvert(filter.Value, propType, out var converted))
                return Fail($"Value '{filter.Value}' is not valid for property '{filter.PropertyName}' ({propType.Name}).");

            return Ok(prop, converted!);
        }

        private static bool TryConvert(string value, Type targetType, out object? result)
        {
            try
            {
                if (targetType.IsEnum)
                {
                    result = Enum.Parse(targetType, value, ignoreCase: true);
                    return true;
                }

                result = Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }

        private static FilterValidationResult Fail(string error) =>
            new() { IsValid = false, Error = error };

        private static FilterValidationResult Ok(PropertyInfo prop, object value) =>
            new() { IsValid = true, Property = prop, ConvertedValue = value };
    }
}
