using Discord;
using Discord.WebSocket;
using ClarityGuildWebsite.Api.Features.Integrations.WarcraftLogs;
using ClarityGuildWebsite.Api.Features.Integrations.WarcraftLogs.GQLQueries;
using ClarityGuildWebsite.Api.Services.DiscordBotInteractions.Feeds;

namespace ClarityGuildWebsite.Api.Features.Integrations.WarcraftLogs.Processing
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
        private readonly HashSet<ulong> _allowedWebhookIds;

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

            var bosses = config.GetSection("WarcraftLogs:Raids:VA:Bosses").Get<BossConfig[]>();
            if (bosses is null || bosses.Length == 0)
                throw new InvalidOperationException("WarcraftLogs:Raids:VA:Bosses is not configured.");
            _bosses = bosses;

            var allowed = config.GetSection("RecruitmentFeed:AllowedWebhookIds").Get<ulong[]>();
            if (allowed is null || allowed.Length == 0)
                throw new InvalidOperationException("RecruitmentFeed:AllowedWebhookIds is not configured.");
            _allowedWebhookIds = [.. allowed];

            _firstKillsQuery = Queries.FirstKills(_bosses);
        }

        public async Task HandleWebhookReceivedAsync(IMessage message)
        {
            if (message.Channel.Id != _incomingChannelId) return;
            if (message.Embeds.Count == 0) return;

            if (message.Author is not IWebhookUser webhookUser || !_allowedWebhookIds.Contains(webhookUser.WebhookId))
            {
                _logger.LogWarning("Ignored a message in the feed channel from an unexpected sender {AuthorId}", message.Author.Id);
                return;
            }

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

                try
                {
                    var character = await _warcraftLogsClient.GetCharacterRankingsAsync(candidate.WarcraftLogsId, candidate.Metric ?? "dps", _firstKillsQuery, CancellationToken.None);
                    var summary = character is null ? null : FirstKillSummaryBuilder.Build(character, _bosses);

                    RecruitmentFeedEmbeds.AddLogs(builder, summary);
                }
                catch (Exception ex) when (ex is HttpRequestException or OperationCanceledException)
                {
                    _logger.LogWarning(ex, "Warcraft Logs enrichment failed for feed message {MessageId}", message.Id);
                    builder.AddField("Mythic - current tier stats:", "Stats temporarily unavailable");
                }
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


