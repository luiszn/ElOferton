using ElOferton.Constants;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace ElOferton.Models.Entities;

public class Cliente
{
    public int Id { get; set; }

    public TipoDocumento TipoDocumento { get; set; } = TipoDocumento.DNI;

    [StringLength(20)]
    public string? NumeroDocumento { get; set; }

    [Required]
    [StringLength(150)]
    public string Nombre { get; set; } = null!;

    [StringLength(20)]
    [Phone]
    public string? Telefono { get; set; }

    [StringLength(120)]
    [EmailAddress]
    public string? Email { get; set; }

    [StringLength(250)]
    public string? Direccion { get; set; }

    public bool Activo { get; set; } = true;

    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    [ValidateNever]
    public ICollection<Venta> Ventas { get; set; } = [];
}
