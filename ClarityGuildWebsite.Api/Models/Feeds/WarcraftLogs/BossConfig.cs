namespace ClarityGuildWebsite.Api.Models.Feeds.WarcraftLogs
{
    public class BossConfig
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string? ShortName { get; init; }

        public string DisplayName => string.IsNullOrWhiteSpace(ShortName) ? Name : ShortName;
    }
}
