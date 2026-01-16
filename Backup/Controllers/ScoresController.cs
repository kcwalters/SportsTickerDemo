using Microsoft.AspNetCore.Mvc;
using NflTickerDemo.Services;
namespace NflTickerDemo.Controllers;
public class ScoresController : Controller
{
    private readonly INflScoresService _scoresService;
    public ScoresController(INflScoresService scoresService)
    {
        _scoresService = scoresService;
    }
    [HttpGet]
    public async Task<IActionResult> NflTickerInner()
    {
        var games = await _scoresService.GetGamesAsync();
        return PartialView(""_NflTickerInner"", games);
    }
}
