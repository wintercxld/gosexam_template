using System.Diagnostics;
using GosExamTemplate.Dtos.Items;
using GosExamTemplate.Models;
using GosExamTemplate.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GosExamTemplate.Controllers;

[AllowAnonymous]
public class HomeController(IItemService items) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] PublicItemsFilterDto filter, CancellationToken ct)
    {
        var model = await items.GetPublicItemsAsync(filter, ct);
        return View(model);
    }

    [HttpGet]
    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
        => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
