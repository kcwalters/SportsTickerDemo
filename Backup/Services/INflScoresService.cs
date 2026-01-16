using NflTickerDemo.Models;
namespace NflTickerDemo.Services;
public interface INflScoresService
{
    Task<IReadOnlyList<NflTickerGame>> GetGamesAsync(CancellationToken cancellationToken = default);
}
