using ClarityGuildWebsite.Api.Data;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Text;
using static ClarityGuildWebsite.Api.Services.DiscordBotInteractions.DiscordModals;

namespace ClarityGuildWebsite.Api.Services.DiscordBotInteractions
{
    public class DiscordInteractionRouter
    {
        private readonly DiscordRestClient _client;
        private readonly DiscordModals _modals;
        private readonly ulong _officerRoleId;
        private readonly IServiceScopeFactory _scopeFactory;

        public DiscordInteractionRouter(DiscordRestClient client, IConfiguration config, IServiceScopeFactory scopeFactory, DiscordModals modals)
        {
            _modals = modals;
            _client = client;
            _officerRoleId = config.GetValue<ulong>("Discord:OfficerRoleId");
            if (_officerRoleId == 0)
                throw new InvalidOperationException("Discord:OfficerRoleId is missing or invalid.");
            _scopeFactory = scopeFactory;

        }

        public async Task HandleButtonInteractionAsync(SocketMessageComponent component)
        {
            switch (component.Data.CustomId)
            {
                case "discordButtonMessageApplicant":
                    await HandleDiscordMessageApplicantAsync(component);
                    break;
                case "discordButtonMessageClarity":
                    await HandleDiscordMessageClarityAsync(component);
                    break;
                case "discordButtonAccept":
                    await HandleDiscordAcceptAsync(component);
                    break;
                case "discordButtonDecline":
                    await HandleDiscordDeclineAsync(component);
                    break;
            }
            return;
        }
        public async Task HandleDiscordMessageApplicantAsync(IComponentInteraction component)
        {
            try
            {
                string threadId = component.ChannelId.ToString();
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var messageObject = await context.GuildApplications
                    .Where(w => w.DiscordThreadId == threadId)
                    .Select(s => new { s.DiscordId, s.DiscordName })
                    .SingleOrDefaultAsync();

                if (messageObject == null)
                {
                    //todo
                    return;
                }

                string discordId = messageObject.DiscordId;
                string discordName = messageObject.DiscordName;

                if (component.User is SocketGuildUser user)
                {
                    if (user.Roles.Any(r => r.Name == "Officer"))
                    {
                        await component.RespondWithModalAsync(_modals.BuildOfficerMessageModalAsync(discordId, discordName));
                    }
                else
                    {
                        await component.RespondAsync("Officer role required for Message Applicant button", ephemeral: true);
                    }
                }
            }
            catch (Discord.Net.HttpException ex)
            {
                await component.RespondAsync("$Cannot send messages to this user: {ex.Message}");
            }
        }

        public async Task HandleModalSubmitted(SocketModal modal, SocketUser user)
        {
            var customIdParts = modal.Data.CustomId.Split(':');
            string modalType = customIdParts[0];

            if (modalType == "applicantMessageModal")
            {
                if (customIdParts.Length < 2)
                {
                    return;
                }

                string discordId = customIdParts[1];
                var components = modal.Data.Components.ToList();

                var messageComponent = components.FirstOrDefault(s => s.CustomId == "sendMessageToApplicantText");
                if (messageComponent == null)
                {
                    return;
                }

                string text = messageComponent.Value;

                IDMChannel dmChannel = await user.CreateDMChannelAsync();
                await dmChannel.SendMessageAsync(text);
            }

            if (modalType == "decline")
            {
                if (customIdParts.Length < 2)
                {
                    return;
                }

                string discordId = customIdParts[1];
                var components = modal.Data.Components.ToList();
                var reason = components
                    .FirstOrDefault(s => s.CustomId == DeclineModal.ReasonFieldId)
                    ?.Values.FirstOrDefault();
                var note = components
                    .FirstOrDefault(s => s.CustomId == DeclineModal.NoteFieldId)
                    ?.Value?.Trim() ?? "";

                
                if(reason is null || !DeclineReasons.All.Contains(reason))
                {
                    await modal.FollowupAsync("A reason must be selected", ephemeral: true);
                    return;
                }
                if (reason == DeclineReasons.Other && string.IsNullOrWhiteSpace(note))
                {
                    await modal.FollowupAsync("Note must be filled in if no reason selected from the dropdown", ephemeral: true);
                    return;
                }

                var friendlyReason = reason switch
                {
                    DeclineReasons.ClassClosed => "We're not currently looking for the classe(s) specified.",
                    DeclineReasons.LowExperience => "We don't feel your experience matches with what we are looking for at the moment.",
                    _ =>null, 
                };
                var message = new StringBuilder("Thank you for your application. Unfortunately we've decided not to go ahead with it at this time. ");
                if (friendlyReason is not null) message.Append(friendlyReason);
                if (!string.IsNullOrWhiteSpace(note)) message.Append(note);

                IDMChannel dmChannel = await user.CreateDMChannelAsync();
                await dmChannel.SendMessageAsync(message.ToString());
            }
        }
        

        public async Task HandleDiscordMessageClarityAsync(IComponentInteraction component)
        {
            //
        }

        public Task<NotificationResult> HandleDiscordAcceptAsync(IComponentInteraction componentInteraction)
        {
            throw new NotImplementedException();
        }

        public async Task HandleDiscordDeclineAsync(IComponentInteraction component)
        {
            try
            {
                string threadId = component.ChannelId.ToString();
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var messageObject = await context.GuildApplications
                    .Where(w => w.DiscordThreadId == threadId)
                    .Select(s => new { s.DiscordId, s.DiscordName, s.PublicId })
                    .SingleOrDefaultAsync();

                if (messageObject == null)
                {
                    return;
                }

                Guid publicId = messageObject.PublicId;
                string discordId = messageObject.DiscordId;
                string discordName = messageObject.DiscordName;
                CancellationToken ctoken = default;

                if (component.User is SocketGuildUser user)
                {
                    if (user.Roles.Any(r => r.Name == "Officer"))
                    {
                        await component.RespondWithModalAsync(_modals.BuildDeclineApplicant(discordName, discordId));
                        try
                        {
                            var toDecline = await context.GuildApplications.FirstOrDefaultAsync(u => u.PublicId == publicId);
                            if (toDecline != null)
                            {
                                toDecline.Status = ApplicationStatus.Declined;
                            }
                            await context.SaveChangesAsync();
                        }
                        catch (OperationCanceledException) when (ctoken.IsCancellationRequested)
                        {
                            throw;
                        }

                    }
                    else
                    {
                        await component.RespondAsync("Officer role required for Decline Applicant button", ephemeral: true);
                    }
                }
            }
            catch (Discord.Net.HttpException ex)
            {
                await component.RespondAsync("$Cannot send messages to this user: {ex.Message}");
            }
        }
    }
}
