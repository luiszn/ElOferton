using ElOferton.ViewModels;

namespace ElOferton.Services.Interfaces;

public interface IVentaService
{
    Task<PosViewModel> PrepararPosAsync(int puntoVentaId, bool esAdministrador);

    Task<List<PosProductoResultado>> BuscarProductosAsync(string? termino, int puntoVentaId);

    Task<ResultadoVenta> RegistrarAsync(PosViewModel model, int usuarioId, bool esAdministrador);

    Task<VentaFiltroViewModel> ObtenerVentasAsync(VentaFiltroViewModel filtro);

    Task<VentaDetalleViewModel?> ObtenerDetalleAsync(int id);
}
