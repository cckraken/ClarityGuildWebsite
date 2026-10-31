using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace ClarityGuildWebsite.Api.Services.DiscordBotInteractions
{
    public class DiscordSlashCommands
    {
        private readonly DiscordSocketClient _socketClient;
        public const string RepostButtonsCommand = "buttons";
        public const string ListCommandsCommand = "commands";
        private readonly ulong _guildId;
        public DiscordSlashCommands(DiscordSocketClient socketClient, IConfiguration config)
        {
            _guildId = config.GetValue<ulong>("Discord:GuildId");
            if (_guildId == 0)
                throw new InvalidOperationException("Discord:GuildId is missing or invalid.");
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
