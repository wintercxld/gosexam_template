using GosExamTemplate.Dtos.Common;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GosExamTemplate.Dtos.Items;

public class PublicItemsFilterDto
{
    public string? Search { get; set; }
    public string? Owner { get; set; }
    public int? TagId { get; set; }
    public decimal? PriceFrom { get; set; }
    public decimal? PriceTo { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = Pagination.DefaultPageSize;

    public IReadOnlyList<ItemListDto> Items { get; set; } = Array.Empty<ItemListDto>();
    public int TotalCount { get; set; }
    public int TotalPages => Pagination.GetTotalPages(TotalCount, PageSize);
    public IEnumerable<SelectListItem> AvailableTags { get; set; } = Array.Empty<SelectListItem>();
    public IEnumerable<SelectListItem> AvailableOwners { get; set; } = Array.Empty<SelectListItem>();

    public PaginationViewModel ToPagination()
    {
        var vm = new PaginationViewModel
        {
            Page = Page,
            PageSize = PageSize,
            TotalCount = TotalCount,
            TotalPages = TotalPages,
            Controller = "Home",
            Action = "Index",
        };
        vm.PreserveRouteValues["Search"] = Search;
        vm.PreserveRouteValues["Owner"] = Owner;
        vm.PreserveRouteValues["TagId"] = TagId;
        vm.PreserveRouteValues["PriceFrom"] = PriceFrom;
        vm.PreserveRouteValues["PriceTo"] = PriceTo;
        return vm;
    }
}
