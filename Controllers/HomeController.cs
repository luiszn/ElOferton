using ElOferton.Models;
using ElOferton.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ElOferton.Controllers;

public sealed class HomeController(IDashboardService dashboardService) : BaseController
{
    public async Task<IActionResult> Index()
    {
        return View(await dashboardService.ObtenerAsync());
    }

    [AllowAnonymous]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}
