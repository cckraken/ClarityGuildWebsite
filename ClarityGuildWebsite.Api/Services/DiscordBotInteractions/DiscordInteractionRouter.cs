using ClarityGuildWebsite.Api.Data;
using ClarityGuildWebsite.Api.Models;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using System.Text;
using static ClarityGuildWebsite.Api.Services.DiscordBotInteractions.DiscordModals;

namespace ClarityGuildWebsite.Api.Services.DiscordBotInteractions
{
    public class DiscordInteractionRouter
    {
        private const string CouldNotDmMessage =
            "Couldn't DM the applicant: they're not in the server or have DMs from server members turned off.";
        private readonly ApplicationDbContext _context;
        private readonly DiscordSocketClient _socketClient;
        private readonly ILogger<DiscordInteractionRouter> _logger;
        private readonly ulong _officerRoleId;
        private readonly ulong _raidersChatId;
        private readonly ulong _trialRoleId;
        private readonly ulong[] _defaultWelcomeChannelIds;

        public DiscordInteractionRouter(ApplicationDbContext context, DiscordSocketClient socketClient, ILogger<DiscordInteractionRouter> logger, IConfiguration config)
        {
            _officerRoleId = config.GetValue<ulong>("Discord:OfficerRoleId");
            if (_officerRoleId == 0)
                throw new InvalidOperationException("Discord:OfficerRoleId is not configured.");

            _raidersChatId = config.GetValue<ulong>("Discord:RaidersChatId");
            if (_raidersChatId == 0)
                throw new InvalidOperationException("Discord:RaidersChatId is not configured.");

            _trialRoleId = config.GetValue<ulong>("Discord:TrialRoleId");
            if (_trialRoleId == 0)
                throw new InvalidOperationException("Discord:TrialRoleId is not configured.");

            _defaultWelcomeChannelIds = config.GetSection("Discord:DefaultWelcomeChannelIds").Get<ulong[]>()
                 ?? throw new InvalidOperationException("Discord:DefaultWelcomeChannelIds is not configured.");



            _context = context;
            _socketClient = socketClient;
            _logger = logger;
        }

        public async Task HandleButtonInteractionAsync(SocketMessageComponent component)
        {
            switch (component.Data.CustomId)
            {
                case DiscordCustomIds.MessageApplicantButton:
                    await OpenOfficerMessageModalAsync(component);
                    break;
                case DiscordCustomIds.DeclineButton:
                    await OpenDeclineModalAsync(component);
                    break;
                case DiscordCustomIds.AcceptButton:
                    await OpenAcceptModalAsync(component);
                    break;

                default:
                    _logger.LogWarning("Unknown button {CustomId}", component.Data.CustomId);
                    await component.RespondAsync("Unknown action.", ephemeral: true);
                    break;
            }
        }

        private async Task OpenOfficerMessageModalAsync(SocketMessageComponent component)
        {
            var application = await GetApplicationForOfficerButtonAsync(component);
            if (application is null)
                return;

            await component.RespondWithModalAsync(
                DiscordModals.BuildOfficerMessage(application.PublicId, application.DiscordName));
        }

        private async Task OpenDeclineModalAsync(SocketMessageComponent component)
        {
            var application = await GetApplicationForOfficerButtonAsync(component);
            if (application is null)
                return;

            if (application.Status == ApplicationStatus.Declined)
            {
                await component.RespondAsync("This application has already been declined.", ephemeral: true);
                return;
            }

            await component.RespondWithModalAsync(
                DiscordModals.BuildDeclineApplicant(application.PublicId, application.DiscordName));
        }

        private async Task OpenAcceptModalAsync(SocketMessageComponent component)
        {
            var application = await GetApplicationForOfficerButtonAsync(component);
            if (application is null)
                return;

            if (application.Status == ApplicationStatus.Accepted)
            {
                await component.RespondAsync("This application has already been accepted", ephemeral: true);
                return;
            }

            await component.RespondWithModalAsync(
                DiscordModals.BuildAcceptApplicant(application.PublicId, application.DiscordName));
        }

