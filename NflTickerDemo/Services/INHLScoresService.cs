using SportsTickerDemo.Models;

namespace SportsTickerDemo.Services
{

    public interface INHLScoresService
    {
        Task<IReadOnlyList<NHLTickerView>> GetGamesAsync(CancellationToken cancellationToken = default);
    }
}