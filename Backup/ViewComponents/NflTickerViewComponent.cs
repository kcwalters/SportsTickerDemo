using Microsoft.AspNetCore.Mvc;
using NflTickerDemo.Services;
namespace NflTickerDemo.ViewComponents;
public class NflTickerViewComponent : ViewComponent
{
    private readonly INflScoresService _scoresService;
    public NflTickerViewComponent(INflScoresService scoresService)
    {
        _scoresService = scoresService;
    }
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var games = await _scoresService.GetGamesAsync();
        return View(games);
    }
}
