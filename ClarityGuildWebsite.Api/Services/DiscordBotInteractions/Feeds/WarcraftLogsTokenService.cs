using Microsoft.IdentityModel.Tokens;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ClarityGuildWebsite.Api.Services.DiscordBotInteractions.Feeds
{
    public class WarcraftLogsTokenService
    {
        private readonly HttpClient _client;
        private DateTime _tokenExpiryTime = DateTime.Now;
        private readonly string _wlApiUrl = "https://www.warcraftlogs.com/oauth/token";
        private string _wlApiToken;
        private readonly string _wlogsClientSecret;
        private readonly string _wlogsClientId;

        public WarcraftLogsTokenService(HttpClient client, IConfiguration config)
        {
            _client = client;


            _wlogsClientId = config.GetValue<string>("WarcraftLogs:ClientId");
            if (string.IsNullOrWhiteSpace(_wlogsClientId))
                throw new InvalidOperationException("WarcraftLogs:ClientId is not configuired or is invalid.");

            _wlogsClientSecret = config.GetValue<string>("WarcraftLogs:ClientSecret");
            if (string.IsNullOrWhiteSpace(_wlogsClientSecret))
                throw new InvalidOperationException("WarcraftLogs:ClientSecret is not configuired or is invalid.");

            _wlApiToken = config.GetValue<string>("WarcraftLogs:ApiToken");
            if (string.IsNullOrWhiteSpace(_wlApiToken))
                throw new InvalidOperationException("WarcraftLogs:ApiToken is not configuired or is invalid.");
        }

        public async Task<string> GetValidTokenAsync()
        {
            if (!string.IsNullOrEmpty(_wlApiToken) && DateTime.UtcNow < _tokenExpiryTime.AddMinutes(-5))
            {
                return _wlApiToken;
            }

            var requestData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            };

            using (var request = new HttpRequestMessage(HttpMethod.Post, _wlApiUrl))
            {
                request.Content = new FormUrlEncodedContent(requestData);
                var authString = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_wlogsClientId}:{_wlogsClientSecret}"));
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authString);

                HttpResponseMessage response = await _client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                string jsonContent = await response.Content.ReadAsStringAsync();
                using (JsonDocument doc = JsonDocument.Parse(jsonContent))
                {
                    _wlApiToken = doc.RootElement.GetProperty("access_token").GetString();

                    int expiresInSeconds = doc.RootElement.GetProperty("expires_in").GetInt32();
                    _tokenExpiryTime = DateTime.UtcNow.AddSeconds(expiresInSeconds);

                    return _wlApiToken;
                }
            }

        }
    }
}
