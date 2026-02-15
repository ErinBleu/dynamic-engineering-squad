/* Interface for ReportIssueRepository.
Defines the operations the service layer can perform on reports 
without exposing EF Core or database details*/

using InfrastructureApp.Models;

namespace InfrastructureApp.Repositories
{
    public interface IReportIssueRepository
    {
        //asynchronously retrieves a report by its ID
        Task<ReportIssue?> GetByIdAsync(int id);

        //adds a new report to the database context
        Task AddAsync(ReportIssue report);

        //saves changes to the dabatase
        Task SaveChangesAsync();
    }
}
