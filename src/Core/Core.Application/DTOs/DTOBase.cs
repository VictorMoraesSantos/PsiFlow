using System.Text.Json.Serialization;

namespace Core.Application.DTOs
{
    public record DTOBase<TId>(
        [property: JsonPropertyOrder(-3)] TId Id,
        [property: JsonPropertyOrder(-2)] DateTime CreatedAt,
        [property: JsonPropertyOrder(-1)] DateTime? UpdatedAt)
    {
        protected DTOBase() : this(default!, default, default) { }
    }
}
