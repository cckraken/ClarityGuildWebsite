namespace ClarityGuildWebsite.Api.Helpers
{
    public static class ApplicationHelpers
    {
        public static string Truncate(string? text, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text ?? string.Empty;

            if (text.Length <= maxLength)
                return text;

            return text.Substring(0, maxLength - 1) + "…";
        }

        public static string Combine(string name, string realm, string role)
        {
            return $"{name} - {realm} ({role})";
        }

        public static (string wlogs, string rio) LinkBuilder(string name, string realm)
        {
            const string error = "Unable to generate link";

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(realm))
                return (error, error);

            const string wlogsBase = "https://www.warcraftlogs.com/character/eu";
            const string rioBase = "https://raider.io/characters/eu";
            string escapedName = Uri.EscapeDataString(name.Trim());
            string realmSlug = Uri.EscapeDataString(ToRealmSlug(realm));      

            var wlogsLink = $"{wlogsBase}/{realmSlug}/{escapedName}";
            var rioLink = $"{rioBase}/{realmSlug}/{escapedName}";

            return (wlogsLink, rioLink);
        }

        public static string ToRealmSlug(string realm)
        {
            return realm.Trim().ToLowerInvariant().Replace(" ", "-");
        }
    }
}
