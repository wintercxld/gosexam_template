using GosExamTemplate.Dtos.Categories;
using GosExamTemplate.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GosExamTemplate.Controllers;

[Authorize]
public class CategoriesController(ICategoryService categories) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(int? page, int? pageSize, CancellationToken ct)
    {
        var model = await categories.GetPagedAsync(page, pageSize, ct);
        return View(model);
    }

    [HttpGet]
    public IActionResult Create() => View("Form", new CategoryDto());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View("Form", dto);
        await categories.CreateAsync(dto, ct);
        TempData["Toast"] = "Категория создана";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var model = await categories.GetAsync(id, ct);
        if (model is null) return NotFound();
        return View("Form", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(CategoryDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View("Form", dto);
        var ok = await categories.UpdateAsync(dto, ct);
        if (!ok) return NotFound();
        TempData["Toast"] = "Категория обновлена";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var model = await categories.GetAsync(id, ct);
        if (model is null) return NotFound();
        return View(model);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
    {
        var ok = await categories.DeleteAsync(id, ct);
        if (!ok) return NotFound();
        TempData["Toast"] = "Категория удалена";
        return RedirectToAction(nameof(Index));
    }
}
