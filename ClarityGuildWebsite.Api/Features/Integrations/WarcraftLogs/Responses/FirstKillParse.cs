namespace ClarityGuildWebsite.Api.Features.Integrations.WarcraftLogs.Responses
{
    public record FirstKillParse(
        string BossId,
        DateTimeOffset KilledAt,
        TimeSpan Duration,
        string? Spec,
        double RankPercent,
        double Dps,
        string? ReportCode,
        int? FightId);
}
