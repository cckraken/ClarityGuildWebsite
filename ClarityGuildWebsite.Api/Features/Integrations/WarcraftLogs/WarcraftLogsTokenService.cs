using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ClarityGuildWebsite.Api.Features.Integrations.WarcraftLogs
{

    public sealed class WarcraftLogsTokenService
    {
        private const string TokenUrl = "https://www.warcraftlogs.com/oauth/token";
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly SemaphoreSlim _lock = new(1, 1);
        private string? _wlApiToken;
        private DateTimeOffset _expiresAt = DateTimeOffset.MinValue;

        public WarcraftLogsTokenService(IHttpClientFactory httpClientFactory, IOptions<WarcraftLogsOptions> logsOptions)
        {
            _httpClientFactory = httpClientFactory;
            var logs = logsOptions.Value;
            _clientId = logs.ClientId;
            _clientSecret = logs.ClientSecret;
        }

        public async Task<string> GetTokenAsync(CancellationToken ctoken)
        {
            if (_wlApiToken is not null && DateTimeOffset.UtcNow < _expiresAt.AddMinutes(-5)) return _wlApiToken;

            await _lock.WaitAsync(ctoken);
            try
            {
                if (_wlApiToken is not null && DateTimeOffset.UtcNow < _expiresAt.AddMinutes(-5)) return _wlApiToken;

                var client = _httpClientFactory.CreateClient("WarcraftLogs");
                using var request = new HttpRequestMessage(HttpMethod.Post, TokenUrl)
                {
                    Content = new FormUrlEncodedContent([new("grant_type", "client_credentials")])
                };

                var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}"));
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                using var response = await client.SendAsync(request, ctoken);
                response.EnsureSuccessStatusCode();

                string jsonContent = await response.Content.ReadAsStringAsync();
                using (JsonDocument doc = JsonDocument.Parse(jsonContent))
                {
                    _wlApiToken = doc.RootElement.GetProperty("access_token").GetString();
                    if (string.IsNullOrEmpty(_wlApiToken))
                        throw new InvalidOperationException("Warcraft Logs token response had no access_token.");

                    int expiresInSeconds = doc.RootElement.GetProperty("expires_in").GetInt32();
                    _expiresAt = DateTime.UtcNow.AddSeconds(expiresInSeconds);
                    return _wlApiToken;
                }
            }
            finally
            {
                _lock.Release();
            }
        }
    }
}