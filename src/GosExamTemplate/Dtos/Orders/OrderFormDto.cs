using System.ComponentModel.DataAnnotations;
using GosExamTemplate.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GosExamTemplate.Dtos.Orders;

public class OrderFormDto
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Выберите объект")]
    [Display(Name = "Объект")]
    public int ItemId { get; set; }

    [Range(1, 1000, ErrorMessage = "Количество от 1 до 1000")]
    [Display(Name = "Количество")]
    public int Quantity { get; set; } = 1;

    [StringLength(500, ErrorMessage = "Не больше 500 символов")]
    [Display(Name = "Комментарий")]
    [DataType(DataType.MultilineText)]
    public string? Comment { get; set; }

    [Display(Name = "Статус")]
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public IEnumerable<SelectListItem> AvailableItems { get; set; } = Array.Empty<SelectListItem>();
}
