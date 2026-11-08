using System.Text.Json;
using System.Text.Json.Serialization;

namespace ClarityGuildWebsite.Api.Features.Integrations.WarcraftLogs.Responses
{

    public class CharacterObj
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonExtensionData]
        public Dictionary<string, JsonElement> BossFields { get; set; } = new();
    }

    public class RaidStats
    {
        [JsonPropertyName("bestAmount")]
        public double BestAmount { get; set; }

        [JsonPropertyName("medianPerformance")]
        public double? MedianPerformance { get; set; }

        [JsonPropertyName("averagePerformance")]
        public double? AveragePerformance { get; set; }

        [JsonPropertyName("totalKills")]
        public int TotalKills { get; set; }

        [JsonPropertyName("fastestKill")]
        public long FastestKill { get; set; }

        [JsonPropertyName("difficulty")]
        public int Difficulty { get; set; }

        [JsonPropertyName("metric")]
        public string? Metric { get; set; }

        [JsonPropertyName("partition")]
        public int? Partition { get; set; }

        [JsonPropertyName("zone")]
        public int? Zone { get; set; }

        [JsonPropertyName("ranks")]
        public List<RankedParse>? Ranks { get; set; }
    }

    public class RankedParse
    {
        [JsonPropertyName("lockedIn")] public bool LockedIn { get; set; }
        [JsonPropertyName("rankPercent")] public double RankPercent { get; set; }
        [JsonPropertyName("historicalPercent")] public double HistoricalPercent { get; set; }
        [JsonPropertyName("todayPercent")] public double TodayPercent { get; set; }
        [JsonPropertyName("rankTotalParses")] public int RankTotalParses { get; set; }
        [JsonPropertyName("historicalTotalParses")] public int HistoricalTotalParses { get; set; }
        [JsonPropertyName("todayTotalParses")] public int TodayTotalParses { get; set; }
        [JsonPropertyName("guild")] public GuildInfo? Guild { get; set; }
        [JsonPropertyName("report")] public ReportInfo? Report { get; set; }
        [JsonPropertyName("duration")] public long Duration { get; set; }
        [JsonPropertyName("startTime")] public long StartTime { get; set; }
        [JsonPropertyName("amount")] public double Amount { get; set; }
        [JsonPropertyName("bracketData")] public int BracketData { get; set; }
        [JsonPropertyName("spec")] public string? Spec { get; set; }
        [JsonPropertyName("bestSpec")] public string? BestSpec { get; set; }
        [JsonPropertyName("class")] public int Class { get; set; }
        [JsonPropertyName("faction")] public int Faction { get; set; }
    }

    public class GuildInfo
    {
        [JsonPropertyName("id")] public int Id { get; set; }
        [JsonPropertyName("name")] public string? Name { get; set; }
        [JsonPropertyName("faction")] public int Faction { get; set; }
    }

    public class ReportInfo
    {
        [JsonPropertyName("code")] public string? Code { get; set; }
        [JsonPropertyName("startTime")] public long StartTime { get; set; }
        [JsonPropertyName("fightID")] public int FightID { get; set; }
    }
}