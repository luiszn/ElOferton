using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElOferton.Models.Entities;

public class Producto
{
    public int Id { get; set; }

    [Required]
    [StringLength(20)]
    public string Codigo { get; set; } = null!;

    [Required]
    [StringLength(150)]
    public string Nombre { get; set; } = null!;

    [StringLength(500)]
    public string? Descripcion { get; set; }

    public int CategoriaId { get; set; }

    public int MarcaId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal PrecioCompra { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal PrecioVenta { get; set; }

    [Range(0, int.MaxValue)]
    public int StockMinimo { get; set; }

    public bool Activo { get; set; } = true;

    [StringLength(250)]
    public string? ImagenUrl { get; set; }

    [Timestamp]
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    [ValidateNever]
    public Categoria Categoria { get; set; } = null!;

    [ValidateNever]
    public Marca Marca { get; set; } = null!;

    [ValidateNever]
    public ICollection<Inventario> Inventarios { get; set; } = [];

    [ValidateNever]
    public ICollection<DetalleVenta> DetallesVenta { get; set; } = [];

    [ValidateNever]
    public ICollection<MovimientoInventario> Movimientos { get; set; } = [];
}
