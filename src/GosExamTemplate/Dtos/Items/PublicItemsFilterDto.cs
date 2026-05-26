using Microsoft.AspNetCore.Mvc.Rendering;

namespace GosExamTemplate.Dtos.Items;

public class PublicItemsFilterDto
{
    public string? Search { get; set; }
    public string? Owner { get; set; }
    public int? TagId { get; set; }
    public decimal? PriceFrom { get; set; }
    public decimal? PriceTo { get; set; }

    public IReadOnlyList<ItemListDto> Items { get; set; } = Array.Empty<ItemListDto>();
    public IEnumerable<SelectListItem> AvailableTags { get; set; } = Array.Empty<SelectListItem>();
    public IEnumerable<SelectListItem> AvailableOwners { get; set; } = Array.Empty<SelectListItem>();
}
