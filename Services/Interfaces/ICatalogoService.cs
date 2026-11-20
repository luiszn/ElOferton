using ElOferton.ViewModels;

namespace ElOferton.Services.Interfaces;

public interface ICatalogoService
{
    Task<CatalogoListViewModel> ObtenerCategoriasAsync(string? buscar);

    Task<CatalogoFormViewModel?> ObtenerCategoriaFormularioAsync(int? id);

    Task<ResultadoOperacion> GuardarCategoriaAsync(CatalogoFormViewModel model);

    Task<ResultadoOperacion> AlternarCategoriaAsync(int id);

    Task<CatalogoListViewModel> ObtenerMarcasAsync(string? buscar);

    Task<CatalogoFormViewModel?> ObtenerMarcaFormularioAsync(int? id);

    Task<ResultadoOperacion> GuardarMarcaAsync(CatalogoFormViewModel model);

    Task<ResultadoOperacion> AlternarMarcaAsync(int id);
}
