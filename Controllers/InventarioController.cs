using ElOferton.Constants;
using ElOferton.Services.Interfaces;
using ElOferton.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElOferton.Controllers;

[Authorize(Policy = Politicas.Inventario)]
public sealed class InventarioController(IInventarioService inventarioService) : BaseController
{
    public async Task<IActionResult> Existencias(string? buscar, int? puntoVentaId, bool? soloStockBajo)
    {
        return View(await inventarioService.ObtenerInventarioAsync(buscar, puntoVentaId, soloStockBajo));
    }

    public async Task<IActionResult> Movimientos(MovimientoFiltroViewModel filtro)
    {
        return View(await inventarioService.ObtenerMovimientosAsync(filtro));
    }

    public async Task<IActionResult> NuevoMovimiento()
    {
        return View(await inventarioService.ObtenerFormularioMovimientoAsync());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> NuevoMovimiento(MovimientoFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(await RecargarFormularioAsync(model));
        }

        var resultado = await inventarioService.RegistrarMovimientoAsync(model, UsuarioActualId);
        Notificar(resultado);
        if (!resultado.Exitoso)
        {
            return View(await RecargarFormularioAsync(model));
        }

        return RedirectToAction(nameof(Movimientos));
    }

    [HttpGet]
    public async Task<IActionResult> Stock(int productoId, int puntoVentaId)
    {
        return Json(new { cantidad = await inventarioService.ObtenerStockAsync(productoId, puntoVentaId) });
    }

    private async Task<MovimientoFormViewModel> RecargarFormularioAsync(MovimientoFormViewModel model)
    {
        var formulario = await inventarioService.ObtenerFormularioMovimientoAsync();
        model.Productos = formulario.Productos;
        model.PuntosVenta = formulario.PuntosVenta;
        return model;
    }
}
