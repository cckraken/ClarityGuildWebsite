using Discord;

namespace ClarityGuildWebsite.Api.Features.RecruitmentFeed;

public static class RecruitmentFeedEmbeds
{
    private const string LogsFieldName = "Mythic - current tier stats:";
    private const int FieldValueLimit = 1024;   // Discord's limit for one embed field
    private const string Fence = "```";

    public static EmbedBuilder AddLogs(EmbedBuilder builder, FirstKillSummary? summary)
    {
        if (summary is null || summary.Bosses.Count == 0)
            return builder.AddField(LogsFieldName, "Logs unavailable");

        var nameWidth = summary.Bosses.Max(b => b.BossName.Length);

        var lines = new List<string>
        {
            $"{"".PadRight(nameWidth)}  First    Ilvl  Best   Kills"
        };

        foreach (var boss in summary.Bosses)
        {
            var name = boss.BossName.PadRight(nameWidth);

            if (boss.FirstKillParse is not double first)
            {
                lines.Add($"{name}  No kill");
                continue;
            }

            var best = boss.BestParse is double bestParse ? $"{ParseEmoji(bestParse)}{Math.Floor(bestParse),3:0}" : "   —";
            
            lines.Add($"{name}  {ParseEmoji(first)}{Math.Floor(first),4:0}  {boss.FirstKillItemLevel,4}  {best}  {boss.Kills,5}");
        }

        var body = string.Join("\n", lines);

        // The fences and their newlines take 8 characters; never exceed Discord's field limit.
        var maxBody = FieldValueLimit - (Fence.Length * 2) - 2;
        if (body.Length > maxBody)
            body = body[..maxBody];

        return builder.AddField(LogsFieldName, $"{Fence}\n{body}\n{Fence}");
    }

    private static string ParseEmoji(double parse) => parse switch
    {
        >= 100 => "🟨",   // gold
        >= 95 => "🟧",   // orange
        >= 75 => "🟪",   // purple
        >= 50 => "🟦",   // blue
        >= 25 => "🟩",   // green
        _ => "⬜",   // grey
    };
}