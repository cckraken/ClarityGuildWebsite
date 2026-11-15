namespace ClarityGuildWebsite.Api.Features.DiscordBot
{
    public sealed class DiscordBotOptions
    {
        public const string SectionName = "Discord";
        public string DiscordBotToken { get; init; } = "";
        public string DiscordWebhookUrl { get; init; } = "";
        public ulong GuildId { get; init; } = 0;
        public ulong[] RecruitmentTeamRoleIds { get; init; } = [];
        public ulong RaidersChatId { get; init; } = 0;
        public ulong TrialRoleId { get; init; } = 0;
        public ulong ApplicationsChannelId { get; init; } = 0; 
        public ulong[] DefaultWelcomeChannelIds { get; init; } = [];

    }
}
