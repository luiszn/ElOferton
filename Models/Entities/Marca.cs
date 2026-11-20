using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace ElOferton.Models.Entities;

public class Marca
{
    public int Id { get; set; }

    [Required]
    [StringLength(80)]
    public string Nombre { get; set; } = null!;

    [StringLength(300)]
    public string? Descripcion { get; set; }

    public bool Activo { get; set; } = true;

    [ValidateNever]
    public ICollection<Producto> Productos { get; set; } = [];
}
