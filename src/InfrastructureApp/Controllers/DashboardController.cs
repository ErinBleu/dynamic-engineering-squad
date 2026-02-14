using InfrastructureApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace InfrastructureApp.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboardRepository _dashboardRepo;

        public DashboardController(IDashboardRepository dashboardRepo)
        {
            _dashboardRepo = dashboardRepo;
        }

        public async Task<IActionResult> Index()
        {
            var vm = await _dashboardRepo.GetDashboardSummaryAsync();
            return View(vm);
        }
    }
}