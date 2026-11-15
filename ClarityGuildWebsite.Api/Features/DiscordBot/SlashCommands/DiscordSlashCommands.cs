using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Options;

namespace ClarityGuildWebsite.Api.Features.DiscordBot.SlashCommands
{
    public class DiscordSlashCommands
    {
        private readonly DiscordSocketClient _socketClient;
        public const string RepostButtonsCommand = "buttons";
        private readonly ulong _guildId;
        public DiscordSlashCommands(DiscordSocketClient socketClient, IOptions<DiscordBotOptions> discordOptions)
        {
            _guildId = discordOptions.Value.GuildId;
            _socketClient = socketClient;
        }

        public async Task RegisterSlashCommandsAsync()
        {
            var guild = _socketClient.GetGuild(_guildId)
                ?? throw new InvalidOperationException($"Guild {_guildId} not found. Check Discord:GuildId and that the bot is in the server.");

            var repostButtons = new SlashCommandBuilder()
                .WithName(RepostButtonsCommand)
                .WithDescription("Re-post the application buttons in this thread");

            await guild.BulkOverwriteApplicationCommandAsync([repostButtons.Build()]);
        }
    }
}
