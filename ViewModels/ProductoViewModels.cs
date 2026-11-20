using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ElOferton.ViewModels;

public class ProductoListViewModel
{
    public List<ProductoItemViewModel> Productos { get; set; } = [];

    public string? Buscar { get; set; }

    public int? CategoriaId { get; set; }

    public int? MarcaId { get; set; }

    public bool? SoloActivos { get; set; } = true;

    public SelectListItem[] Categorias { get; set; } = [];

    public SelectListItem[] Marcas { get; set; } = [];
}

public class ProductoItemViewModel
{
    public int Id { get; set; }

    public string Codigo { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public int CategoriaId { get; set; }

    public string Categoria { get; set; } = string.Empty;

    public int MarcaId { get; set; }

    public string Marca { get; set; } = string.Empty;

    public decimal PrecioCompra { get; set; }

    public decimal PrecioVenta { get; set; }

    public int StockTotal { get; set; }

    public int StockMinimo { get; set; }

    public bool Activo { get; set; }

    public string? ImagenUrl { get; set; }
}

public class ProductoFormViewModel
{
    public int? Id { get; set; }

    [Required]
    [StringLength(20)]
    [Display(Name = "Código")]
    public string Codigo { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(500)]
    [Display(Name = "Descripción")]
    public string? Descripcion { get; set; }

    [Required]
    [Display(Name = "Categoría")]
    public int CategoriaId { get; set; }

    [Required]
    [Display(Name = "Marca")]
    public int MarcaId { get; set; }

    [Required]
    [Range(0, 1000000)]
    [Display(Name = "Precio de compra")]
    public decimal PrecioCompra { get; set; }

    [Required]
    [Range(0, 1000000)]
    [Display(Name = "Precio de venta")]
    public decimal PrecioVenta { get; set; }

    [Required]
    [Range(0, 100000)]
    [Display(Name = "Stock mínimo")]
    public int StockMinimo { get; set; }

    [Display(Name = "Activo")]
    public bool Activo { get; set; } = true;

    [StringLength(250)]
    [Display(Name = "Imagen (URL)")]
    public string? ImagenUrl { get; set; }

    public SelectListItem[] Categorias { get; set; } = [];

    public SelectListItem[] Marcas { get; set; } = [];
}

public class ProductoDetalleViewModel
{
    public ProductoItemViewModel Producto { get; set; } = new();

    public List<StockAmbienteViewModel> Stocks { get; set; } = [];
}

public class StockAmbienteViewModel
{
    public int PuntoVentaId { get; set; }

    public string Ambiente { get; set; } = string.Empty;

    public string Tipo { get; set; } = string.Empty;

    public int Cantidad { get; set; }
}
