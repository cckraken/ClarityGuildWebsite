using ClarityGuildWebsite.Api.Data;
using ClarityGuildWebsite.Api.Helpers;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;

namespace ClarityGuildWebsite.Api.Services.DiscordBotInteractions
{
    public class ApplicantMessagingService : IApplicantMessagingService
    {
        private readonly DiscordSocketClient _socketClient;
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;
        private const string UnknownErrorMsg = "Sorry, I couldn’t pass that on. Please contact a member of the Recruitment Team.";
        public ApplicantMessagingService(ILogger<ApplicantMessagingService> logger, DiscordSocketClient socketClient, ApplicationDbContext context)
        {
            _logger = logger;
            _socketClient = socketClient;
            _context = context;
        }

        public async Task HandleApplicantMessageAsync(IMessage message)
        {
            if (message.Author.IsBot) return;
            if (message.Channel is not IDMChannel) return;
            if (message is not SocketUserMessage) return;

            var channel = message.Channel;

            // Route the reply to the application with the most recent activity: the last recruitment team member DM, or submission if never messaged
            // Applicants can have several applications (re-applying after a decline), so "newest" alone could pick the wrong thread
            // Very unlikely scenario given the 3 month auto deletion, but worth covering
            var authorId = message.Author.Id.ToString();
            var application = await _context.GuildApplications
                .Where(s => s.DiscordId == authorId && s.DiscordThreadId != null)
                .OrderByDescending(s => s.LastRecruitmentTeamMessagedUtc ?? s.TimestampUtc)
                .Select(s => new
                {
                    s.DiscordThreadId
                })
                .FirstOrDefaultAsync();

            if (application?.DiscordThreadId == null)
            {
                _logger.LogInformation("DM received from a user with no application");
                await channel.SendMessageAsync("I don’t have an application from you. You can apply at www.clarityguild.co.uk");
                return;
            }

            if (!ulong.TryParse(application.DiscordThreadId, out var discordThreadId))
            {
                _logger.LogWarning("Unable to get {DiscordThreadId}", application.DiscordThreadId);
                await channel.SendMessageAsync(UnknownErrorMsg);
                return;
            }

            if (string.IsNullOrWhiteSpace(message.Content) && message.Attachments.Count < 1)
            {
                _logger.LogInformation("Empty message sent to CCK");
                await channel.SendMessageAsync("Did you mean to send any text? Please try again.");
                return;
            }

            try
            {
                var embed = new EmbedBuilder()
                {
                    Title = $"Message received from: {message.Author.Username}",
                    Color = Color.Blue,
                    Description = string.IsNullOrWhiteSpace(message.Content) ? "(sent attachments)" : message.Content,
                    ImageUrl = message.Attachments.FirstOrDefault(a => a.ContentType?.StartsWith("image/") == true)?.Url,
                    Timestamp = message.Timestamp
                }.Build();

                var threadChannel = await _socketClient.GetChannelAsync(discordThreadId) as IThreadChannel;
                if (threadChannel is null)
                {
                    _logger.LogWarning("Thread not found: {ThreadId}", discordThreadId);
                    await channel.SendMessageAsync(UnknownErrorMsg);
                    return;
                }
                try
                {
                    await threadChannel.SendMessageAsync(embed: embed, allowedMentions: AllowedMentions.None);
                    foreach (var chunk in DiscordMessageChunker.ChunkLines(message.Attachments.Select(a => a.Url)))
                    {
                        await threadChannel.SendMessageAsync(chunk, allowedMentions: AllowedMentions.None);
                    }
                    
                    try
                    {
                        await message.AddReactionAsync(new Emoji("✅"));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Applicant message {MessageId} was relayed, but its confirmation reaction failed", message.Id);
                    }
                }
                catch (HttpException ex) when (ex.DiscordCode is DiscordErrorCode.MissingPermissions or DiscordErrorCode.UnknownChannel)
                {
                    _logger.LogWarning("Failed to send applicant message to thread with error: {DiscordCode}", ex.DiscordCode);
                    await channel.SendMessageAsync(UnknownErrorMsg);
                }
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to relay applicant message");
                await channel.SendMessageAsync(UnknownErrorMsg);

            }
        }
    }
}

