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
        public ApplicantMessagingService(ILogger<ApplicantMessagingService> logger, DiscordSocketClient socketClient, IConfiguration config, ApplicationDbContext context)
        {
            _logger = logger;
            _socketClient = socketClient;
            _context = context;
        }

        public async Task HandleApplicantMessageAsync(IMessage message, IMessageChannel channel)
        {
            if (message.Author.IsBot) return;
            if (message.Channel is not IDMChannel) return;
            if (message is not SocketUserMessage) return;

            string discordUserName = null;

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
                _logger.LogWarning("Unable to find application for user {AuthorId}", authorId);
                await channel.SendMessageAsync("Unable to find user from our application database. Please contact cckraken17 for assistance.");
                return;
            }

            if (!ulong.TryParse(authorId, out var discordUserId))
            {
                _logger.LogWarning("Unable to get {DiscordId} from sender author property", application.DiscordId);
                await channel.SendMessageAsync("Unable to find user using your discord account from our application database. Please contact cckraken17 for assistance.");
                return;
            }

            if (!ulong.TryParse(application.DiscordThreadId, out var discordThreadId))
            {
                _logger.LogWarning("Unable to get {DiscordThreadId}", application.DiscordThreadId);
                await channel.SendMessageAsync("Unable to find application on our server. To apply, please visit www.clarityguild.co.uk.");
                return;
            }

            if (!string.IsNullOrWhiteSpace(application.DiscordName))
            {
                discordUserName = application.DiscordName;
            }

            var applicant = discordUserId;
            var thread = discordThreadId;

            if (string.IsNullOrWhiteSpace(message.Content) && message.Attachments.Count < 1)
            {
                _logger.LogWarning("Empty message sent to CCK from {authorId}", authorId);
                await channel.SendMessageAsync("Did you mean to send any text? Please try again."); //can be done better?
                return;
            }

            if (message.Content.Length > 2000)
            {
                _logger.LogWarning("Message sent exceeds Discords API limit of 2000");
                await channel.SendMessageAsync("Less than 2000 characters, please!");
            }

            //add channel checks if bot is expanded to read dm's from somewhere other than a direct message channel one day

            try
            {

                var applicantDm = message.Content;
                if (message.Attachments.Count > 0)
                {
                    applicantDm += "\n" + string.Join("\n", message.Attachments.Select(s => s.Url));
                }

                var embed = new EmbedBuilder()
                {
                    Title = $"Message received from: {discordUserName}",
                    Color = Color.Blue,
                    Description = message.Content,
                    ImageUrl = message.Attachments.FirstOrDefault()?.Url,
                    Timestamp = DateTime.UtcNow,
                }.Build();

                var threadChannel = await _socketClient.GetChannelAsync(thread) as IThreadChannel;
                try
                {
                    IUserMessage messageSent = await threadChannel.SendMessageAsync(embed: embed, allowedMentions: AllowedMentions.None);
                    await message.AddReactionAsync(new Emoji("✅"));
                }
                catch (HttpException ex) when (ex.DiscordCode is DiscordErrorCode.MissingPermissions or DiscordErrorCode.UnknownChannel)
                {
                    _logger.LogWarning("Failed to send applicant message to thread with error: {DiscordCode}", ex.DiscordCode);
                }
                catch (NullReferenceException ex)
                {
                    _logger.LogWarning("Thread does not exist: {thread} with message: {ex.Message}", thread, ex.Message);
                    await channel.SendMessageAsync("Unable to find application thread");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("Failed to send applicant message: {ex.Message}", ex.Message);

                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Failed to send applicant message: {ex.Message}", ex.Message);

            }
        }
    }
}

