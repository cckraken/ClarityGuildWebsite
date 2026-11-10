using ClarityGuildWebsite.Api.Models.Applications;

namespace ClarityGuildWebsite.Api.Features.Applications
{
    public interface IApplicationDiscordPostingService
    {
        Task PostAppToDiscord(GuildApplication application, CancellationToken ctoken);
    }
}