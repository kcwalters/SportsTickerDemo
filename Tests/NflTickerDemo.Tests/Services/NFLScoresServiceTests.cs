using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SportsTickerDemo.Models;
using SportsTickerDemo.Services;
using SportsTickerDemo.Tests.Helpers;
using Xunit;

namespace SportsTickerDemo.Tests.Services
{
    public class NFLScoresServiceTests
    {
        private static (NFLScoresService service, IMemoryCache cache) CreateService(string json)
        {
            var httpClient = FakeHttpMessageHandler.CreateClient(json);
            var logger = Mock.Of<ILogger<NFLScoresService>>();
            var cache = new MemoryCache(new MemoryCacheOptions());
            var opts = Options.Create(new SportsTickerOptions());
            var service = new NFLScoresService(httpClient, logger, cache, opts);
            return (service, cache);
        }

        private const string SampleJson = "{""events"": [{""id"": ""401547711"", ""competitions"": [{""status"": {""type"": {""state"": ""in"", ""shortDetail"": ""Q2 05:32""}}, ""competitors"": [{""homeAway"": ""away"", ""score"": ""10"", ""team"": {""abbreviation"": ""BUF"", ""logos"": [{""href"": ""http://logo.com/buf.png""}]}}, {""homeAway"": ""home"", ""score"": ""14"", ""team"": {""abbreviation"": ""KC"", ""logo"": ""http://logo.com/kc.png""}}]}]}]}";

        [Fact]
        public async Task GetGamesAsync_CachesResult()
        {
            var (service, cache) = CreateService(SampleJson);
            var first = await service.GetGamesAsync();
            var second = await service.GetGamesAsync();
            Assert.Same(first, second);
            Assert.NotEmpty(first);
        }

        [Fact]
        public async Task GetGamesAsync_ParsesEvent()
        {
            var (service, _) = CreateService(SampleJson);
            var result = await service.GetGamesAsync();
            Assert.Single(result);
            var g = result[0];
            Assert.Equal("BUF", g.AwayTeam);
            Assert.Equal("KC", g.HomeTeam);
            Assert.Equal(10, g.AwayScore);
            Assert.Equal(14, g.HomeScore);
            Assert.True(g.IsLive);
            Assert.False(g.IsFinal);
            Assert.Equal("Q2 05:32", g.StatusText);
            Assert.Equal("401547711", g.EventId);
            Assert.Equal("http://logo.com/buf.png", g.AwayLogo);
            Assert.Equal("http://logo.com/kc.png", g.HomeLogo);
        }

        [Fact]
        public async Task GetGamesAsync_MissingEvents_ReturnsEmptyAndCaches()
        {
            var (service, cache) = CreateService("{}\n");
            var result = await service.GetGamesAsync();
            Assert.Empty(result);
            // call again ensures cached empty
            var result2 = await service.GetGamesAsync();
            Assert.Same(result, result2);
        }

        [Fact]
        public async Task GetGamesAsync_OnHttpError_ReturnsEmptyAndCaches()
        {
            var httpClient = FakeHttpMessageHandler.CreateClient("bad", HttpStatusCode.InternalServerError);
            var logger = Mock.Of<ILogger<NFLScoresService>>();
            var cache = new MemoryCache(new MemoryCacheOptions());
            var opts = Options.Create(new SportsTickerOptions());
            var service = new NFLScoresService(httpClient, logger, cache, opts);

            var result = await service.GetGamesAsync();
            Assert.Empty(result);
            var result2 = await service.GetGamesAsync();
            Assert.Same(result, result2);
        }

        [Theory]
        [InlineData("{\"team\":{\"abbreviation\":\"NYJ\"}}", "nyj")]
        public void GetLogoUrl_Fallbacks_ToLocalSvg(string teamJson, string expectedLower)
        {
            using var doc = JsonDocument.Parse(teamJson);
            var teamNode = doc.RootElement.GetProperty("team");
            var url = InvokeGetLogoUrl(teamNode, "NYJ");
            Assert.Equal($"/img/nfl/{expectedLower}.svg", url);
        }

        private static string InvokeGetLogoUrl(JsonElement teamNode, string abbr)
        {
            var mi = typeof(NFLScoresService).GetMethod("GetLogoUrl", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            return (string)mi!.Invoke(null, new object[] { teamNode, abbr });
        }
    }
}