using Discord.WebSocket;

namespace ClarityGuildWebsite.Api.Features.DiscordBot
{
    public static class DiscordGuildOptionsCollectionExtensions
    {
        public static IServiceCollection AddDiscordGuildOptions(this IServiceCollection services, IConfiguration config)
        {
            services.AddOptions<DiscordGuildOptions>()
                .Bind(config.GetSection(DiscordGuildOptions.SectionName))
                .Validate(o => o.GuildId > 0, "Discord:GuildId is not configured.")
                .Validate(o => o.RaidersChatId > 0, "Discord:RaidersChatId is not configured.")
                .Validate(o => o.TrialRoleId > 0, "Discord:TrailRoleId is not configured.")
                .Validate(o => o.RecruitmentTeamRoleIds is { Length: > 0 }, "Discord:RecruitmentTeamRoleIds is not configured.")
                .ValidateOnStart();

            services.AddSingleton(_ => new DiscordSocketClient(new DiscordSocketConfig
            {
                ConnectionTimeout = 15000,
                GatewayIntents = Discord.GatewayIntents.Guilds | Discord.GatewayIntents.DirectMessages | Discord.GatewayIntents.GuildMessages | Discord.GatewayIntents.MessageContent,
                UseInteractionSnowflakeDate = false
            }));
            services.AddSingleton<DiscordBotConnectionService>();
            return services;
                
        }
    }
}
