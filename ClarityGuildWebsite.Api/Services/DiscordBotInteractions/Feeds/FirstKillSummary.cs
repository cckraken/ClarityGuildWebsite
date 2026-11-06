public sealed record FirstKillSummary(IReadOnlyList<FirstParse> Bosses, double? AverageBest);

public sealed record FirstParse(
    string BossName,
    double? BestParse,
    double? FirstKillParse,
    int? FirstKillItemLevel,
    int Kills);