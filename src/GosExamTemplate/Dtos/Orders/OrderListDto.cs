using GosExamTemplate.Models.Entities;

namespace GosExamTemplate.Dtos.Orders;

public class OrderListDto
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public string? Comment { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }

    public int ItemId { get; set; }
    public string ItemTitle { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
}
