using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SportsTickerDemo.Services;
using Xunit;

public class NBAScoresServiceTests
{
    private sealed class FakeHandler : HttpMessageHandler
    {
        private readonly HttpResponseMessage _response;
        public FakeHandler(HttpResponseMessage response) => _response = response;
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => Task.FromResult(_response);
    }

    [Fact]
    public async Task GetGamesAsync_ReturnsEmpty_OnNonEvents()
    {
        var json = "{ \"events\": [] }";
        var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json) };
        var client = new HttpClient(new FakeHandler(response));
        var logger = Mock.Of<ILogger<NBAScoresService>>();
        var cache = new MemoryCache(new MemoryCacheOptions());
        var opts = Options.Create(new SportsTickerOptions());

        var svc = new NBAScoresService(client, logger, cache, opts);
        var result = await svc.GetGamesAsync();

        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
