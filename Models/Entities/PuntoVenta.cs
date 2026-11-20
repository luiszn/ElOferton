using ElOferton.Constants;
using ElOferton.Models.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace ElOferton.Models.Entities;

public class PuntoVenta
{
    public int Id { get; set; }

    [Required]
    [StringLength(10)]
    public string Codigo { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string Nombre { get; set; } = null!;

    public TipoAmbiente Tipo { get; set; }

    [StringLength(200)]
    public string? Direccion { get; set; }

    public bool Activo { get; set; } = true;

    [ValidateNever]
    public ICollection<ApplicationUser> Usuarios { get; set; } = [];

    [ValidateNever]
    public ICollection<Inventario> Inventarios { get; set; } = [];

    [ValidateNever]
    public ICollection<Venta> Ventas { get; set; } = [];

    [ValidateNever]
    public ICollection<MovimientoInventario> MovimientosOrigen { get; set; } = [];

    [ValidateNever]
    public ICollection<MovimientoInventario> MovimientosDestino { get; set; } = [];
}
