using ClarityGuildWebsite.Api.Data;
using ClarityGuildWebsite.Api.Helpers;
using ClarityGuildWebsite.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ClarityGuildWebsite.Api.Services
{
    public class ApplicationDiscordPostingService : IApplicationDiscordPostingService
    {
        private readonly IApplicationNotifier _notifier;
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _context;

        public ApplicationDiscordPostingService(IApplicationNotifier notifier, ILogger<ApplicationDiscordPostingService> logger, ApplicationDbContext context)
        {
            _notifier = notifier;
            _logger = logger;
            _context = context;
        }
        public async Task PostAppToDiscord(GuildApplication application, CancellationToken ctoken)
        {
            var result =  await _notifier.PostAndRecordAsync(application, ctoken);
            application.LastAttemptedUtc = DateTime.UtcNow;
            if (result.Success)
            {
                application.PostStatus = PostStatus.Delivered;
                application.DiscordMessageId = result.MessageId;
                application.LastPostError = null;
                _logger.LogInformation("Post successful for {PublicId} : {DiscordMessageId}", application.PublicId, application.DiscordMessageId);
            }
            else
            {
                application.PostStatus = PostStatus.Failed;
                application.LastPostError = string.IsNullOrWhiteSpace(result.Error) ? "Unknown Error" : ApplicationHelpers.Truncate(result.Error, 250);
                _logger.LogWarning("Post failed for {PublicId}: {Error}", application.PublicId, result.Error);
            }
            await _context.SaveChangesAsync();
        }
    }
}
