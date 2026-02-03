using Microsoft.AspNetCore.Mvc;
using InfrastructureApp.Models;
using InfrastructureApp.Services;
using InfrastructureApp.ViewModels;

namespace InfrastructureApp.Controllers;

public class LeaderboardController : Controller
{
    private readonly LeaderboardService _service;

    public LeaderboardController(LeaderboardService service)
    {
        _service = service;
    }

    [HttpGet("/Leaderboard")]
    public async Task<IActionResult> Index(int top = 25)
    {
        var entries = await _service.GetTopAsync(top);

        var vm = new LeaderboardIndexViewModel
        {
            Entries = entries,
            TopN = top <= 0 ? 25 : top
        };

        return View(vm);
    }

    [HttpPost("/Leaderboard/Add")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(AddPointsRequest form, int top = 25)
    {
        if (!ModelState.IsValid)
        {
            var entries = await _service.GetTopAsync(top);
            return View("Index", new LeaderboardIndexViewModel
            {
                Entries = entries,
                Form = form,
                ErrorMessage = "Please fix the validation errors and try again.",
                TopN = top <= 0 ?25 : top

            });
            
        }

        try
        {
            await _service.AddPointsAsync(form.DisplayName!, form.Points);
            return RedirectToAction(nameof(Index), new { top });
        }
        catch (ArgumentException ex)
        {
            var entries = await _service.GetTopAsync(top);
            return View("Index", new LeaderboardIndexViewModel
            {
                Entries = entries,
                Form = form,
                ErrorMessage = ex.Message,
                TopN = top <= 0 ? 25 : top
            });
        }
    }
}