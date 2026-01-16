using System.Text.Json;
using NflTickerDemo.Models;
namespace NflTickerDemo.Services;
public class NflScoresService : INflScoresService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<NflScoresService> _logger;
    private const string ScoreboardUrl =
        ""https://site.api.espn.com/apis/site/v2/sports/football/nfl/scoreboard"";
    public NflScoresService(HttpClient httpClient, ILogger<NflScoresService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
    public async Task<IReadOnlyList<NflTickerGame>> GetGamesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var response = await _httpClient.GetAsync(ScoreboardUrl, cancellationToken);
            response.EnsureSuccessStatusCode();
            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
            var root = doc.RootElement;
            if (!root.TryGetProperty(""events"", out var eventsElement) ||
                eventsElement.ValueKind != JsonValueKind.Array)
            {
                _logger.LogWarning(""ESPN scoreboard: 'events' array missing or invalid."");
                return Array.Empty<NflTickerGame>();
            }
            var games = new List<NflTickerGame>();
            foreach (var ev in eventsElement.EnumerateArray())
            {
                var game = MapEventToTickerGame(ev);
                if (game != null)
                {
                    games.Add(game);
                }
            }
            return games;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ""Error fetching NFL scoreboard from ESPN."");
            return Array.Empty<NflTickerGame>();
        }
    }
    private static NflTickerGame? MapEventToTickerGame(JsonElement ev)
    {
        if (!ev.TryGetProperty(""competitions"", out var compsElement) ||
            compsElement.ValueKind != JsonValueKind.Array ||
            compsElement.GetArrayLength() == 0)
        {
            return null;
        }
        var comp = compsElement[0];
        string state = """";
        string shortDetail = """";
        if (comp.TryGetProperty(""status"", out var statusElement) &&
            statusElement.TryGetProperty(""type"", out var typeElement))
        {
            state = typeElement.GetProperty(""state"").GetString() ?? """";
            shortDetail = typeElement.GetProperty(""shortDetail"").GetString() ?? """";
        }
        string awayTeam = """";
        string homeTeam = """";
        int? awayScore = null;
        int? homeScore = null;
        if (comp.TryGetProperty(""competitors"", out var competitorsElement) &&
            competitorsElement.ValueKind == JsonValueKind.Array)
        {
            foreach (var teamElement in competitorsElement.EnumerateArray())
            {
                var homeAway = teamElement.GetProperty(""homeAway"").GetString();
                var teamAbbr = teamElement
                    .GetProperty(""team"")
                    .GetProperty(""abbreviation"")
                    .GetString() ?? """";
                int? score = null;
                if (teamElement.TryGetProperty(""score"", out var scoreElement) &&
                    scoreElement.ValueKind == JsonValueKind.String &&
                    int.TryParse(scoreElement.GetString(), out var parsedScore))
                {
                    score = parsedScore;
                }
                if (homeAway == ""away"")
                {
                    awayTeam = teamAbbr;
                    awayScore = score;
                }
                else
                {
                    homeTeam = teamAbbr;
                    homeScore = score;
                }
            }
        }
        string? eventId = ev.TryGetProperty(""id"", out var idElement)
            ? idElement.GetString()
            : null;
        bool isLive = state == ""in"";
        bool isFinal = state == ""post"";
        var statusText = !string.IsNullOrWhiteSpace(shortDetail)
            ? shortDetail
            : state switch
            {
                ""pre"" => ""Scheduled"",
                ""in"" => ""Live"",
                ""post"" => ""Final"",
                _ => state
            };
        return new NflTickerGame
        {
            AwayTeam = awayTeam,
            HomeTeam = homeTeam,
            AwayScore = awayScore,
            HomeScore = homeScore,
            StatusText = statusText,
            IsLive = isLive,
            IsFinal = isFinal,
            EventId = eventId
        };
    }
}
