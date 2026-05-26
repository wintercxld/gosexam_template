using System.ComponentModel.DataAnnotations;

namespace GosExamTemplate.Models.Entities;

public class Item
{
    public int Id { get; set; }

    [Required]
    [StringLength(128, MinimumLength = 2)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(2000, MinimumLength = 10)]
    public string Description { get; set; } = string.Empty;

    [Range(0, 1_000_000)]
    public decimal Price { get; set; }

    [StringLength(512)]
    [Url]
    public string? ImageUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public string OwnerId { get; set; } = string.Empty;
    public ApplicationUser? Owner { get; set; }

    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    public ICollection<ItemTag> ItemTags { get; set; } = new List<ItemTag>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
