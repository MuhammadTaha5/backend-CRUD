namespace StudentManagement.Domain.Repositories
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> Paginate<T>(
            this IQueryable<T> query,
            int pageNumber,
            int pageSize)
        {
            return query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
        }
    }
}