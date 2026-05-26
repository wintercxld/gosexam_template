using GosExamTemplate.Dtos.Tags;
using GosExamTemplate.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GosExamTemplate.Controllers;

[Authorize]
public class TagsController(ITagService tags) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var model = await tags.GetAllAsync(ct);
        return View(model);
    }

    [HttpGet]
    public IActionResult Create() => View("Form", new TagDto());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TagDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View("Form", dto);
        await tags.CreateAsync(dto, ct);
        TempData["Toast"] = "Тег создан";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var model = await tags.GetAsync(id, ct);
        if (model is null) return NotFound();
        return View("Form", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TagDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View("Form", dto);
        var ok = await tags.UpdateAsync(dto, ct);
        if (!ok) return NotFound();
        TempData["Toast"] = "Тег обновлен";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var model = await tags.GetAsync(id, ct);
        if (model is null) return NotFound();
        return View(model);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
    {
        var ok = await tags.DeleteAsync(id, ct);
        if (!ok) return NotFound();
        TempData["Toast"] = "Тег удалён";
        return RedirectToAction(nameof(Index));
    }
}
