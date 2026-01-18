using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SportsTickerDemo.Tests.Helpers
{
    public class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpResponseMessage _response;
        public FakeHttpMessageHandler(HttpResponseMessage response)
        {
            _response = response;
        }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_response);
        }
        public static HttpClient CreateClient(string content, HttpStatusCode statusCode = HttpStatusCode.OK, string mediaType = "application/json")
        {
            var response = new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(content, System.Text.Encoding.UTF8, mediaType)
            };
            return new HttpClient(new FakeHttpMessageHandler(response));
        }
    }
}