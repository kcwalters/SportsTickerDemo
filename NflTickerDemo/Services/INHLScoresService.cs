using SportsTickerDemo.Models;

namespace SportsTickerDemo.Services
{

    public interface INHLScoresService
    {
        Task<IReadOnlyList<NHLTickerGame>> GetGamesAsync(CancellationToken cancellationToken = default);
    }
}