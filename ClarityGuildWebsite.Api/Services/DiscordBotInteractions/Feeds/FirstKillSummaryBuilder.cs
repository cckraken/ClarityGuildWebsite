using ClarityGuildWebsite.Api.Models.Feeds.WarcraftLogs;
using System.Text.Json;

namespace ClarityGuildWebsite.Api.Services.DiscordBotInteractions.Feeds
{
    public static class FirstKillSummaryBuilder
    {
        public static FirstKillSummary Build(CharacterObj character, IReadOnlyList<BossConfig> bosses)
        {
            var rows = new List<FirstParse>();

            foreach (var boss in bosses)
            {
                RaidStats? stats = null;
                if (character.BossFields.TryGetValue($"e{boss.Id}", out var element))
                    stats = element.Deserialize<RaidStats>();

                var ranks = stats?.Ranks ?? [];
                if (ranks.Count == 0)
                {
                    rows.Add(new FirstParse(boss.Name, null, null, null, 0));
                    continue;
                }

                var first = ranks.MinBy(s => s.StartTime);
                var best = ranks.MaxBy(s => s.RankPercent);
                rows.Add(new FirstParse(boss.DisplayName, null, null, null, 0));
                rows.Add(new FirstParse(boss.Name, first.RankPercent, first.BracketData, best.RankPercent, stats!.TotalKills));
            }

            return new FirstKillSummary(rows);
        }
    }
}
