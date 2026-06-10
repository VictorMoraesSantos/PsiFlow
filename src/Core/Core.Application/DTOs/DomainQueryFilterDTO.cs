namespace Core.Application.DTOs
{
    public record DomainQueryFilterDto(int? Page = 1, int? PageSize = 20)
    {
        public DateOnly? CreatedAt { get; init; }
        public DateOnly? UpdatedAt { get; init; }
        public bool? IsDeleted { get; init; }
        public string? SortBy { get; init; }
        public bool? SortDesc { get; init; }
    }
}
