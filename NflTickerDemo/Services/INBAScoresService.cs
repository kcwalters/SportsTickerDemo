using SportsTickerDemo.Models;

namespace SportsTickerDemo.Services
{

    public interface INBAScoresService
    {
        Task<IReadOnlyList<NBATickerView>> GetGamesAsync(CancellationToken cancellationToken = default);
    }
}