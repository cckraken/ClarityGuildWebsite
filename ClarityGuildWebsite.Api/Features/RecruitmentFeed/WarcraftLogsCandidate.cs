namespace ClarityGuildWebsite.Api.Features.RecruitmentFeed;

public sealed record WarcraftLogsCandidate(
    long WarcraftLogsId,
    string Name,
    string ClassName,
    string RealmName,
    string Region,
    string? Spec,
    string? Metric,
    RaidProgressLine? Progress);

public sealed record RaidProgressLine(int Kills, int Total, string Difficulty, string RaidName);