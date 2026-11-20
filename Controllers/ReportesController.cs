using ElOferton.Constants;
using ElOferton.Services.Interfaces;
using ElOferton.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace ElOferton.Controllers;

[Authorize(Policy = Politicas.Reportes)]
public sealed class ReportesController(IReporteService reporteService) : BaseController
{
    public async Task<IActionResult> Index(ReporteFiltroViewModel filtro)
    {
        return View(await reporteService.ObtenerAsync(filtro));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExportarCsv(ReporteFiltroViewModel filtro)
    {
        var reporte = await reporteService.ObtenerAsync(filtro);
        var lineas = new List<string> { "Grupo,Detalle,Cantidad,Total" };
        lineas.AddRange(reporte.Filas.Select(f =>
            $"{Escapar(f.Grupo)},{Escapar(f.Detalle ?? string.Empty)},{f.Cantidad},{f.Total.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)}"));

        var contenido = string.Join("\r\n", lineas);
        var bytes = Encoding.UTF8.GetPreamble()
            .Concat(Encoding.UTF8.GetBytes(contenido))
            .ToArray();

        return File(bytes, "text/csv; charset=utf-8", $"reporte-{reporte.TipoReporte}-{DateTime.Now:yyyyMMdd-HHmm}.csv");
    }

    private static string Escapar(string valor)
    {
        var texto = valor.Replace("\"", "\"\"");
        return texto.Contains(',') || texto.Contains('"') || texto.Contains('\n')
            ? $"\"{texto}\""
            : texto;
    }
}
