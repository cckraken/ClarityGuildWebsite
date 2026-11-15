using ClarityGuildWebsite.Api.Features.DiscordBot.Interactions;
using ClarityGuildWebsite.Api.Features.DiscordBot.SlashCommands;
using Discord.Rest;
using Discord.WebSocket;

namespace ClarityGuildWebsite.Api.Features.DiscordBot
{
    public static class DiscordBotServiceCollectionExtensions
    {
        public static IServiceCollection AddDiscordBot(this IServiceCollection services, IConfiguration config)
        {
            services.AddOptions<DiscordBotOptions>()
                .Bind(config.GetSection(DiscordBotOptions.SectionName))
                .Validate(o => o.GuildId > 0, "Discord:GuildId is not configured.")
                .Validate(o => !string.IsNullOrWhiteSpace(o.BotToken), "Discord:BotToken is not configured.")
                .Validate(o => o.RaidersChatId > 0, "Discord:RaidersChatId is not configured.")
                .Validate(o => o.TrialRoleId > 0, "Discord:TrialRoleId is not configured.")
                .Validate(o => o.RecruitmentTeamRoleIds is { Length: > 0 }, "Discord:RecruitmentTeamRoleIds is not configured.")
                .Validate(o => o.DefaultWelcomeChannelIds is { Length: > 0 }, "Discord:DefaultWelcomeChannelIds is not configured.")
                .Validate(o => o.ApplicationsChannelId > 0, "Discord:ApplicationsChannelId is not configured.")
                .ValidateOnStart();

            services.AddSingleton(_ => new DiscordSocketClient(new DiscordSocketConfig
            {
                ConnectionTimeout = 15000,
                GatewayIntents = Discord.GatewayIntents.Guilds | Discord.GatewayIntents.DirectMessages | Discord.GatewayIntents.GuildMessages | Discord.GatewayIntents.MessageContent,
                UseInteractionSnowflakeDate = false
            }));
            services.AddSingleton<DiscordBotConnectionService>();
            services.AddSingleton<DiscordRestClient>();
            services.AddHostedService<DiscordGatewayService>();
            services.AddScoped<DiscordInteractionRouter>();
            services.AddScoped<DiscordSlashCommandsRouter>();
            services.AddScoped<DiscordSlashCommands>();
            return services;  
        }
    }
}
