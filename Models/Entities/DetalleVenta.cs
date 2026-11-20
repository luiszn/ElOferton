using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElOferton.Models.Entities;

public class DetalleVenta
{
    public int Id { get; set; }

    public int VentaId { get; set; }

    public int ProductoId { get; set; }

    [Required]
    [StringLength(20)]
    public string CodigoProducto { get; set; } = null!;

    [Required]
    [StringLength(150)]
    public string NombreProducto { get; set; } = null!;

    public int Cantidad { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal PrecioUnitario { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Subtotal { get; set; }

    [ValidateNever]
    public Venta Venta { get; set; } = null!;

    [ValidateNever]
    public Producto Producto { get; set; } = null!;
}
