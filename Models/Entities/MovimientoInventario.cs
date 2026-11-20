using ElOferton.Constants;
using ElOferton.Models.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace ElOferton.Models.Entities;

public class MovimientoInventario
{
    public int Id { get; set; }

    public DateTime Fecha { get; set; } = DateTime.UtcNow;

    public int ProductoId { get; set; }

    public TipoMovimiento Tipo { get; set; }

    public int Cantidad { get; set; }

    public int PuntoVentaId { get; set; }

    public int? PuntoVentaDestinoId { get; set; }

    public int? VentaId { get; set; }

    public int UsuarioId { get; set; }

    [StringLength(250)]
    public string Motivo { get; set; } = null!;

    public int StockAnterior { get; set; }

    public int StockPosterior { get; set; }

    [ValidateNever]
    public Producto Producto { get; set; } = null!;

    [ValidateNever]
    public PuntoVenta PuntoVenta { get; set; } = null!;

    [ValidateNever]
    public PuntoVenta? PuntoVentaDestino { get; set; }

    [ValidateNever]
    public Venta? Venta { get; set; }

    [ValidateNever]
    public ApplicationUser Usuario { get; set; } = null!;
}
