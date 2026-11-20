using ElOferton.Constants;
using ElOferton.Services.Interfaces;
using ElOferton.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElOferton.Controllers;

[Authorize(Policy = Politicas.Ventas)]
public sealed class ClientesController(IClienteService clienteService) : BaseController
{
    public async Task<IActionResult> Index(string? buscar)
    {
        return View(await clienteService.ObtenerListaAsync(buscar));
    }

    public async Task<IActionResult> Details(int id)
    {
        var cliente = await clienteService.ObtenerDetalleAsync(id);
        return cliente is null ? NotFound() : View(cliente);
    }

    public async Task<IActionResult> Create()
    {
        return View(await clienteService.ObtenerFormularioAsync(null));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ClienteFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var resultado = await clienteService.GuardarAsync(model);
        Notificar(resultado);
        return resultado.Exitoso ? RedirectToAction(nameof(Index)) : View(model);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var cliente = await clienteService.ObtenerFormularioAsync(id);
        return cliente is null ? NotFound() : View(cliente);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ClienteFormViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var resultado = await clienteService.GuardarAsync(model);
        Notificar(resultado);
        return resultado.Exitoso ? RedirectToAction(nameof(Index)) : View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleEstado(int id)
    {
        Notificar(await clienteService.AlternarEstadoAsync(id));
        return RedirectToAction(nameof(Index));
    }
}
