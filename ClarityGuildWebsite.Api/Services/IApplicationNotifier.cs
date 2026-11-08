using ClarityGuildWebsite.Api.Models.Applications;

namespace ClarityGuildWebsite.Api.Services
{
    public interface IApplicationNotifier
    {
        Task<NotificationResult> PostAndRecordAsync(GuildApplication application, CancellationToken ctoken);
    }
}
