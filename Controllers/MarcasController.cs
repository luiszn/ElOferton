using ElOferton.Constants;
using ElOferton.Services.Interfaces;
using ElOferton.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElOferton.Controllers;

public sealed class MarcasController(ICatalogoService catalogoService) : BaseController
{
    public async Task<IActionResult> Index(string? buscar)
    {
        return View(await catalogoService.ObtenerMarcasAsync(buscar));
    }

    [Authorize(Policy = Politicas.Inventario)]
    public async Task<IActionResult> Create()
    {
        return View(await catalogoService.ObtenerMarcaFormularioAsync(null));
    }

    [HttpPost]
    [Authorize(Policy = Politicas.Inventario)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CatalogoFormViewModel model)
    {
        Preparar(model);
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var resultado = await catalogoService.GuardarMarcaAsync(model);
        Notificar(resultado);
        return resultado.Exitoso ? RedirectToAction(nameof(Index)) : View(model);
    }

    [Authorize(Policy = Politicas.Inventario)]
    public async Task<IActionResult> Edit(int id)
    {
        var modelo = await catalogoService.ObtenerMarcaFormularioAsync(id);
        return modelo is null ? NotFound() : View(modelo);
    }

    [HttpPost]
    [Authorize(Policy = Politicas.Inventario)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CatalogoFormViewModel model)
    {
        Preparar(model);
        if (id != model.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var resultado = await catalogoService.GuardarMarcaAsync(model);
        Notificar(resultado);
        return resultado.Exitoso ? RedirectToAction(nameof(Index)) : View(model);
    }

    [HttpPost]
    [Authorize(Policy = Politicas.Inventario)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleEstado(int id)
    {
        Notificar(await catalogoService.AlternarMarcaAsync(id));
        return RedirectToAction(nameof(Index));
    }

    private static void Preparar(CatalogoFormViewModel model)
    {
        model.Titulo = "Marca";
        model.Controlador = "Marcas";
    }
}
