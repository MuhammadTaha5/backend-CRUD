
using System.Text.Json.Serialization;

namespace MyFirstAPI.Models

{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SectionEnum
    {
        A,
        B,
        C
    }
}