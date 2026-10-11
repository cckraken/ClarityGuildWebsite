namespace ClarityGuildWebsite.Api.Helpers
{
    public class ApplicationHelpers
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
    }
}
