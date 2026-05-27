using GosExamTemplate.Dtos.Common;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GosExamTemplate.Dtos.Items;

public class MyItemsFilterDto
{
    public string? Search { get; set; }
    public int? CategoryId { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public decimal? PriceFrom { get; set; }
    public decimal? PriceTo { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = Pagination.DefaultPageSize;

    public IReadOnlyList<ItemListDto> Items { get; set; } = Array.Empty<ItemListDto>();
    public int TotalCount { get; set; }
    public int TotalPages => Pagination.GetTotalPages(TotalCount, PageSize);
    public IEnumerable<SelectListItem> AvailableCategories { get; set; } = Array.Empty<SelectListItem>();

    public PaginationViewModel ToPagination()
    {
        var vm = new PaginationViewModel
        {
            Page = Page,
            PageSize = PageSize,
            TotalCount = TotalCount,
            TotalPages = TotalPages,
            Controller = "Items",
            Action = "My",
        };
        vm.PreserveRouteValues["Search"] = Search;
        vm.PreserveRouteValues["CategoryId"] = CategoryId;
        vm.PreserveRouteValues["DateFrom"] = DateFrom?.ToString("yyyy-MM-dd");
        vm.PreserveRouteValues["DateTo"] = DateTo?.ToString("yyyy-MM-dd");
        vm.PreserveRouteValues["PriceFrom"] = PriceFrom;
        vm.PreserveRouteValues["PriceTo"] = PriceTo;
        return vm;
    }
}
