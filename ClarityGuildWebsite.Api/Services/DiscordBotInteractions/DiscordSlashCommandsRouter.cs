using ClarityGuildWebsite.Api.Data;
using ClarityGuildWebsite.Api.Helpers;
using ClarityGuildWebsite.Api.Models;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using System.Text;
using ClarityGuildWebsite.Api.Services;

namespace ClarityGuildWebsite.Api.Services.DiscordBotInteractions
{
    public class DiscordSlashCommandsRouter
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;
        private readonly ulong[] _recruitmentTeamRoleIds;
        public DiscordSlashCommandsRouter(ApplicationDbContext context, ILogger<DiscordSlashCommandsRouter> logger, IConfiguration config)
        {
            _recruitmentTeamRoleIds = config.GetSection("Discord:RecruitmentTeamRoleIds").Get<ulong[]>()
             ?? throw new InvalidOperationException("Discord:RecruitmentTeamRoleIds is not configured.");
            if (_recruitmentTeamRoleIds.Length == 0)
                throw new InvalidOperationException("Discord:RecruitmentTeamRoleIds must contain at least one role ID.");

            _context = context;
            _logger = logger;
        }

        public async Task HandleSlashCommandExecutedAsync(SocketSlashCommand command)
        {
            await command.DeferAsync(ephemeral: true);

            try
            {
                switch (command.Data.Name)
                {
                    case DiscordSlashCommands.RepostButtonsCommand:
                        await RepostButtonsAsync(command);
                        break;

                    default:
                        _logger.LogWarning("Unknown slash command {CommandName}", command.Data.Name);
                        await command.FollowupAsync("Unknown command.", ephemeral: true);
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Slash command {CommandName} failed", command.Data.Name);
                await command.FollowupAsync("Something went wrong. Please try again.", ephemeral: true);
            }
        }

        private async Task RepostButtonsAsync(SocketSlashCommand command)
        {
            if (command.User is not SocketGuildUser manager || !CanManageApplications(manager))
            {
                _logger.LogWarning("Non-recruitment manager {UserId} used /{CommandName}", command.User.Id, command.Data.Name);
                await command.FollowupAsync("Only recruitment managers can use this.", ephemeral: true);
                return;
            }

            var threadIdText = command.ChannelId?.ToString();
            var isApplicationThread = threadIdText is not null
                && await _context.GuildApplications.AnyAsync(a => a.DiscordThreadId == threadIdText);
            if (!isApplicationThread)
            {
                await command.FollowupAsync("This command only works inside an application thread.", ephemeral: true);
                return;
            }

            var channel = await command.GetChannelAsync();
            if (channel is null)
            {
                _logger.LogWarning("Couldn't re-post buttons in {ThreadId}: channel not available", threadIdText);
                await command.FollowupAsync("Couldn't find this thread. Please try again.", ephemeral: true);
                return;
            }

            await channel.SendMessageAsync(components: DiscordButtons.BuildApplicationButtons());
            _logger.LogInformation("Buttons re-posted in thread {ThreadId} by {UserId}", threadIdText, manager.Id);
            await command.FollowupAsync("Buttons re-posted.", ephemeral: true);
        }

        private bool CanManageApplications(SocketGuildUser user) =>
            user.Roles.Any(r => _recruitmentTeamRoleIds.Contains(r.Id));

    }
}
