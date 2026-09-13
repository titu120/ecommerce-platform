using Ecommerce.Application.DTOs.Dashboard;

namespace Ecommerce.Application.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardDto> GetDashboardAsync();
    }
}