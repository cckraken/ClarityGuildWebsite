using ClarityGuildWebsite.Api.Models;
using Discord;

namespace ClarityGuildWebsite.Api.Services
{
    public interface IApplicationNotifier
    {
        Task<NotificationResult> PostAndRecordAsync(GuildApplication application, CancellationToken ctoken);
    }
}
