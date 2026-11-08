using ClarityGuildWebsite.Api.Models.Feeds;
using ClarityGuildWebsite.Api.Models.Feeds.WarcraftLogs;
using ClarityGuildWebsite.Api.Services.DiscordBotInteractions.Feeds;
using Discord;

namespace ClarityGuildWebsite.Api.Tests;

public class WarcraftLogsEmbedParserTests
{
    private const string ValidUrl = "https://www.warcraftlogs.com/character/id/123456?metric=dps&spec=Arcane";
    private const string ValidTitle = "Testchar (Mage - Horde - Kazzak - EU)";
    private const string ValidFooter = "5/8 Mythic (The Venomous Abyss)";

    private static Embed BuildEmbed(string? url = ValidUrl, string? title = ValidTitle, string? footer = ValidFooter)
    {
        var builder = new EmbedBuilder().WithDescription("**Contact:** testuser");
        if (url is not null) builder.WithUrl(url);
        if (title is not null) builder.WithTitle(title);
        if (footer is not null) builder.WithFooter(footer);
        return builder.Build();
    }

    [Fact]
    public void Valid_embed_is_parsed()
    {
        var result = WarcraftLogsEmbedParser.TryParse(BuildEmbed());

        Assert.NotNull(result);
        Assert.Equal(123456, result.WarcraftLogsId);
        Assert.Equal("Testchar", result.Name);
        Assert.Equal("Mage", result.ClassName);
        Assert.Equal("Kazzak", result.RealmName);
        Assert.Equal("EU", result.Region);
        Assert.Equal("Arcane", result.Spec);
        Assert.Equal("dps", result.Metric);
        Assert.Equal(new RaidProgressLine(5, 8, "Mythic", "The Venomous Abyss"), result.Progress);
    }

    [Fact]
    public void Spaces_in_class_and_realm_are_kept()
    {
        var result = WarcraftLogsEmbedParser.TryParse(BuildEmbed(title: "Testchar (Death Knight - Alliance - Twisting Nether - EU)"));

        Assert.NotNull(result);
        Assert.Equal("Death Knight", result.ClassName);
        Assert.Equal("Twisting Nether", result.RealmName);
    }

    [Fact]
    public void Missing_spec_and_metric_are_null_not_rejected()
    {
        var result = WarcraftLogsEmbedParser.TryParse(BuildEmbed(url: "https://www.warcraftlogs.com/character/id/123456"));

        Assert.NotNull(result);
        Assert.Null(result.Spec);
        Assert.Null(result.Metric);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("Heroic")]
    [InlineData("9/8 Mythic (The Venomous Abyss)")]
    [InlineData("5/8 LFR (The Venomous Abyss)")]
    public void Unexpected_footer_gives_null_progress_but_still_parses(string? footer)
    {
        var result = WarcraftLogsEmbedParser.TryParse(BuildEmbed(footer: footer));

        Assert.NotNull(result);
        Assert.Null(result.Progress);
    }

    [Theory]
    [InlineData("https://www.warcraftlogs.com.evil.example/character/id/123456", ValidTitle)]   // look-alike host
    [InlineData("http://www.warcraftlogs.com/character/id/123456", ValidTitle)]               // not https
    [InlineData("https://www.warcraftlogs.com/character/eu/kazzak/testchar", ValidTitle)]      // different link format
    [InlineData("https://www.warcraftlogs.com/character/id/abc", ValidTitle)]                  // non-numeric ID
    [InlineData(ValidUrl, "Testchar (Mage - Horde - Kazzak)")]                                 // 3 parts
    [InlineData(ValidUrl, "Testchar Mage - Horde - Kazzak - EU")]                              // no brackets
    [InlineData(ValidUrl, "(Mage - Horde - Kazzak - EU)")]                                     // no name
    [InlineData(ValidUrl, null)]                                                               // no title
    [InlineData(null, ValidTitle)]                                                             // no link
    public void Unexpected_embeds_are_rejected(string? url, string? title)
    {
        Assert.Null(WarcraftLogsEmbedParser.TryParse(BuildEmbed(url: url, title: title)));
    }
}