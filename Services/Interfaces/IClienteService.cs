using ElOferton.ViewModels;

namespace ElOferton.Services.Interfaces;

public interface IClienteService
{
    Task<ClienteListViewModel> ObtenerListaAsync(string? buscar);

    Task<ClienteFormViewModel?> ObtenerFormularioAsync(int? id);

    Task<ClienteDetalleViewModel?> ObtenerDetalleAsync(int id);

    Task<ResultadoOperacion> GuardarAsync(ClienteFormViewModel model);

    Task<ResultadoOperacion> AlternarEstadoAsync(int id);
}
