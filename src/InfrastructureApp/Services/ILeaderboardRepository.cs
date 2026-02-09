using InfrastructureApp.Models;

namespace InfrastructureApp.Services;

//ILeaderboardRepository defines the contract for leaderboard data access.

//This interface sits at the repo layer and abstracts:
//-Where leaderboard data is stored (SQL)
//-How entries are queried or updated

//By coding against this interface, the application remains:
//-Testable (it can be mocked or stubbed)
//-Loosely coupled to persistence tech
//-Open to future storage changes without impacting services or controllers

public interface ILeaderboardRepository
{

    //Retrieves all leaderboard entries from the data store

    //Returns a read only collection to prevent consumers from
    //accidentally mutating repo-owned data
    //Async ensures scalability when backed by a db
    Task<IReadOnlyCollection<LeaderboardEntry>> GetAllAsync();

    //Parameters:
    //-displayName
    //-pointsToAdd
    //-updated UTC
    Task UpsertAddPointsAsync(string displayName, int pointsToAdd, System.DateTime updatedAtUtc);
    //Seeds the leaderboard with initial data if the underlying store is empty
    //Good for Dev, Tests, and first runs

    //Must ensure that avoids overwriting real or prod data.
    Task SeedIfEmptyAsync(IEnumerable<LeaderboardEntry> seedEntries);
}