using ClarityGuildWebsite.Api.Models.Applications;

namespace ClarityGuildWebsite.Api.Services
{
    public interface IApplicationDiscordPostingService
    {
        Task PostAppToDiscord(GuildApplication application, CancellationToken ctoken);
    }
}
