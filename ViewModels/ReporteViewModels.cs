using Microsoft.AspNetCore.Mvc.Rendering;

namespace ElOferton.ViewModels;

public static class TiposReporte
{
    public const string VentasPorFecha = "VentasPorFecha";
    public const string VentasPorMes = "VentasPorMes";
    public const string VentasPorVendedor = "VentasPorVendedor";
    public const string VentasPorPuntoVenta = "VentasPorPuntoVenta";
    public const string ProductosMasVendidos = "ProductosMasVendidos";
    public const string ProductosBajoStock = "ProductosBajoStock";
    public const string MovimientosInventario = "MovimientosInventario";
    public const string MetodosPago = "MetodosPago";

    public static readonly SelectListItem[] Opciones =
    [
        new() { Value = VentasPorFecha, Text = "Ventas por fecha" },
        new() { Value = VentasPorMes, Text = "Ventas por mes" },
        new() { Value = VentasPorVendedor, Text = "Ventas por vendedor" },
        new() { Value = VentasPorPuntoVenta, Text = "Ventas por punto de venta" },
        new() { Value = ProductosMasVendidos, Text = "Productos más vendidos" },
        new() { Value = ProductosBajoStock, Text = "Productos con bajo stock" },
        new() { Value = MovimientosInventario, Text = "Movimientos de inventario" },
        new() { Value = MetodosPago, Text = "Métodos de pago" }
    ];
}

public class ReporteFiltroViewModel
{
    public string TipoReporte { get; set; } = TiposReporte.VentasPorFecha;

    public DateTime? FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }

    public int? PuntoVentaId { get; set; }

    public decimal? TotalGeneral { get; set; }

    public List<ReporteFilaViewModel> Filas { get; set; } = [];

    public SelectListItem[] Tipos { get; set; } = TiposReporte.Opciones;

    public SelectListItem[] PuntosVenta { get; set; } = [];

    public string Titulo { get; set; } = string.Empty;
}

public class ReporteFilaViewModel
{
    public string Grupo { get; set; } = string.Empty;

    public string? Detalle { get; set; }

    public int Cantidad { get; set; }

    public decimal Total { get; set; }
}
