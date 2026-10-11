using ClarityGuildWebsite.Api.Models;

namespace ClarityGuildWebsite.Api.Services
{
    public interface IApplicationNotifier
    {
        Task<NotificationResult> NotifyNewApplicationAsync(GuildApplication application, CancellationToken ctoken);
    }
}
