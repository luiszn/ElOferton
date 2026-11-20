using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ElOferton.ViewModels;

public class PosViewModel
{
    [Required]
    [Display(Name = "Punto de venta")]
    public int PuntoVentaId { get; set; }

    [Display(Name = "Cliente")]
    public int? ClienteId { get; set; }

    [StringLength(150)]
    [Display(Name = "Cliente ocasional")]
    public string? ClienteOcasional { get; set; }

    [Required]
    [Display(Name = "Método de pago")]
    public int MetodoPagoId { get; set; }

    [Range(0, 10)]
    [Display(Name = "Descuento (%)")]
    public decimal PorcentajeDescuento { get; set; }

    [Display(Name = "Monto recibido")]
    public decimal? MontoRecibido { get; set; }

    [StringLength(300)]
    public string? Observaciones { get; set; }

    public List<PosItemViewModel> Items { get; set; } = [];

    public SelectListItem[] Clientes { get; set; } = [];

    public SelectListItem[] MetodosPago { get; set; } = [];

    public SelectListItem[] PuntosVenta { get; set; } = [];

    public bool EsAdministrador { get; set; }

    public decimal Subtotal => Items.Sum(i => i.Subtotal);

    public decimal Descuento => Math.Round(Subtotal * PorcentajeDescuento / 100m, 2, MidpointRounding.AwayFromZero);

    public decimal Total => Subtotal - Descuento;

    public decimal Vuelto => MontoRecibido.HasValue ? Math.Max(0, MontoRecibido.Value - Total) : 0;
}

public class PosItemViewModel
{
    public int ProductoId { get; set; }

    public string Codigo { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public int Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public int StockDisponible { get; set; }

    public decimal Subtotal { get; set; }
}

public class VentaConfirmacionViewModel
{
    public int VentaId { get; set; }

    public string NumeroComprobante { get; set; } = string.Empty;

    public DateTime Fecha { get; set; }

    public string Cliente { get; set; } = string.Empty;

    public string PuntoVenta { get; set; } = string.Empty;

    public string Vendedor { get; set; } = string.Empty;

    public string MetodoPago { get; set; } = string.Empty;

    public decimal Subtotal { get; set; }

    public decimal Descuento { get; set; }

    public decimal Total { get; set; }

    public decimal? MontoRecibido { get; set; }

    public decimal? Vuelto { get; set; }

    public List<PosItemViewModel> Items { get; set; } = [];
}

public class VentaFiltroViewModel
{
    public DateTime? FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }

    public int? PuntoVentaId { get; set; }

    public int? UsuarioId { get; set; }

    public List<VentaItemViewModel> Ventas { get; set; } = [];

    public SelectListItem[] PuntosVenta { get; set; } = [];

    public SelectListItem[] Vendedores { get; set; } = [];
}

public class VentaItemViewModel
{
    public int Id { get; set; }

    public string NumeroComprobante { get; set; } = string.Empty;

    public DateTime Fecha { get; set; }

    public string Cliente { get; set; } = string.Empty;

    public string PuntoVenta { get; set; } = string.Empty;

    public string Vendedor { get; set; } = string.Empty;

    public string MetodoPago { get; set; } = string.Empty;

    public int PuntoVentaId { get; set; }

    public int UsuarioId { get; set; }

    public decimal Total { get; set; }

    public string Estado { get; set; } = string.Empty;
}

public class VentaDetalleViewModel
{
    public VentaConfirmacionViewModel Venta { get; set; } = new();

    public string? Observaciones { get; set; }
}
