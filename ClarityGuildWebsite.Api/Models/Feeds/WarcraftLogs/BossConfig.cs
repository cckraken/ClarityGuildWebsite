namespace ClarityGuildWebsite.Api.Models.Feeds.WarcraftLogs
{
    public class BossConfig()
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ShortName { get; init; }

        public string DisplayName => string.IsNullOrWhiteSpace(ShortName) ? Name : ShortName;
    }
}
