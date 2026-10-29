using Discord;

namespace ClarityGuildWebsite.Api.Services.DiscordBotInteractions
{
    public interface IApplicantMessagingService
    {
        Task HandleApplicantMessageAsync(IMessage message, IMessageChannel channel);
    }
}
