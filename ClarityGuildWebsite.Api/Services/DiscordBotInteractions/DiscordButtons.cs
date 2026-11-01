using Discord;

namespace ClarityGuildWebsite.Api.Services.DiscordBotInteractions
{
    public static class DiscordButtons
    {
        public static MessageComponent BuildApplicationButtons() =>
            new ComponentBuilder()
                .WithButton("Message Applicant", DiscordCustomIds.MessageApplicantButton, ButtonStyle.Primary, row: 0)
                .WithButton("Accept", DiscordCustomIds.AcceptButton, ButtonStyle.Success, row: 1)
                .WithButton("Decline", DiscordCustomIds.DeclineButton, ButtonStyle.Danger, row: 1)
                .Build();
    }
}
