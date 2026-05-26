using System.ComponentModel.DataAnnotations;

namespace GosExamTemplate.Dtos.Tags;

public class TagDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Введите название тега")]
    [StringLength(32, MinimumLength = 2, ErrorMessage = "От 2 до 32 символов")]
    [Display(Name = "Тег")]
    public string Name { get; set; } = string.Empty;

    public int ItemsCount { get; set; }
}
