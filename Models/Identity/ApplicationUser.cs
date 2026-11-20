using ElOferton.Models.Entities;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElOferton.Models.Identity;

public class ApplicationUser : IdentityUser<int>
{
    [Required]
    [StringLength(150)]
    public string NombreCompleto { get; set; } = null!;

    public int? PuntoVentaId { get; set; }

    public bool Activo { get; set; } = true;

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(PuntoVentaId))]
    public PuntoVenta? PuntoVenta { get; set; }
}
