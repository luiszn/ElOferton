using ElOferton.ViewModels;

namespace ElOferton.Services.Interfaces;

public interface IDashboardService
{
    Task<DashboardViewModel> ObtenerAsync();
}
