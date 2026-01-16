using SportsTickerDemo.Models;
namespace SportsTickerDemo.Services
{

    public interface INFLScoresService
    {
        Task<IReadOnlyList<NFLTickerGame>> GetGamesAsync(CancellationToken cancellationToken = default);
    }
}