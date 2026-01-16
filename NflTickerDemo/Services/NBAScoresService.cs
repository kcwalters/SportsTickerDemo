using System.Text.Json;
using Microsoft.Extensions.Logging;
using SportsTickerDemo.Models;

namespace SportsTickerDemo.Services
{
    public class NBAScoresService : INBAScoresService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<NBAScoresService> _logger;

        private const string ScoreboardUrl =
            "https://site.api.espn.com/apis/site/v2/sports/basketball/nba/scoreboard";

        public NBAScoresService(HttpClient httpClient, ILogger<NBAScoresService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IReadOnlyList<NBATickerGame>> GetGamesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                using var response = await _httpClient.GetAsync(ScoreboardUrl, cancellationToken);
                response.EnsureSuccessStatusCode();

                await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
                var root = doc.RootElement;

                if (!root.TryGetProperty("events", out var eventsElement) ||
                    eventsElement.ValueKind != JsonValueKind.Array)
                {
                    _logger.LogWarning("ESPN NBA scoreboard: 'events' array missing or invalid.");
                    return Array.Empty<NBATickerGame>();
                }

                var games = new List<NBATickerGame>();
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
                _logger.LogError(ex, "Error fetching NBA scoreboard from ESPN.");
                return Array.Empty<NBATickerGame>();
            }
        }

        private static NBATickerGame? MapEventToTickerGame(JsonElement ev)
        {
            if (!ev.TryGetProperty("competitions", out var compsElement) ||
                compsElement.ValueKind != JsonValueKind.Array ||
                compsElement.GetArrayLength() == 0)
            {
                return null;
            }

            var comp = compsElement[0];

            string state = "";
            string shortDetail = "";
            if (comp.TryGetProperty("status", out var statusElement) &&
                statusElement.TryGetProperty("type", out var typeElement))
            {
                state = typeElement.GetProperty("state").GetString() ?? "";
                shortDetail = typeElement.GetProperty("shortDetail").GetString() ?? "";
            }

            string awayTeam = "";
            string homeTeam = "";
            int? awayScore = null;
            int? homeScore = null;
            string awayLogo = "";
            string homeLogo = "";

            if (comp.TryGetProperty("competitors", out var competitorsElement) &&
                competitorsElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var teamElement in competitorsElement.EnumerateArray())
                {
                    var homeAway = teamElement.GetProperty("homeAway").GetString();
                    var teamNode = teamElement.GetProperty("team");

                    var teamAbbr = teamNode.GetProperty("abbreviation").GetString() ?? "";
                    var logoUrl = GetLogoUrl(teamNode, teamAbbr);

                    int? score = null;
                    if (teamElement.TryGetProperty("score", out var scoreElement) &&
                        scoreElement.ValueKind == JsonValueKind.String &&
                        int.TryParse(scoreElement.GetString(), out var parsedScore))
                    {
                        score = parsedScore;
                    }

                    if (homeAway == "away")
                    {
                        awayTeam = teamAbbr;
                        awayScore = score;
                        awayLogo = logoUrl;
                    }
                    else
                    {
                        homeTeam = teamAbbr;
                        homeScore = score;
                        homeLogo = logoUrl;
                    }
                }
            }

            string? eventId = ev.TryGetProperty("id", out var idElement)
                ? idElement.GetString()
                : null;

            bool isLive = state == "in";
            bool isFinal = state == "post";

            var statusText = !string.IsNullOrWhiteSpace(shortDetail)
                ? shortDetail
                : state switch
                {
                    "pre" => "Scheduled",
                    "in" => "Live",
                    "post" => "Final",
                    _ => state
                };

            return new NBATickerGame
            {
                AwayTeam = awayTeam,
                HomeTeam = homeTeam,
                AwayScore = awayScore,
                HomeScore = homeScore,
                AwayLogo = awayLogo,
                HomeLogo = homeLogo,
                StatusText = statusText,
                IsLive = isLive,
                IsFinal = isFinal,
                EventId = eventId
            };
        }

        private static string GetLogoUrl(JsonElement teamNode, string teamAbbr)
        {
            if (teamNode.TryGetProperty("logos", out var logosElement) &&
                logosElement.ValueKind == JsonValueKind.Array &&
                logosElement.GetArrayLength() > 0)
            {
                var href = logosElement[0].GetProperty("href").GetString();
                if (!string.IsNullOrWhiteSpace(href))
                {
                    return href!;
                }
            }

            if (teamNode.TryGetProperty("logo", out var logoElement) &&
                logoElement.ValueKind == JsonValueKind.String)
            {
                var href = logoElement.GetString();
                if (!string.IsNullOrWhiteSpace(href))
                {
                    return href!;
                }
            }

            return $"/images/nba/{teamAbbr.ToLowerInvariant()}.svg";
        }
    }
}