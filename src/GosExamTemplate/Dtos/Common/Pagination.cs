namespace GosExamTemplate.Dtos.Common;

public static class Pagination
{
    public const int DefaultPageSize = 6;
    public const int MaxPageSize = 50;

    public static (int Page, int PageSize) Normalize(int? page, int? pageSize)
    {
        var normalizedPage = page is > 0 ? page.Value : 1;
        var normalizedSize = pageSize is > 0 and <= MaxPageSize
            ? pageSize.Value
            : DefaultPageSize;
        return (normalizedPage, normalizedSize);
    }

    public static int GetTotalPages(int totalCount, int pageSize)
        => pageSize > 0 ? (int)Math.Ceiling(totalCount / (double)pageSize) : 0;

    public static int ClampPage(int page, int totalPages)
        => totalPages > 0 ? Math.Clamp(page, 1, totalPages) : 1;
}
