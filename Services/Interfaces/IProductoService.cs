using ElOferton.ViewModels;

namespace ElOferton.Services.Interfaces;

public interface IProductoService
{
    Task<ProductoListViewModel> ObtenerListaAsync(string? buscar, int? categoriaId, int? marcaId, bool? soloActivos);

    Task<ProductoFormViewModel?> ObtenerFormularioAsync(int? id);

    Task<ProductoDetalleViewModel?> ObtenerDetalleAsync(int id);

    Task<ResultadoOperacion> GuardarAsync(ProductoFormViewModel model);

    Task<ResultadoOperacion> AlternarEstadoAsync(int id);

    Task<List<PosProductoResultado>> BuscarParaPosAsync(string? termino, int puntoVentaId);
}
