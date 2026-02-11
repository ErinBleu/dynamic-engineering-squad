using InfrastructureApp.Models;

namespace InfrastructureApp.Services;

//LeaderboardService contains the business logic for the leaderboard operations.
//It will
//-enforce domain rules
//-coordinate with the repository for data access
//-Shield controllers from persistence and ordering details.


//Keeping this logic here (instead of controllers or repos)
//results in a clean separation of concerns and testable behavior.

public class LeaderboardService
{

    //Repo abstraction used for leaderboard persistence.
    //Doesn't care how data is stored
    private readonly ILeaderboardRepository _repo;


    //Repo is inhected via dependency injection
    //This enables mocking in unit tests and swapping implementations.
    public LeaderboardService(ILeaderboardRepository repo)
    {
        _repo = repo;
    }


    //Returns top 25 entries
    //Deterministic sorting to avoid ties or random order
    public async Task<IReadOnlyList<LeaderboardEntry>> GetTopAsync(int n = 25)
    {

        //If an invalid or nagative value is supplied, fall back to the default
        if (n <= 0) n = 25;

        //Retrieve all leaderboard entries from the repo.
        //Ordering and limiting handled in the service layer
        var all = await _repo.GetAllAsync();

        //Sort rules:
        //1. Userpoints desc
        //2. UserId asc
        //3. updatedAt desc
        var ordered = all
            .OrderByDescending(e => e.UserPoints)
            .ThenBy(e => e.UserId)
            .ThenByDescending(e => e.UpdatedAtUtc)

            //Limit results
            .Take(n)

            //Make a sequence to avoid deferred execution
            .ToList()

            //Expose results as read only to protect invariants
            .AsReadOnly();
        return ordered;    
    }


    /*Method enforces all business validation before delegating persistence to the repo
    public async Task AddPointsAsync(string displayName, int pointsToAdd)
    {

        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentException("DisplayName is required.", nameof(displayName));

        var name = displayName.Trim();
        if (name.Length > 64)
            throw new ArgumentException("DisplayName must be 64 characters or fewer", nameof(displayName));

        if (pointsToAdd <=0)
            throw new ArgumentException("Points must be greater than 0.", nameof(pointsToAdd));    

        //Points cannot be negative: enforced by requiring pointsToAdd > 0
        await _repo.UpsertAddPointsAsync(name, pointsToAdd, DateTime.UtcNow);

    }

    public Task SeedIfEmptyAsync(IEnumerable<LeaderboardEntry> seedEntries)
        => _repo.SeedIfEmptyAsync(seedEntries);*/
}