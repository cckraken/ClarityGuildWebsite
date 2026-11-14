namespace ClarityGuildWebsite.Api.Features.Integrations.WarcraftLogs
{
    public sealed class WarcaftLogsOptions
    {
        public const string SectionName = "WarcraftLogs";

        public string ClientId { get; init; } = "";
        public string ClientSecret { get; init; } = "";
        public BossConfig[] Bosses { get; init; } = [];
    }
}
