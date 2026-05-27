using Microsoft.AspNetCore.Routing;

namespace GosExamTemplate.Dtos.Common;

public class PaginationViewModel
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = Pagination.DefaultPageSize;
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public string Controller { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public IDictionary<string, object?> PreserveRouteValues { get; set; } = new Dictionary<string, object?>();

    public int ShownFrom => TotalCount == 0 ? 0 : (Page - 1) * PageSize + 1;
    public int ShownTo => TotalCount == 0 ? 0 : Math.Min(Page * PageSize, TotalCount);

    public RouteValueDictionary GetRouteForPage(int targetPage)
    {
        var route = new RouteValueDictionary();
        foreach (var (key, value) in PreserveRouteValues)
        {
            if (value is null) continue;
            if (value is string s && string.IsNullOrWhiteSpace(s)) continue;
            route[key] = value;
        }

        route["page"] = targetPage;
        route["pageSize"] = PageSize;
        return route;
    }
}
