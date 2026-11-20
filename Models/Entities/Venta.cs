using ElOferton.Constants;
using ElOferton.Models.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElOferton.Models.Entities;

public class Venta
{
    public int Id { get; set; }

    [Required]
    [StringLength(20)]
    public string NumeroComprobante { get; set; } = null!;

    public DateTime Fecha { get; set; } = DateTime.UtcNow;

    public int? ClienteId { get; set; }

    [StringLength(150)]
    public string? ClienteOcasional { get; set; }

    public int PuntoVentaId { get; set; }

    public int UsuarioId { get; set; }

    public int MetodoPagoId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Subtotal { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal PorcentajeDescuento { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Descuento { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Total { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? MontoRecibido { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? Vuelto { get; set; }

    public EstadoVenta Estado { get; set; } = EstadoVenta.Completada;

    [StringLength(300)]
    public string? Observaciones { get; set; }

    [ValidateNever]
    public Cliente? Cliente { get; set; }

    [ValidateNever]
    public PuntoVenta PuntoVenta { get; set; } = null!;

    [ValidateNever]
    public ApplicationUser Usuario { get; set; } = null!;

    [ValidateNever]
    public MetodoPago MetodoPago { get; set; } = null!;

    [ValidateNever]
    public ICollection<DetalleVenta> Detalles { get; set; } = [];
}
