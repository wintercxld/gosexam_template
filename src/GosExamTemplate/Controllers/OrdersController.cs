using System.Security.Claims;
using GosExamTemplate.Dtos.Orders;
using GosExamTemplate.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GosExamTemplate.Controllers;

[Authorize]
public class OrdersController(IOrderService orders) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(int? page, int? pageSize, CancellationToken ct)
    {
        var model = await orders.GetMyOrdersAsync(GetCurrentUserId(), page, pageSize, ct);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create(int? itemId, CancellationToken ct)
    {
        var model = await orders.BuildCreateFormAsync(itemId, ct);
        return View("Form", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(OrderFormDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            var fresh = await orders.BuildCreateFormAsync(dto.ItemId, ct);
            dto.AvailableItems = fresh.AvailableItems;
            return View("Form", dto);
        }

        await orders.CreateAsync(dto, GetCurrentUserId(), ct);
        TempData["Toast"] = "Заказ создан";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var model = await orders.GetForEditAsync(id, GetCurrentUserId(), ct);
        if (model is null) return Forbid();
        return View("Form", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(OrderFormDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            var fresh = await orders.BuildCreateFormAsync(dto.ItemId, ct);
            dto.AvailableItems = fresh.AvailableItems;
            return View("Form", dto);
        }

        var ok = await orders.UpdateAsync(dto, GetCurrentUserId(), ct);
        if (!ok) return Forbid();

        TempData["Toast"] = "Заказ обновлён";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var model = await orders.GetAsync(id, GetCurrentUserId(), ct);
        if (model is null) return Forbid();
        return View(model);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
    {
        var ok = await orders.DeleteAsync(id, GetCurrentUserId(), ct);
        if (!ok) return Forbid();

        TempData["Toast"] = "Заказ удалён";
        return RedirectToAction(nameof(Index));
    }

    private string GetCurrentUserId()
        => User.FindFirstValue(ClaimTypes.NameIdentifier)
           ?? throw new InvalidOperationException("Пользователь не аутентифицирован");
}
