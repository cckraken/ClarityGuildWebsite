using ClarityGuildWebsite.Api.Models;

namespace ClarityGuildWebsite.Api.Services
{
    public interface IApplicationDiscordPostingService
    {
        Task PostAppToDiscord(GuildApplication application, CancellationToken ctoken);
    }
}
