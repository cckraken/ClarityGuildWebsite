using Discord;

namespace ClarityGuildWebsite.Api.Services.DiscordBotInteractions.Feeds
{
    public static class RecruitmentFeedEmbeds
    {
        public static EmbedBuilder AddLogs(EmbedBuilder builder, FirstKillSummary? summary)
        {
            if (summary is null)
                return builder.AddField("Mythic logs", "No public logs");

            var lines = summary.Bosses.Select(s => s.FirstKillParse is null
                ? $"{s.BossName}: -"
                : $"{s.BossName}: First: {s.FirstKillParse}");
            return builder.AddField("Mythic logs", string.Join("\n", lines));
        }
    }
}
