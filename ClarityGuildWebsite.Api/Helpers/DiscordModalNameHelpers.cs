using Discord.WebSocket;
using ClarityGuildWebsite.Api.Services.DiscordBotInteractions;
namespace ClarityGuildWebsite.Api.Helpers
{

    public static class DiscordModalNameHelpers
    {
        public static string BuildModalTitle(SocketMessageComponent component, string verb)
        {
            if (component.Channel is SocketThreadChannel thread
                && thread.Name.StartsWith(DiscordNames.ApplicationThreadPrefix, StringComparison.Ordinal))
            {
                var name = thread.Name[DiscordNames.ApplicationThreadPrefix.Length..].Trim();
                if (string.IsNullOrWhiteSpace(name)) return $"{verb} applicant";

                var title = $"{verb} {name}";
                return title.Length <= 45 ? title : $"{verb} applicant";
            }

            return $"{verb} applicant";
        }

    }
}