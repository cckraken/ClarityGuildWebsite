using Discord.WebSocket;

public static class DiscordModalNameHelpers
{
    public static string DiscordNameTitle(SocketMessageComponent component, string verb)
    {
        if (component.Channel is SocketThreadChannel thread
            && thread.Name.StartsWith("New Application -", StringComparison.OrdinalIgnoreCase))
        {
            var name = thread.Name[DiscordNames.ApplicationThreadPrefix.Length..].Trim();
            var title = $"{verb} {name}";
            return title.Length <= 45 ? title : $"{name} applicant";
        }

        return $"{verb} applicant"; ;
    }

    public static class DiscordNames
    {
        public const string ApplicationThreadPrefix = "New Application - ";
    }

}
