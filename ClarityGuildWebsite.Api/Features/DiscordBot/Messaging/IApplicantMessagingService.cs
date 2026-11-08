using Discord;

namespace ClarityGuildWebsite.Api.Features.DiscordBot.Messaging
{
    public interface IApplicantMessagingService
    {
        Task HandleApplicantMessageAsync(IMessage message);
    }
}
