//Defines the function listed in IReportIssueRepository

using InfrastructureApp.Data;
using InfrastructureApp.Models;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureApp.Repositories
{
    public class ReportIssueRepositoryEf : IReportIssueRepository
    {
        private readonly ApplicationDbContext _db;

        public ReportIssueRepositoryEf(ApplicationDbContext db)
        {
            _db = db;
        }

        //queries the reports table and returns report if found
        public Task<ReportIssue?> GetByIdAsync(int id)
        {
            return _db.ReportIssue.FirstOrDefaultAsync(r => r.Id == id);
        }

        //adds a new report report to EF's change tracker
        public async Task AddAsync(ReportIssue report)
        {
            _db.ReportIssue.Add(report);
            await Task.CompletedTask;
        }

        //saves changes to the database, inserts reports row into the reports table
        public Task SaveChangesAsync()
        {
            return _db.SaveChangesAsync();
        }
    }
}
