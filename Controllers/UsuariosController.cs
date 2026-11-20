using ElOferton.Constants;
using ElOferton.Services.Interfaces;
using ElOferton.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElOferton.Controllers;

[Authorize(Policy = Politicas.Usuarios)]
public sealed class UsuariosController(IUsuarioService usuarioService) : BaseController
{
    public async Task<IActionResult> Index()
    {
        return View(await usuarioService.ObtenerUsuariosAsync());
    }

    public async Task<IActionResult> Create()
    {
        return View(await usuarioService.ObtenerFormularioCrearAsync());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CrearUsuarioViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(await RecargarFormularioAsync(model));
        }

        var resultado = await usuarioService.CrearAsync(model);
        Notificar(resultado);
        return resultado.Exitoso ? RedirectToAction(nameof(Index)) : View(await RecargarFormularioAsync(model));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleEstado(int id)
    {
        if (id == UsuarioActualId)
        {
            TempData["Error"] = "No puede desactivar su propio usuario.";
            return RedirectToAction(nameof(Index));
        }

        Notificar(await usuarioService.AlternarEstadoAsync(id));
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarRol(CambiarRolViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction(nameof(Index));
        }

        Notificar(await usuarioService.CambiarRolAsync(model.UsuarioId, model.Rol));
        return RedirectToAction(nameof(Index));
    }

    private async Task<CrearUsuarioViewModel> RecargarFormularioAsync(CrearUsuarioViewModel model)
    {
        var formulario = await usuarioService.ObtenerFormularioCrearAsync();
        model.Roles = formulario.Roles;
        model.PuntosVenta = formulario.PuntosVenta;
        return model;
    }
}
