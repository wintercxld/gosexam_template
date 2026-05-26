using System.ComponentModel.DataAnnotations;

namespace GosExamTemplate.Dtos.Categories;

public class CategoryDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Введите название категории")]
    [StringLength(64, MinimumLength = 2, ErrorMessage = "От 2 до 64 символов")]
    [Display(Name = "Название")]
    public string Name { get; set; } = string.Empty;

    [StringLength(256)]
    [Display(Name = "Описание")]
    public string? Description { get; set; }

    public int ItemsCount { get; set; }
}
