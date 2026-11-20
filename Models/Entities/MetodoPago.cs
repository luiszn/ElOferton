using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace ElOferton.Models.Entities;

public class MetodoPago
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Nombre { get; set; } = null!;

    public bool RequiereMontoRecibido { get; set; }

    public bool Activo { get; set; } = true;

    [ValidateNever]
    public ICollection<Venta> Ventas { get; set; } = [];
}
