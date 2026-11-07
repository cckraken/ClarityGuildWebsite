using ClarityGuildWebsite.Api.Models.Feeds.WarcraftLogs;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ClarityGuildWebsite.Api.Services.DiscordBotInteractions.Feeds
{
    public sealed class WarcraftLogsClient(HttpClient http, WarcraftLogsTokenService tokens, ILogger<WarcraftLogsClient> logger)
    {
        public async Task<CharacterObj?> GetCharacterRankingsAsync(long characterId, string metric, string query, CancellationToken ctoken)
        {
            var payload = new
            {
                query,
                variables = new { id = characterId, difficulty = 5, metric}
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, "client")
            {
                Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await tokens.GetTokenAsync(ctoken));

            using var response = await http.SendAsync(request, ctoken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(ctoken);
                logger.LogError("WarcraftLogs API error ({StatusCode}): {ErrorContent}", response.StatusCode, errorContent);
                return null;
            }

            var jsonResponse = await response.Content.ReadAsStringAsync(ctoken);

            using var doc = JsonDocument.Parse(jsonResponse);
            if (doc.RootElement.TryGetProperty("errors", out var errors))
            {
                logger.LogError("WarcraftLogs GraphQL errors: {Errors}", errors.GetRawText());
            }

            if (!doc.RootElement.TryGetProperty("data", out var data)
                || !data.TryGetProperty("characterData", out var characterData)
                || !characterData.TryGetProperty("character", out var characterElement)
                || characterElement.ValueKind != JsonValueKind.Object)
            {
                return null;
            }

            return characterElement.Deserialize<CharacterObj>();

        }
    }
}