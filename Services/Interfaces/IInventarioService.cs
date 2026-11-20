using ElOferton.ViewModels;

namespace ElOferton.Services.Interfaces;

public interface IInventarioService
{
    Task<InventarioFiltroViewModel> ObtenerInventarioAsync(string? buscar, int? puntoVentaId, bool? soloStockBajo);

    Task<MovimientoFormViewModel> ObtenerFormularioMovimientoAsync();

    Task<MovimientoFiltroViewModel> ObtenerMovimientosAsync(MovimientoFiltroViewModel filtro);

    Task<ResultadoOperacion> RegistrarMovimientoAsync(MovimientoFormViewModel model, int usuarioId);

    Task<int> ObtenerStockAsync(int productoId, int puntoVentaId);
}
