using ElOferton.Constants;
using ElOferton.Data;
using ElOferton.Models.Entities;
using ElOferton.Services.Interfaces;
using ElOferton.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ElOferton.Services;

public sealed class ReporteService(ElOfertonDbContext db) : IReporteService
{
    public async Task<ReporteFiltroViewModel> ObtenerAsync(ReporteFiltroViewModel filtro)
    {
        var inicio = filtro.FechaInicio?.Date ?? Reloj.InicioMes;
        var finExclusivo = filtro.FechaFin.HasValue
            ? filtro.FechaFin.Value.Date.AddDays(1)
            : Reloj.InicioDia.AddDays(1);

        filtro.FechaInicio = inicio;
        filtro.FechaFin = finExclusivo.AddDays(-1);

        var ventas = db.Ventas.AsNoTracking()
            .Where(v => v.Estado == EstadoVenta.Completada && v.Fecha >= inicio && v.Fecha < finExclusivo);

        if (filtro.PuntoVentaId.HasValue)
        {
            ventas = ventas.Where(v => v.PuntoVentaId == filtro.PuntoVentaId.Value);
        }

        filtro.Titulo = ObtenerTitulo(filtro.TipoReporte);

        filtro.Filas = filtro.TipoReporte switch
        {
            TiposReporte.VentasPorFecha => await ObtenerVentasPorFechaAsync(ventas),

            TiposReporte.VentasPorMes => await ObtenerVentasPorMesAsync(ventas),

            TiposReporte.VentasPorVendedor => await ventas
                .GroupBy(v => v.Usuario.NombreCompleto)
                .Select(g => new ReporteFilaViewModel
                {
                    Grupo = g.Key,
                    Cantidad = g.Count(),
                    Total = g.Sum(v => v.Total)
                })
                .OrderByDescending(f => f.Total)
                .ToListAsync(),

            TiposReporte.VentasPorPuntoVenta => await ventas
                .GroupBy(v => v.PuntoVenta.Nombre)
                .Select(g => new ReporteFilaViewModel
                {
                    Grupo = g.Key,
                    Cantidad = g.Count(),
                    Total = g.Sum(v => v.Total)
                })
                .OrderByDescending(f => f.Total)
                .ToListAsync(),

            TiposReporte.ProductosMasVendidos => await db.DetallesVenta.AsNoTracking()
                .Where(d => d.Venta.Estado == EstadoVenta.Completada
                            && d.Venta.Fecha >= inicio
                            && d.Venta.Fecha < finExclusivo
                            && (!filtro.PuntoVentaId.HasValue || d.Venta.PuntoVentaId == filtro.PuntoVentaId.Value))
                .GroupBy(d => new { d.ProductoId, d.CodigoProducto, d.NombreProducto })
                .Select(g => new ReporteFilaViewModel
                {
                    Grupo = g.Key.NombreProducto,
                    Detalle = g.Key.CodigoProducto,
                    Cantidad = g.Sum(d => d.Cantidad),
                    Total = g.Sum(d => d.Subtotal)
                })
                .OrderByDescending(f => f.Cantidad)
                .ToListAsync(),

            TiposReporte.ProductosBajoStock => await ObtenerProductosBajoStockAsync(),

            TiposReporte.MovimientosInventario => await ObtenerMovimientosAsync(inicio, finExclusivo, filtro.PuntoVentaId),

            TiposReporte.MetodosPago => await ventas
                .GroupBy(v => v.MetodoPago.Nombre)
                .Select(g => new ReporteFilaViewModel
                {
                    Grupo = g.Key,
                    Cantidad = g.Count(),
                    Total = g.Sum(v => v.Total)
                })
                .OrderByDescending(f => f.Total)
                .ToListAsync(),

            _ => []
        };

        filtro.TotalGeneral = filtro.Filas.Sum(f => f.Total);
        filtro.PuntosVenta = await ServicioAyuda.PuntosVentaAsync(db);
        return filtro;
    }

