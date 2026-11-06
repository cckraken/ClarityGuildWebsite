using Discord;

namespace ClarityGuildWebsite.Api.Services.DiscordBotInteractions.Feeds
{
    public static class RecruitmentFeedEmbeds
    {
        public static EmbedBuilder AddLogs(EmbedBuilder builder, FirstKillSummary? summary)
        {
            if (summary is null)
                return builder.AddField("Mythic logs", "No public logs");

            var lines = summary.Bosses.Select(s => s.FirstKillParse is double parse
                ? $"{s.BossName}: {Math.Floor(parse):0}"
                : $"{s.BossName}: —");
            return builder.AddField("Mythic logs", string.Join("\n", lines));
        }
    }
}
