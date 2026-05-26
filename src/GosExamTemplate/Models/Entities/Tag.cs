using System.ComponentModel.DataAnnotations;

namespace GosExamTemplate.Models.Entities;

public class Tag
{
    public int Id { get; set; }

    [Required]
    [StringLength(32, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    public ICollection<ItemTag> ItemTags { get; set; } = new List<ItemTag>();
}