    private static async Task<List<ReporteFilaViewModel>> ObtenerVentasPorFechaAsync(IQueryable<Venta> ventas)
    {
        var datos = await ventas
            .GroupBy(v => v.Fecha.Date)
            .Select(g => new { Fecha = g.Key, Cantidad = g.Count(), Total = g.Sum(v => v.Total) })
            .OrderBy(g => g.Fecha)
            .ToListAsync();

        return datos.Select(g => new ReporteFilaViewModel
        {
            Grupo = g.Fecha.ToString("dd/MM/yyyy"),
            Cantidad = g.Cantidad,
            Total = g.Total
        }).ToList();
    }

    private static async Task<List<ReporteFilaViewModel>> ObtenerVentasPorMesAsync(IQueryable<Venta> ventas)
    {
        var datos = await ventas
            .GroupBy(v => new { v.Fecha.Year, v.Fecha.Month })
            .Select(g => new { g.Key.Year, g.Key.Month, Cantidad = g.Count(), Total = g.Sum(v => v.Total) })
            .OrderBy(g => g.Year).ThenBy(g => g.Month)
            .ToListAsync();

        return datos.Select(g => new ReporteFilaViewModel
        {
            Grupo = $"{g.Year}-{g.Month:00}",
            Cantidad = g.Cantidad,
            Total = g.Total
        }).ToList();
    }

    private async Task<List<ReporteFilaViewModel>> ObtenerProductosBajoStockAsync()
    {
        var stocks = await db.Inventarios.AsNoTracking()
            .GroupBy(i => i.ProductoId)
            .Select(g => new { ProductoId = g.Key, Cantidad = g.Sum(i => i.Cantidad) })
            .ToListAsync();

        var minimos = await db.Productos.AsNoTracking()
            .Where(p => p.Activo)
            .Select(p => new { p.Id, p.Codigo, p.Nombre, p.StockMinimo })
            .ToListAsync();

        return (from p in minimos
                join s in stocks on p.Id equals s.ProductoId into existencias
                from s in existencias.DefaultIfEmpty()
                where (s == null ? 0 : s.Cantidad) <= p.StockMinimo
                select new ReporteFilaViewModel
                {
                    Grupo = p.Nombre,
                    Detalle = p.Codigo,
                    Cantidad = s == null ? 0 : s.Cantidad,
                    Total = 0
                })
            .OrderBy(f => f.Grupo)
            .ToList();
    }

    private async Task<List<ReporteFilaViewModel>> ObtenerMovimientosAsync(
        DateTime inicio,
        DateTime finExclusivo,
        int? puntoVentaId)
    {
        var datos = await (from m in db.MovimientosInventario.AsNoTracking()
                           join p in db.Productos.AsNoTracking() on m.ProductoId equals p.Id
                           join pv in db.PuntosVenta.AsNoTracking() on m.PuntoVentaId equals pv.Id
                           join u in db.Users.AsNoTracking() on m.UsuarioId equals u.Id
                           where m.Fecha >= inicio && m.Fecha < finExclusivo
                                 && (!puntoVentaId.HasValue || m.PuntoVentaId == puntoVentaId.Value)
                           orderby m.Fecha descending
                           select new
                           {
                               m.Fecha,
                               Producto = p.Nombre,
                               Ambiente = pv.Nombre,
                               Usuario = u.NombreCompleto,
                               m.Motivo,
                               m.Cantidad
                           })
            .Take(500)
            .ToListAsync();

        return datos.Select(d => new ReporteFilaViewModel
        {
            Grupo = d.Fecha.ToString("dd/MM/yyyy HH:mm"),
            Detalle = $"{d.Producto} | {d.Ambiente} | {d.Usuario} | {d.Motivo}",
            Cantidad = d.Cantidad,
            Total = 0
        }).ToList();
    }

    private static string ObtenerTitulo(string tipo) => tipo switch
    {
        TiposReporte.VentasPorFecha => "Ventas por fecha",
        TiposReporte.VentasPorMes => "Ventas por mes",
        TiposReporte.VentasPorVendedor => "Ventas por vendedor",
        TiposReporte.VentasPorPuntoVenta => "Ventas por punto de venta",
        TiposReporte.ProductosMasVendidos => "Productos más vendidos",
        TiposReporte.ProductosBajoStock => "Productos con bajo stock",
        TiposReporte.MovimientosInventario => "Movimientos de inventario",
        TiposReporte.MetodosPago => "Métodos de pago utilizados",
        _ => "Reportes"
    };
}
