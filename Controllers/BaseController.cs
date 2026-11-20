using ElOferton.Constants;
using ElOferton.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ElOferton.Controllers;

[Authorize]
public abstract class BaseController : Controller
{
    protected int UsuarioActualId =>
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;

    protected int PuntoVentaActualId =>
        int.TryParse(User.FindFirstValue("PuntoVentaId"), out var id) ? id : 0;

    protected bool EsAdministrador => User.IsInRole(Roles.Administrador);

    protected void Notificar(ResultadoOperacion resultado)
    {
        TempData[resultado.Exitoso ? "Mensaje" : "Error"] = resultado.Mensaje;
    }
}
