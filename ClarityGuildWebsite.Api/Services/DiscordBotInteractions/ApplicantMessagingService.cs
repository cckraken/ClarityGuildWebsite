using ClarityGuildWebsite.Api.Data;
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
        private const string UnknownErrorMsg = "Sorry, I couldn’t pass that on. Please contact an officer.";
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

            var authorId = message.Author.Id.ToString();
            var application = await _context.GuildApplications
                .AsNoTracking()
                .Where(s => s.DiscordId == authorId && s.DiscordId != null)
                .OrderByDescending(s => s.TimestampUtc)
                .Select(s => new
                {
                    s.DiscordId,
                    s.DiscordThreadId,
                    s.DiscordName
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

                var applicantDm = message.Content;
                if (message.Attachments.Count > 0)
                {
                    applicantDm += "\n" + string.Join("\n", message.Attachments.Select(s => s.Url));
                }

                var embed = new EmbedBuilder()
                {
                    Title = $"Message received from: {message.Author.Username}",
                    Color = Color.Blue,
                    Description = applicantDm,
                    ImageUrl = message.Attachments.FirstOrDefault()?.Url,
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
                    await message.AddReactionAsync(new Emoji("✅"));
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

