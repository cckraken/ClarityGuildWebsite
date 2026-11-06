using ClarityGuildWebsite.Api.Models.Feeds;
using System.Net.Http;
using System.Net.Http.Json;
using Discord;
using Discord.WebSocket;
using ClarityGuildWebsite.Api.Models.Feeds.WarcraftLogs;
using ClarityGuildWebsite.Api.Models.Feeds.WarcraftLogs.GQLQueries;
using ClarityGuildWebsite.Api.Services;
using System.Text.Json;
using Azure.Core;
using Microsoft.IdentityModel.Protocols;
using System.Text;

namespace ClarityGuildWebsite.Api.Services.DiscordBotInteractions.Feeds
{
    public class RecruitmentFeedHandler
    {
        private readonly DiscordSocketClient _socketClient;
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;
        private readonly WarcraftLogsTokenService _service;
        private readonly ulong _incomingChannelId;
        private readonly ulong _outgoingChannelId;

        public RecruitmentFeedHandler(DiscordSocketClient socketClient, HttpClient httpClient, ILogger<RecruitmentFeedHandler> logger, WarcraftLogsTokenService service, IConfiguration config)
        {
            _socketClient = socketClient;
            _httpClient = httpClient;
            _logger = logger;
            _service = service;

            _incomingChannelId = config.GetValue<ulong>("RecruitmentFeed:IncomingChannelId");
            if (_incomingChannelId == 0)
                throw new InvalidOperationException("RecruitmentFeed:IncomingChannelId is not configured.");

            _outgoingChannelId = config.GetValue<ulong>("RecruitmentFeed:OutgoingChannelId");
            if (_outgoingChannelId == 0)
                throw new InvalidOperationException("RecruitmentFeed:OutgoingChannelId is not configured.");
        }

        public async Task HandleWebhookReceivedAsync(IMessage message)
        {
            if (message is not IMessage) return;
            if (message is null) return;
            if (message.Channel is IDMChannel) return;
            if (message.Embeds.Count == 0) return;
            if (message.Source != MessageSource.Webhook) return;

            if (_socketClient.GetChannel(_outgoingChannelId) is not IMessageChannel outgoing)
            {
                _logger.LogError("Recruitment feed output channel {ChannelId} not found or not a text channel", _outgoingChannelId);
                return;
            }

            foreach (var embed in message.Embeds)
            {
                var candidate = WarcraftLogsEmbedParser.TryParse(embed);
                if (candidate is null)
                {
                    _logger.LogWarning("Skipped an embed in feed message {MessageId}: not in the expected Warcraft Logs format", message.Id);
                    continue;
                }

                //enrich ToDo
                var builder = embed.ToEmbedBuilder();

                await outgoing.SendMessageAsync(embed: builder.Build(), allowedMentions: AllowedMentions.None);

                
                var raidStats = await FetchParseData(candidate.WarcraftLogsId);

                var firstKills = await ParseFirstKills(candidate.WarcraftLogsId);

                FirstKillSummary? summary = firstKills is null
                    ? null
                    : new FirstKillSummary(
                        Bosses: firstKills
                            .Select(kv => new FirstParse(
                                BossName: kv.Key,
                                BestParse: null,                    // not available from FirstKillParse
                                FirstKillParse: kv.Value.RankPercent,
                                FirstKillItemLevel: null,           // not available from FirstKillParse
                                Kills: 1))                          // placeholder
                            .ToList(),
                        AverageBest: null);

                var builderFirstParse = embed.ToEmbedBuilder();
                RecruitmentFeedEmbeds.AddLogs(builderFirstParse, summary);

                await outgoing.SendMessageAsync(embed: builderFirstParse.Build(), allowedMentions: AllowedMentions.None);

            }
        }

        public async Task<Dictionary<string, FirstKillParse>?> ParseFirstKills(long id)
        {
            var character = await FetchParseData(id);
            if (character is null) return null;

            return character.BossFields
                .Select(b => new { b.Key, Stats = b.Value.Deserialize<RaidStats>() })
                .Where(b => b.Stats?.Ranks is { Count: > 0 })
                .ToDictionary(
                    b => b.Key,
                    b =>
                    {
                        var first = b.Stats!.Ranks!.MinBy(r => r.StartTime)!;
                        return new FirstKillParse(
                            b.Key,
                            DateTimeOffset.FromUnixTimeMilliseconds(first.StartTime),
                            TimeSpan.FromMilliseconds(first.Duration),
                            first.Spec,
                            first.RankPercent,
                            first.Amount,
                            first.Report?.Code,
                            first.Report?.FightID);
                    });
        }

        //public static CharacterObj? ParseMedian(CharacterObj raidStats)
        //{
        //    var character = raidStats?.BossFields;
        //    if (character is null)
        //        return null;

        //    foreach (KeyValuePair<string, JsonElement> boss in character)
        //    {
        //        string bossId = boss.Key;
        //        RaidStats? stats = boss.Value.Deserialize<RaidStats>();
        //        if (stats is null || stats.TotalKills == 0) continue;

        //        var bossName = bossId;
        //        var avgParse = stats.MedianPerformance;

        //    }
        //}

        public async Task<CharacterObj?> FetchParseData(long id)
        {

            string graphQLQuery = Queries.FirstKillsData;
            var payload = new
            {
                query = graphQLQuery,
                variables = new { id = id, difficulty = 5, metric = "dps" }
            };
            string jsonRequestBody = JsonSerializer.Serialize(payload);

            using HttpContent content = new StringContent(content: jsonRequestBody, encoding: Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, "https://www.warcraftlogs.com/api/v2/client");
            request.Content = content;
            var token = await _service.GetValidTokenAsync();
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("WarcraftLogs API error ({StatusCode}): {ErrorContent}", response.StatusCode, errorContent);
                return null;
            }


            var jsonResponse = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(jsonResponse);
            if (doc.RootElement.TryGetProperty("errors", out var errors))
            {
                _logger.LogError("WarcraftLogs GraphQL errors: {Errors}", errors.GetRawText());
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


