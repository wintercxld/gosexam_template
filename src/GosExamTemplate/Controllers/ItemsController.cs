using System.Security.Claims;
using GosExamTemplate.Dtos.Items;
using GosExamTemplate.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GosExamTemplate.Controllers;

[Authorize]
public class ItemsController(IItemService items) : Controller
{
    [HttpGet]
    public async Task<IActionResult> My([FromQuery] MyItemsFilterDto filter, CancellationToken ct)
    {
        var ownerId = GetCurrentUserId();
        var model = await items.GetMyItemsAsync(ownerId, filter, ct);
        return View(model);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        var model = await items.GetDetailsAsync(id, ct);
        if (model is null) return NotFound();
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        var model = await items.BuildCreateFormAsync(ct);
        return View("Form", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ItemFormDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            await ReloadFormOptionsAsync(dto, ct);
            return View("Form", dto);
        }

        var id = await items.CreateAsync(dto, GetCurrentUserId(), ct);
        TempData["Toast"] = "Запись создана";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var model = await items.GetForEditAsync(id, GetCurrentUserId(), ct);
        if (model is null) return Forbid();
        return View("Form", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ItemFormDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            await ReloadFormOptionsAsync(dto, ct);
            return View("Form", dto);
        }

        var ok = await items.UpdateAsync(dto, GetCurrentUserId(), ct);
        if (!ok) return Forbid();

        TempData["Toast"] = "Запись обновлена";
        return RedirectToAction(nameof(Details), new { id = dto.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var model = await items.GetDetailsAsync(id, ct);
        if (model is null) return NotFound();
        if (model.OwnerId != GetCurrentUserId()) return Forbid();
        return View(model);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
    {
        var ok = await items.DeleteAsync(id, GetCurrentUserId(), ct);
        if (!ok) return Forbid();

        TempData["Toast"] = "Запись удалена";
        return RedirectToAction(nameof(My));
    }

    private async Task ReloadFormOptionsAsync(ItemFormDto dto, CancellationToken ct)
    {
        var fresh = await items.BuildCreateFormAsync(ct);
        dto.AvailableCategories = fresh.AvailableCategories;
        dto.AvailableTags = fresh.AvailableTags;
    }

    private string GetCurrentUserId()
        => User.FindFirstValue(ClaimTypes.NameIdentifier)
           ?? throw new InvalidOperationException("Пользователь не аутентифицирован");
}
