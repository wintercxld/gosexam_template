using System.ComponentModel.DataAnnotations;

namespace GosExamTemplate.Models.Entities;

public class Category
{
    public int Id { get; set; }

    [Required]
    [StringLength(64, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [StringLength(256)]
    public string? Description { get; set; }

    public ICollection<Item> Items { get; set; } = new List<Item>();
}
