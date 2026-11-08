using Discord;

namespace ClarityGuildWebsite.Api.Features.Discord.Messaging
{
    public interface IApplicantMessagingService
    {
        Task HandleApplicantMessageAsync(IMessage message);
    }
}
