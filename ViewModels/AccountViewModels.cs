using System.ComponentModel.DataAnnotations;

namespace ElOferton.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Ingrese su correo.")]
    [EmailAddress]
    [Display(Name = "Correo electrónico")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingrese su contraseña.")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Recordarme")]
    public bool Recordarme { get; set; }

    public string? ReturnUrl { get; set; }
}

public class CrearUsuarioViewModel
{
    [Required]
    [StringLength(150)]
    [Display(Name = "Nombre completo")]
    public string NombreCompleto { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [Display(Name = "Correo electrónico")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    [Display(Name = "Usuario")]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden.")]
    [Display(Name = "Confirmar contraseña")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Rol")]
    public string Rol { get; set; } = string.Empty;

    [Display(Name = "Punto de venta")]
    public int? PuntoVentaId { get; set; }

    public Microsoft.AspNetCore.Mvc.Rendering.SelectListItem[] Roles { get; set; } = [];

    public Microsoft.AspNetCore.Mvc.Rendering.SelectListItem[] PuntosVenta { get; set; } = [];
}

public class CambiarPasswordViewModel
{
    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña actual")]
    public string Actual { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 8)]
    [DataType(DataType.Password)]
    [Display(Name = "Nueva contraseña")]
    public string Nueva { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Compare(nameof(Nueva), ErrorMessage = "Las contraseñas no coinciden.")]
    [Display(Name = "Confirmar nueva contraseña")]
    public string Confirmar { get; set; } = string.Empty;
}

public class PerfilViewModel
{
    public string NombreCompleto { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Rol { get; set; } = string.Empty;

    public string PuntoVenta { get; set; } = string.Empty;
}
