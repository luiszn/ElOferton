using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ElOferton.ViewModels;

public class UsuarioListViewModel
{
    public List<UsuarioItemViewModel> Usuarios { get; set; } = [];
}

public class UsuarioItemViewModel
{
    public int Id { get; set; }

    public string NombreCompleto { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public string Rol { get; set; } = string.Empty;

    public string PuntoVenta { get; set; } = string.Empty;

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }
}

public class CambiarRolViewModel
{
    public int UsuarioId { get; set; }

    [Required]
    public string Rol { get; set; } = string.Empty;

    public SelectListItem[] Roles { get; set; } = [];
}
