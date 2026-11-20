using ElOferton.Constants;
using ElOferton.Services.Interfaces;
using ElOferton.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElOferton.Controllers;

public sealed class ProductosController(IProductoService productoService) : BaseController
{
    public async Task<IActionResult> Index(string? buscar, int? categoriaId, int? marcaId, bool? soloActivos = true)
    {
        return View(await productoService.ObtenerListaAsync(buscar, categoriaId, marcaId, soloActivos));
    }

    public async Task<IActionResult> Details(int id)
    {
        var producto = await productoService.ObtenerDetalleAsync(id);
        return producto is null ? NotFound() : View(producto);
    }

    [Authorize(Policy = Politicas.Inventario)]
    public async Task<IActionResult> Create()
    {
        return View(await productoService.ObtenerFormularioAsync(null));
    }

    [HttpPost]
    [Authorize(Policy = Politicas.Inventario)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductoFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(await RecargarListasAsync(model));
        }

        var resultado = await productoService.GuardarAsync(model);
        Notificar(resultado);
        return resultado.Exitoso ? RedirectToAction(nameof(Index)) : View(await RecargarListasAsync(model));
    }

    [Authorize(Policy = Politicas.Inventario)]
    public async Task<IActionResult> Edit(int id)
    {
        var producto = await productoService.ObtenerFormularioAsync(id);
        return producto is null ? NotFound() : View(producto);
    }

    [HttpPost]
    [Authorize(Policy = Politicas.Inventario)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductoFormViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(await RecargarListasAsync(model));
        }

        var resultado = await productoService.GuardarAsync(model);
        Notificar(resultado);
        return resultado.Exitoso ? RedirectToAction(nameof(Index)) : View(await RecargarListasAsync(model));
    }

    [HttpPost]
    [Authorize(Policy = Politicas.Inventario)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleEstado(int id)
    {
        Notificar(await productoService.AlternarEstadoAsync(id));
        return RedirectToAction(nameof(Index));
    }

    private async Task<ProductoFormViewModel> RecargarListasAsync(ProductoFormViewModel model)
    {
        var formulario = await productoService.ObtenerFormularioAsync(model.Id);
        model.Categorias = formulario?.Categorias ?? [];
        model.Marcas = formulario?.Marcas ?? [];
        return model;
    }
}
