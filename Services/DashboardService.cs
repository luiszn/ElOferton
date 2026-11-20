using ElOferton.Constants;
using ElOferton.Data;
using ElOferton.Services.Interfaces;
using ElOferton.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ElOferton.Services;

public sealed class DashboardService(ElOfertonDbContext db) : IDashboardService
{
    public async Task<DashboardViewModel> ObtenerAsync()
    {
        var inicioDia = Reloj.InicioDia;
        var finDia = inicioDia.AddDays(1);
        var inicioMes = Reloj.InicioMes;
        var ventasCompletadas = db.Ventas.AsNoTracking().Where(v => v.Estado == EstadoVenta.Completada);

        var ventasHoy = await ventasCompletadas
            .Where(v => v.Fecha >= inicioDia && v.Fecha < finDia)
            .SumAsync(v => (decimal?)v.Total) ?? 0;
        var cantidadHoy = await ventasCompletadas
            .Where(v => v.Fecha >= inicioDia && v.Fecha < finDia)
            .CountAsync();
        var ventasMes = await ventasCompletadas
            .Where(v => v.Fecha >= inicioMes && v.Fecha < finDia)
            .SumAsync(v => (decimal?)v.Total) ?? 0;
        var productosRegistrados = await db.Productos.AsNoTracking().CountAsync(p => p.Activo);
        var clientesRegistrados = await db.Clientes.AsNoTracking().CountAsync(c => c.Activo);
        var ambientesActivos = await db.PuntosVenta.AsNoTracking().CountAsync(p => p.Activo);

        var stocks = await db.Inventarios.AsNoTracking()
            .GroupBy(i => i.ProductoId)
            .ToDictionaryAsync(g => g.Key, g => g.Sum(i => i.Cantidad));
        var productos = await db.Productos.AsNoTracking()
            .Where(p => p.Activo)
            .Select(p => new { p.Id, p.StockMinimo })
            .ToListAsync();
        var productosStockBajo = productos.Count(p => stocks.GetValueOrDefault(p.Id) <= p.StockMinimo);

        var ultimasVentas = await ventasCompletadas
            .OrderByDescending(v => v.Fecha)
            .Take(5)
            .Select(v => new VentaResumenViewModel
            {
                NumeroComprobante = v.NumeroComprobante,
                Fecha = v.Fecha,
                Cliente = v.Cliente != null ? v.Cliente.Nombre : v.ClienteOcasional ?? "Cliente ocasional",
                PuntoVenta = v.PuntoVenta.Nombre,
                Total = v.Total
            })
            .ToListAsync();

        var masVendidos = await db.DetallesVenta.AsNoTracking()
            .Where(d => d.Venta.Fecha >= inicioMes && d.Venta.Fecha < finDia && d.Venta.Estado == EstadoVenta.Completada)
            .GroupBy(d => new { d.ProductoId, d.NombreProducto })
            .Select(g => new ProductoMasVendidoViewModel
            {
                Nombre = g.Key.NombreProducto,
                Cantidad = g.Sum(d => d.Cantidad),
                Total = g.Sum(d => d.Subtotal)
            })
            .OrderByDescending(p => p.Cantidad)
            .Take(5)
            .ToListAsync();

        var ingresos = await ventasCompletadas
            .Where(v => v.Fecha >= inicioMes && v.Fecha < finDia)
            .GroupBy(v => v.MetodoPago.Nombre)
            .Select(g => new IngresoMetodoPagoViewModel
            {
                Metodo = g.Key,
                Total = g.Sum(v => v.Total),
                Cantidad = g.Count()
            })
            .OrderByDescending(x => x.Total)
            .ToListAsync();

        var inicioSerie = inicioDia.AddDays(-6);
        var datosSerie = await ventasCompletadas
            .Where(v => v.Fecha >= inicioSerie && v.Fecha < finDia)
            .GroupBy(v => v.Fecha.Date)
            .Select(g => new { Fecha = g.Key, Total = g.Sum(v => v.Total) })
            .ToListAsync();

        var serie = Enumerable.Range(0, 7)
            .Select(i =>
            {
                var fecha = inicioSerie.AddDays(i);
                return new PuntoSerieViewModel
                {
                    Fecha = fecha,
                    Total = datosSerie.FirstOrDefault(d => d.Fecha == fecha)?.Total ?? 0
                };
            })
            .ToList();

        return new DashboardViewModel
        {
            VentasHoy = ventasHoy,
            VentasHoyCantidad = cantidadHoy,
            VentasMes = ventasMes,
            ProductosRegistrados = productosRegistrados,
            ClientesRegistrados = clientesRegistrados,
            AmbientesActivos = ambientesActivos,
            ProductosStockBajo = productosStockBajo,
            UltimasVentas = ultimasVentas,
            ProductosMasVendidos = masVendidos,
            IngresosPorMetodoPago = ingresos,
            SerieVentas = serie
        };
    }
}
