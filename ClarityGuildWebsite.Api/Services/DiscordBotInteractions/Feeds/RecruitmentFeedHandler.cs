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
        private readonly ILogger _logger;
        private readonly WarcraftLogsClient _warcraftLogsClient;
        private readonly BossConfig[] _bosses;
        private readonly string _firstKillsQuery;
        private readonly ulong _incomingChannelId;
        private readonly ulong _outgoingChannelId;

        public RecruitmentFeedHandler(DiscordSocketClient socketClient, ILogger<RecruitmentFeedHandler> logger, WarcraftLogsClient warcraftLogsClient, IConfiguration config)
        {
            _socketClient = socketClient;
            _logger = logger;
            _warcraftLogsClient = warcraftLogsClient;

            _incomingChannelId = config.GetValue<ulong>("RecruitmentFeed:IncomingChannelId");
            if (_incomingChannelId == 0)
                throw new InvalidOperationException("RecruitmentFeed:IncomingChannelId is not configured.");

            _outgoingChannelId = config.GetValue<ulong>("RecruitmentFeed:OutgoingChannelId");
            if (_outgoingChannelId == 0)
                throw new InvalidOperationException("RecruitmentFeed:OutgoingChannelId is not configured.");

            _bosses = config.GetSection("WarcraftLogs:Raids:VA:Bosses").Get<BossConfig[]>();
            if (_bosses is null || _bosses.Length == 0)
                throw new InvalidOperationException("WarcraftLogs:Bosses is not configured.");

            _firstKillsQuery = Queries.FirstKills(_bosses);
        }

        public async Task HandleWebhookReceivedAsync(IMessage message)
        {
            if (message.Channel.Id != _incomingChannelId) return;
            if (message is null) return;
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

                var builder = embed.ToEmbedBuilder();

                var character = await _warcraftLogsClient.GetCharacterRankingsAsync(candidate.WarcraftLogsId, candidate.Metric ?? "dps", _firstKillsQuery, CancellationToken.None);
                var summary = character is null ? null : FirstKillSummaryBuilder.Build(character, _bosses);

                RecruitmentFeedEmbeds.AddLogs(builder, summary);

                await outgoing.SendMessageAsync(embed: builder.Build(), allowedMentions: AllowedMentions.None);
            }
        }

        //public async Task<Dictionary<string, FirstKillParse>?> ParseFirstKills(long id)
        //{
        //    var character = await FetchParseData(id);
        //    if (character is null) return null;

        //    return character.BossFields
        //        .Select(b => new { b.Key, Stats = b.Value.Deserialize<RaidStats>() })
        //        .Where(b => b.Stats?.Ranks is { Count: > 0 })
        //        .ToDictionary(
        //            b => b.Key,
        //            b =>
        //            {
        //                var first = b.Stats!.Ranks!.MinBy(r => r.StartTime)!;
        //                return new FirstKillParse(
        //                    b.Key,
        //                    DateTimeOffset.FromUnixTimeMilliseconds(first.StartTime),
        //                    TimeSpan.FromMilliseconds(first.Duration),
        //                    first.Spec,
        //                    first.RankPercent,
        //                    first.Amount,
        //                    first.Report?.Code,
        //                    first.Report?.FightID);
        //            });
        //}

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

    }
}


