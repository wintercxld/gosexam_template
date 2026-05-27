namespace GosExamTemplate.Dtos.Common;

public class PagedIndexDto<T>
{
    public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = Pagination.DefaultPageSize;
    public int TotalCount { get; set; }
    public int TotalPages => Pagination.GetTotalPages(TotalCount, PageSize);

    public PaginationViewModel ToPagination(string controller, string action)
        => new()
        {
            Page = Page,
            PageSize = PageSize,
            TotalCount = TotalCount,
            TotalPages = TotalPages,
            Controller = controller,
            Action = action,
        };
}
