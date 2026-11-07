namespace ClarityGuildWebsite.Api.Services.DiscordBotInteractions.Feeds
{
    public sealed record FirstKillSummary(IReadOnlyList<FirstParse> Bosses);

    public sealed record FirstParse(
        string BossName,
        double? FirstKillParse,
        int? FirstKillItemLevel,
        double? BestParse,
        int Kills);
}