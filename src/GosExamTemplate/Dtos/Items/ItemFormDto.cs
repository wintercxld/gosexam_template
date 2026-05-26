using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GosExamTemplate.Dtos.Items;

public class ItemFormDto
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Введите название")]
    [StringLength(128, MinimumLength = 2, ErrorMessage = "От 2 до 128 символов")]
    [Display(Name = "Название")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите описание")]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "От 10 до 2000 символов")]
    [Display(Name = "Описание")]
    [DataType(DataType.MultilineText)]
    public string Description { get; set; } = string.Empty;

    [Range(0, 1_000_000, ErrorMessage = "Цена в диапазоне 0..1 000 000")]
    [Display(Name = "Цена")]
    public decimal Price { get; set; }

    [Url(ErrorMessage = "Введите корректную ссылку")]
    [StringLength(512)]
    [Display(Name = "Изображение (URL)")]
    public string? ImageUrl { get; set; }

    [Required(ErrorMessage = "Выберите категорию")]
    [Display(Name = "Категория")]
    public int CategoryId { get; set; }

    [Display(Name = "Теги")]
    public List<int> TagIds { get; set; } = new();

    public IEnumerable<SelectListItem> AvailableCategories { get; set; } = Array.Empty<SelectListItem>();
    public IEnumerable<SelectListItem> AvailableTags { get; set; } = Array.Empty<SelectListItem>();
}
