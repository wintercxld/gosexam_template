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

    public IReadOnlyList<ItemListDto> Items { get; set; } = Array.Empty<ItemListDto>();
    public IEnumerable<SelectListItem> AvailableCategories { get; set; } = Array.Empty<SelectListItem>();
}
