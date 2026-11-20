using ElOferton.Constants;
using ElOferton.Services.Interfaces;
using ElOferton.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElOferton.Controllers;

[Authorize(Policy = Politicas.Ventas)]
public sealed class VentasController(IVentaService ventaService) : BaseController
{
    public async Task<IActionResult> Index()
    {
        return View(await ventaService.PrepararPosAsync(PuntoVentaActualId, EsAdministrador));
    }

    [HttpGet]
    public async Task<IActionResult> BuscarProductos(string? termino, int puntoVentaId)
    {
        return Json(await ventaService.BuscarProductosAsync(termino, puntoVentaId));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Registrar(PosViewModel model)
    {
        var resultado = await ventaService.RegistrarAsync(model, UsuarioActualId, EsAdministrador);
        if (resultado.Exitoso && resultado.Comprobante is not null)
        {
            TempData["Mensaje"] = resultado.Mensaje;
            return View("Comprobante", resultado.Comprobante);
        }

        ModelState.AddModelError(string.Empty, resultado.Mensaje);
        var pos = await ventaService.PrepararPosAsync(model.PuntoVentaId, EsAdministrador);
        pos.ClienteId = model.ClienteId;
        pos.ClienteOcasional = model.ClienteOcasional;
        pos.MetodoPagoId = model.MetodoPagoId;
        pos.PorcentajeDescuento = model.PorcentajeDescuento;
        pos.MontoRecibido = model.MontoRecibido;
        pos.Observaciones = model.Observaciones;
        pos.Items = model.Items;
        return View("Index", pos);
    }

    public async Task<IActionResult> Historial(VentaFiltroViewModel filtro)
    {
        return View(await ventaService.ObtenerVentasAsync(filtro));
    }

    public async Task<IActionResult> Detalle(int id)
    {
        var venta = await ventaService.ObtenerDetalleAsync(id);
        return venta is null ? NotFound() : View(venta);
    }

    public async Task<IActionResult> Reimprimir(int id)
    {
        var venta = await ventaService.ObtenerDetalleAsync(id);
        return venta is null ? NotFound() : View(venta);
    }
}
