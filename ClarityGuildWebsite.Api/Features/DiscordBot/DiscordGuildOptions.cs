namespace ClarityGuildWebsite.Api.Features.DiscordBot
{
    public sealed class DiscordGuildOptions
    {
        public const string SectionName = "DiscordGuild";
        public ulong GuildId { get; init; } = 0;
        public ulong[] RecruitmentTeamRoleIds { get; init; } = [];
        public ulong RaidersChatId { get; init; } = 0;
        public ulong TrialRoleId { get; init; } = 0;
        public ulong[] DefaultWelcomeChannelIds { get; init; } = [];

    }
}
