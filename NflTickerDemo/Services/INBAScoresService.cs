using SportsTickerDemo.Models;

namespace SportsTickerDemo.Services
{

    public interface INBAScoresService
    {
        Task<IReadOnlyList<NBATickerGame>> GetGamesAsync(CancellationToken cancellationToken = default);
    }
}