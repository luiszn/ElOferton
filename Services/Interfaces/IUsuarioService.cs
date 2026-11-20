using ElOferton.ViewModels;

namespace ElOferton.Services.Interfaces;

public interface IUsuarioService
{
    Task<UsuarioListViewModel> ObtenerUsuariosAsync();

    Task<CrearUsuarioViewModel> ObtenerFormularioCrearAsync();

    Task<ResultadoOperacion> CrearAsync(CrearUsuarioViewModel model);

    Task<ResultadoOperacion> AlternarEstadoAsync(int id);

    Task<ResultadoOperacion> CambiarRolAsync(int id, string rol);

    Task<PerfilViewModel?> ObtenerPerfilAsync(int id);
}
