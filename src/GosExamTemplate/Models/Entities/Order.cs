using System.ComponentModel.DataAnnotations;

namespace GosExamTemplate.Models.Entities;

public enum OrderStatus
{
    Pending = 0,
    Confirmed = 1,
    Cancelled = 2,
    Completed = 3,
}

public class Order
{
    public int Id { get; set; }

    [Range(1, 1000)]
    public int Quantity { get; set; } = 1;

    [StringLength(500)]
    public string? Comment { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }

    public int ItemId { get; set; }
    public Item? Item { get; set; }
}
