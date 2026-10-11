namespace ClarityGuildWebsite.Api.Services
{
    public record DiscordWebhookPayload(List<DiscordEmbed> Embeds);
    public record DiscordEmbed(string Title, List<DiscordEmbedField> Fields);

    public record DiscordEmbedField(string Name, string Value, bool Inline = false);

    public record DiscordMessageResponse(string Id);
}