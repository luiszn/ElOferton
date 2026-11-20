using System.ComponentModel.DataAnnotations;

namespace ElOferton.ViewModels;

public class CatalogoListViewModel
{
    public List<CatalogoItemViewModel> Items { get; set; } = [];

    public string Titulo { get; set; } = string.Empty;

    public string Controlador { get; set; } = string.Empty;

    public string? Buscar { get; set; }
}

public class CatalogoItemViewModel
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public bool Activo { get; set; }

    public int CantidadRelacionada { get; set; }
}

public class CatalogoFormViewModel
{
    public int? Id { get; set; }

    [Required]
    [StringLength(80)]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(300)]
    public string? Descripcion { get; set; }

    public bool Activo { get; set; } = true;

    public string Titulo { get; set; } = string.Empty;

    public string Controlador { get; set; } = string.Empty;
}
