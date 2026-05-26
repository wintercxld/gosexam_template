using Microsoft.AspNetCore.Identity;

namespace GosExamTemplate.Models.Entities;

public class ApplicationUser : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Item> Items { get; set; } = new List<Item>();

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
