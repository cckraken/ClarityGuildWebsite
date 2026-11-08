using ClarityGuildWebsite.Api.Models.Applications;

namespace ClarityGuildWebsite.Api.Features.Notifications
{
    public interface IApplicationNotifier
    {
        Task<NotificationResult> PostAndRecordAsync(GuildApplication application, CancellationToken ctoken);
    }
}
