namespace ClarityGuildWebsite.Api.Services
{
    public record NotificationResult(bool Success, string? MessageId, string? ThreadId, string? Error)
    {
        public static NotificationResult Delivered(string messageId, string? threadId) =>
            new(true, messageId, threadId, null);

        public static NotificationResult Failed(string error) =>
            new(false, null, null, error);
    }
}
