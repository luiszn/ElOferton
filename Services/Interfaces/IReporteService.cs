using ElOferton.ViewModels;

namespace ElOferton.Services.Interfaces;

public interface IReporteService
{
    Task<ReporteFiltroViewModel> ObtenerAsync(ReporteFiltroViewModel filtro);
}