        private async Task<GuildApplication?> GetApplicationForOfficerButtonAsync(SocketMessageComponent component)
        {
            if (component.User is not SocketGuildUser officer || !IsOfficer(officer))
            {
                _logger.LogWarning("Non-officer {UserId} clicked {CustomId}", component.User.Id, component.Data.CustomId);
                await component.RespondAsync("Only officers can use this.", ephemeral: true);
                return null;
            }

            var threadId = component.ChannelId?.ToString();
            var application = threadId is null
                ? null
                : await _context.GuildApplications
                    .AsNoTracking()
                    .SingleOrDefaultAsync(a => a.DiscordThreadId == threadId);

            if (application is null)
            {
                _logger.LogWarning("No application found for thread {ThreadId}", threadId);
                await component.RespondAsync("Couldn't find the application for this thread. It may have been deleted.", ephemeral: true);
                return null;
            }

            return application;
        }

        public async Task HandleModalSubmittedAsync(SocketModal modal)
        {
            await modal.DeferAsync(ephemeral: true);

            try
            {
                if (modal.User is not SocketGuildUser officer || !IsOfficer(officer))
                {
                    _logger.LogWarning("Non-officer {UserId} submitted {CustomId}", modal.User.Id, modal.Data.CustomId);
                    await modal.FollowupAsync("Only officers can use this.", ephemeral: true);
                    return;
                }

                var parts = modal.Data.CustomId.Split(':', 2);
                if (parts.Length < 2 || !Guid.TryParse(parts[1], out var publicId))
                {
                    _logger.LogWarning("Modal had an invalid custom ID {CustomId}", modal.Data.CustomId);
                    await modal.FollowupAsync("Something went wrong with this form. Please try again.", ephemeral: true);
                    return;
                }

                var application = await _context.GuildApplications
                    .SingleOrDefaultAsync(a => a.PublicId == publicId);
                if (application is null)
                {
                    _logger.LogWarning("Modal submitted for missing application {PublicId}", publicId);
                    await modal.FollowupAsync("Couldn't find this application. It may have been deleted.", ephemeral: true);
                    return;
                }

                switch (parts[0])
                {
                    case OfficerMessageModal.Type:
                        await SendOfficerMessageAsync(modal, officer, application);
                        break;
                    case AcceptModal.Type:
                        await AcceptAsync(modal, officer, application);
                        break;
                    case DeclineModal.Type:
                        await DeclineAsync(modal, officer, application);
                        break;

                    default:
                        _logger.LogWarning("Unknown modal type {CustomId}", modal.Data.CustomId);
                        await modal.FollowupAsync("Unknown action.", ephemeral: true);
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Modal {CustomId} failed", modal.Data.CustomId);
                await modal.FollowupAsync("Something went wrong. Please try again.", ephemeral: true);
            }
        }

        private async Task SendOfficerMessageAsync(SocketModal modal, SocketGuildUser officer, GuildApplication application)
        {
            var text = GetTextValue(modal, OfficerMessageModal.MessageFieldId);
            if (string.IsNullOrWhiteSpace(text))
            {
                await modal.FollowupAsync("The message can't be empty.", ephemeral: true);
                return;
            }

            var delivered = await TryDmApplicantAsync(application, $"**Message from the Clarity officers:**\n{text}\nReply in this DM and I'll send it to your application. If successfully sent, I'll react to the message.");

            await PostToThreadAsync(modal,
                $"✉️ {officer.DisplayName} messaged the applicant{(delivered ? "" : " (DM failed)")}:\n{text}");

            await modal.FollowupAsync(delivered ? "Message sent." : CouldNotDmMessage, ephemeral: true);
        }

        private async Task AcceptAsync(SocketModal modal, SocketGuildUser officer, GuildApplication application)
        {
            if (!ulong.TryParse(application.DiscordId, out var memberId))
            {
                _logger.LogWarning("Unable to get user using {PublicId}", application.PublicId);
                await modal.FollowupAsync("Unable to find user associated with this application.", ephemeral: true);
                return;
            }

            var selectedChannels = modal.Data.Components
                .FirstOrDefault(s => s.CustomId == AcceptModal.ChannelsFieldId)
                ?.Values ?? [];
            var ids = selectedChannels.Count > 0
                ? selectedChannels.Where(id => ulong.TryParse(id, out _)).Select(ulong.Parse)
                : _defaultWelcomeChannelIds;
            var mentions = string.Join(", ", ids.Select(id => $"<#{id}>"));
            var welcomeMessage = GetTextValue(modal, AcceptModal.WelcomeMessageFieldId);

            var template = string.IsNullOrWhiteSpace(welcomeMessage) ? AcceptModal.DefaultWelcomeMessage : welcomeMessage;

            if (application.Status == ApplicationStatus.Accepted)
            {
                await modal.FollowupAsync("This application has already been accepted.", ephemeral: true);
                return;
            }

            if (application.Status == ApplicationStatus.Declined)
            {
                await modal.FollowupAsync("Changed from declined to accepted.", ephemeral: true);
            }

            application.Status = ApplicationStatus.Accepted;
            await _context.SaveChangesAsync();

            var text = template
                .Replace("{member}", $"<@{application.DiscordId}>")
                .Replace("{channels}", mentions);

            var roleAdded = await ApplyTrialRoles(modal, officer, memberId);
            var welcomed = await PostWelcomeToRaiderChatAsync(modal, text, memberId);

            var problems = new List<String>();
            if (!roleAdded) problems.Add("the Trial role couldn't be added");
            if (!welcomed) problems.Add("the welcome message couldn't be posted");

            var summary = problems.Count == 0
                ? "Application accepted: trial role added and welcome posted."
                : $"Application accepted, but {string.Join(" and ", problems)}.";

            await PostToThreadAsync(modal, $"✅ Accepted by {officer.DisplayName}. {summary}");
            await modal.FollowupAsync(summary, ephemeral: true);
        }

        private async Task DeclineAsync(SocketModal modal, SocketGuildUser officer, GuildApplication application)
        {
            var reason = modal.Data.Components
                .FirstOrDefault(c => c.CustomId == DeclineModal.ReasonFieldId)
                ?.Values.FirstOrDefault();
            var note = GetTextValue(modal, DeclineModal.NoteFieldId);

            if (reason is null || !DeclineReasons.OfficerLabels.ContainsKey(reason))
            {
                await modal.FollowupAsync("Please select a reason.", ephemeral: true);
                return;
            }

            if (reason == DeclineReasons.Other && string.IsNullOrWhiteSpace(note))
            {
                await modal.FollowupAsync("Please add a note when the reason is Other.", ephemeral: true);
                return;
            }

            if (application.Status == ApplicationStatus.Declined)
            {
                await modal.FollowupAsync("This application has already been declined.", ephemeral: true);
                return;
            }

            application.Status = ApplicationStatus.Declined;
            await _context.SaveChangesAsync();

            var delivered = await TryDmApplicantAsync(application, BuildDeclineMessage(reason, note));

            var record = new StringBuilder($"❌ Declined by {officer.DisplayName}: {DeclineReasons.OfficerLabels[reason]}");
            if (!string.IsNullOrWhiteSpace(note))
                record.Append($"\nNote: {note}");
            if (!delivered)
                record.Append("\n(DM failed: the applicant hasn't been told.)");
            await PostToThreadAsync(modal, record.ToString());

            await modal.FollowupAsync(
                delivered ? "Application declined and the applicant has been told." : $"Application declined. {CouldNotDmMessage}",
                ephemeral: true);
        }

        private static string BuildDeclineMessage(string reason, string note)
        {
            var message = new StringBuilder(
                "Thank you for your application to Clarity. Unfortunately we've decided not to go ahead with it at this time.");

            if (DeclineReasons.ApplicantText.TryGetValue(reason, out var reasonText))
                message.Append($"\n\n{reasonText}");

            if (!string.IsNullOrWhiteSpace(note))
                message.Append($"\n\n{note}");

            return message.ToString();
        }

        private bool IsOfficer(SocketGuildUser user) =>
            user.Roles.Any(r => r.Id == _officerRoleId);
        private static string GetTextValue(SocketModal modal, string fieldId) =>
            modal.Data.Components
                .FirstOrDefault(c => c.CustomId == fieldId)
                ?.Value?.Trim() ?? "";

        private async Task<bool> TryDmApplicantAsync(GuildApplication application, string text)
        {
            if (!ulong.TryParse(application.DiscordId, out var discordId))
            {
                _logger.LogWarning("Application {PublicId} has an invalid Discord ID", application.PublicId);
                return false;
            }

            var user = await _socketClient.GetUserAsync(discordId);
            if (user is null)
            {
                _logger.LogWarning("Discord user for application {PublicId} not found", application.PublicId);
                return false;
            }

            try
            {
                var dm = await user.CreateDMChannelAsync();
                await dm.SendMessageAsync(text);
                return true;
            }
            catch (HttpException ex) when (ex.DiscordCode == DiscordErrorCode.CannotSendMessageToUser)
            {
                _logger.LogInformation("Couldn't DM applicant for {PublicId}: DMs refused", application.PublicId);
                return false;
            }
        }

        private async Task<bool> PostWelcomeToRaiderChatAsync(SocketModal modal, string text, ulong mentionUser)
        {
            var channel =  await _socketClient.GetChannelAsync(_raidersChatId) as IMessageChannel;
            if (channel is null)
            {
                _logger.LogWarning("Couldn't post welcome message for {CustomId}: channel not available", modal.Data.CustomId);
                return false;
            }

            try
            {
                await channel.SendMessageAsync(text, allowedMentions: new AllowedMentions { UserIds = new List<ulong> { mentionUser} });
                return true;
            }
            catch (HttpException ex) when (ex.DiscordCode is DiscordErrorCode.MissingPermissions or DiscordErrorCode.InsufficientPermissions)
            {
                _logger.LogWarning("Couldn't post welcome to {ChannelId}: {DiscordCode}", _raidersChatId, ex.DiscordCode);
                await modal.FollowupAsync($"Unable to post to Raiders Chat with error code: {ex.DiscordCode}", ephemeral: true);
                return false;
            }
        }

        private async Task<bool> ApplyTrialRoles(SocketModal modal, SocketGuildUser officer, ulong memberId)
        {
            try
            {
                await _socketClient.Rest.AddRoleAsync(officer.Guild.Id, memberId, _trialRoleId);
                return true;
            }
            catch (HttpException ex) when (ex.DiscordCode is DiscordErrorCode.MissingPermissions or DiscordErrorCode.InsufficientPermissions)
            {
                _logger.LogWarning("CCK does not the required permissions to update roles, with error: {DiscordCode}.", ex.DiscordCode);
                await modal.FollowupAsync("CCK is missing the required permissions to update roles. Check that CCK sits above the trial role as well.", ephemeral: true);
                return false;
            }
            catch (HttpException ex) when (ex.DiscordCode is DiscordErrorCode.UnknownMember)
            {
                _logger.LogWarning("Applicant not found on the server, with error: {DiscordCode}.", ex.DiscordCode);
                await modal.FollowupAsync("Applicant not found on the server. Unable to apply roles.", ephemeral: true);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to apply trial role for {MemberId}.", memberId);
                return false;
            }
        }

        private async Task PostToThreadAsync(SocketModal modal, string text)
        {
            var channel = await modal.GetChannelAsync();
            if (channel is null)
            {
                _logger.LogWarning("Couldn't post thread record for {CustomId}: channel not available", modal.Data.CustomId);
                return;
            }

            await channel.SendMessageAsync(text, allowedMentions: AllowedMentions.None);
        }
    }
}