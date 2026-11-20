using ElOferton.Constants;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ElOferton.ViewModels;

public class ClienteListViewModel
{
    public List<ClienteItemViewModel> Clientes { get; set; } = [];

    public string? Buscar { get; set; }
}

public class ClienteItemViewModel
{
    public int Id { get; set; }

    public string TipoDocumento { get; set; } = string.Empty;

    public string? NumeroDocumento { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string? Telefono { get; set; }

    public string? Email { get; set; }

    public bool Activo { get; set; }

    public int CantidadCompras { get; set; }

    public decimal TotalCompras { get; set; }
}

public class ClienteFormViewModel
{
    public int? Id { get; set; }

    [Display(Name = "Tipo de documento")]
    public TipoDocumento TipoDocumento { get; set; } = TipoDocumento.DNI;

    [StringLength(20)]
    [Display(Name = "Número de documento")]
    public string? NumeroDocumento { get; set; }

    [Required]
    [StringLength(150)]
    [Display(Name = "Nombres / razón social")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(20)]
    [Phone]
    public string? Telefono { get; set; }

    [StringLength(120)]
    [EmailAddress]
    public string? Email { get; set; }

    [StringLength(250)]
    public string? Direccion { get; set; }

    public bool Activo { get; set; } = true;
}

public class ClienteDetalleViewModel
{
    public ClienteItemViewModel Cliente { get; set; } = new();

    public string? Direccion { get; set; }

    public List<CompraClienteViewModel> Compras { get; set; } = [];
}

public class CompraClienteViewModel
{
    public int VentaId { get; set; }

    public string NumeroComprobante { get; set; } = string.Empty;

    public DateTime Fecha { get; set; }

    public string PuntoVenta { get; set; } = string.Empty;

    public decimal Total { get; set; }
}
