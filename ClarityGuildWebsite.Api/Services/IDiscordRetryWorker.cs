using Azure.Identity;

namespace ClarityGuildWebsite.Api.Services
{
    public interface IDiscordRetryWorker : IHostedService
    {
        public record RetryObject(string PublicId, DateTime LastAttemptedUtc, int PostStatus, int Retry);
    }
}
