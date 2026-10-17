namespace ClarityGuildWebsite.Api.Services
{
    public record DiscordPayload(List<DiscordEmbed> Embeds);
    public record DiscordEmbed(string Title, List<DiscordEmbedField> Fields, int? Color, DateTimeOffset? Timestamp, DiscordEmbedFooter Footer);

    public record DiscordEmbedField(string Name, string Value, bool Inline = false);

    public record DiscordEmbedFooter(string Text);

    public record DiscordMessageResponse(string Id);
}