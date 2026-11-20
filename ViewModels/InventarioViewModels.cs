using ElOferton.Constants;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ElOferton.ViewModels;

public class InventarioFiltroViewModel
{
    public string? Buscar { get; set; }

    public int? PuntoVentaId { get; set; }

    public bool? SoloStockBajo { get; set; }

    public List<InventarioItemViewModel> Items { get; set; } = [];

    public SelectListItem[] PuntosVenta { get; set; } = [];
}

public class InventarioItemViewModel
{
    public int ProductoId { get; set; }

    public string Codigo { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public int PuntoVentaId { get; set; }

    public string Ambiente { get; set; } = string.Empty;

    public int Cantidad { get; set; }

    public bool StockBajo { get; set; }
}

public class MovimientoFormViewModel
{
    [Required]
    [Display(Name = "Producto")]
    public int ProductoId { get; set; }

    [Required]
    [Display(Name = "Ambiente")]
    public int PuntoVentaId { get; set; }

    [Display(Name = "Ambiente destino")]
    public int? PuntoVentaDestinoId { get; set; }

    [Required]
    [Display(Name = "Tipo de movimiento")]
    public TipoMovimiento Tipo { get; set; }

    [Required]
    [Range(-100000, 100000)]
    [Display(Name = "Cantidad (negativa en ajustes)")]
    public int Cantidad { get; set; }

    [Required]
    [StringLength(250)]
    public string Motivo { get; set; } = string.Empty;

    public SelectListItem[] Productos { get; set; } = [];

    public SelectListItem[] PuntosVenta { get; set; } = [];

    public int StockActual { get; set; }
}

public class MovimientoFiltroViewModel
{
    public DateTime? FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }

    public int? ProductoId { get; set; }

    public int? PuntoVentaId { get; set; }

    public TipoMovimiento? Tipo { get; set; }

    public List<MovimientoItemViewModel> Items { get; set; } = [];

    public SelectListItem[] Productos { get; set; } = [];

    public SelectListItem[] PuntosVenta { get; set; } = [];
}

public class MovimientoItemViewModel
{
    public int Id { get; set; }

    public DateTime Fecha { get; set; }

    public int ProductoId { get; set; }

    public string Producto { get; set; } = string.Empty;

    public TipoMovimiento TipoMovimiento { get; set; }

    public string Tipo { get; set; } = string.Empty;

    public int PuntoVentaId { get; set; }

    public int Cantidad { get; set; }

    public string Ambiente { get; set; } = string.Empty;

    public string? AmbienteDestino { get; set; }

    public string Usuario { get; set; } = string.Empty;

    public string Motivo { get; set; } = string.Empty;

    public int StockAnterior { get; set; }

    public int StockPosterior { get; set; }
}
