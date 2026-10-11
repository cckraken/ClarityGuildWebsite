using Azure;
using ClarityGuildWebsite.Api.Models;
using ClarityGuildWebsite.Api.Helpers;
using Microsoft.AspNetCore.HttpLogging;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Reflection;
using System.Text.Json;


namespace ClarityGuildWebsite.Api.Services
{
    public class DiscordWebhookNotifier : IApplicationNotifier
    {
        private readonly HttpClient _http;
        private readonly string _webhookUrl;

        public DiscordWebhookNotifier(HttpClient http, IConfiguration config)
        {
            _http = http;
            _webhookUrl = config["Discord:WebhookUrl"] ?? throw new InvalidOperationException("Discord:WebhookUrl is not configured.");
        }

        public async Task<NotificationResult> NotifyNewApplicationAsync(GuildApplication application, CancellationToken ctoken)
        {
            var embed = new DiscordEmbed(
                Title: "New Application " + application.MainName,
                Fields:
                [
                   new("Discord ID", application.DiscordId, true),
                   new("Country", application.Country, true),
                   new("Main Character", ApplicationHelpers.Combine(application.MainName, application.MainRealm, application.MainRole), true),
                   new("Alt Character", ApplicationHelpers.Combine(application.AltName, application.AltRealm, application.AltRole), true),
                   new("UI Screenshot", application.Screenshot, true),
                   new("Tech", application.Tech, false),
                   new("About", ApplicationHelpers.Truncate(application.About, 1024), false),
                   new("History", ApplicationHelpers.Truncate(application.History, 1024), false),
                ]);
            if (string.IsNullOrWhiteSpace(application.Extra))
                embed.Fields.Add(new("Extra", "N/A"));
            else
            {
                embed.Fields.Add(new("Extra", ApplicationHelpers.Truncate(application.Extra, 1024)));
            }
            try
            {
                var payload = new DiscordWebhookPayload(Embeds: new List<DiscordEmbed> { embed });
                using var response = await _http.PostAsJsonAsync(_webhookUrl + "?wait=true", payload, ctoken);
                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync(ctoken);
                    return new NotificationResult(false, null, $"{(int)response.StatusCode}: {body}");
                }
                else
                {
                    var message = await response.Content.ReadFromJsonAsync<DiscordMessageResponse>(ctoken);
                    return new NotificationResult(true, message?.Id, null);
                }
            }
            catch (HttpRequestException ex)
            {
                return new NotificationResult(false, null, $"Discord cannot be reached: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                if(ctoken.IsCancellationRequested)
                {
                    return new NotificationResult(false, null, $"User canceled operation: {ex.Message}");
                }
                else return new NotificationResult(false, null, $"Request timed out: {ex.Message}");
            }
            catch (JsonException ex)
            {
                return new NotificationResult(false, null, $"Discords reponse was not valid JSON: {ex.Message}");
            }
        }
    }
}
