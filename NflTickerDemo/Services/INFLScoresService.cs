using SportsTickerDemo.Models;

namespace SportsTickerDemo.Services
{

    public interface INFLScoresService
    {
        Task<IReadOnlyList<NFLTickerView>> GetGamesAsync(CancellationToken cancellationToken = default);
    }
}