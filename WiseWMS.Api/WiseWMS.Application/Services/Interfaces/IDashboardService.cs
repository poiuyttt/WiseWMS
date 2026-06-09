using WiseWMS.Application.DTOs;

namespace WiseWMS.Application.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardDto> GetDashboard();
    }
}
