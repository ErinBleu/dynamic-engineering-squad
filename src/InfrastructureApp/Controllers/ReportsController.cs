using InfrastructureApp.Data;
using InfrastructureApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureApp.Controllers
{
    // Controller handles requests related to reports pages
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _db; // Database (EF Core) access

        // Constructor injects the database to query data
        public ReportsController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: /Reports/Latest
        // Shows the Latest Reports page
        [HttpGet]
        public async Task<IActionResult> Latest()
        {
            
            // Use existing database reports (no seeding needed) and map them to ViewModels
            var items = await _db.ReportIssue
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new LatestReportItemViewModel
                {
                    Id = r.Id,
                    Description = r.Description,
                    Status = r.Status,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();

            // Put list into page ViewModel
            var vm = new LatestReportsViewModel
            {
                Reports = items
            };

            // Send data to the view (Razor page)
            return View(vm);
        }
    }
}
