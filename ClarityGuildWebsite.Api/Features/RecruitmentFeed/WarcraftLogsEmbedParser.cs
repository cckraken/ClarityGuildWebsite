using ClarityGuildWebsite.Api.Features.RecruitmentFeed;
using Discord;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;

namespace ClarityGuildWebsite.Api.Features.Integrations.WarcraftLogs.Processing;

/// <summary>
/// Parses the embeds Warcraft Logs recruitment webhook posts.
/// Returns null for anything not in the expected format.
/// </summary>
public static partial class WarcraftLogsEmbedParser
{
    private const string WarcraftLogsHost = "www.warcraftlogs.com";
    public static WarcraftLogsCandidate? TryParse(IEmbed embed)
    {
        if (!Uri.TryCreate(embed.Url, UriKind.Absolute, out var url)) return null;
        if (url.Scheme != Uri.UriSchemeHttps || url.Host != WarcraftLogsHost) return null;

        var segments = url.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length != 3 || segments[0] != "character" || segments[1] != "id") return null;
        if (!long.TryParse(segments[2], NumberStyles.None, CultureInfo.InvariantCulture, out var id) || id <= 0) return null;

        var title = embed.Title;
        if (string.IsNullOrWhiteSpace(title)) return null;

        var open = title.IndexOf('(');
        if (open < 1 || !title.EndsWith(')')) return null;

        var name = title[..open].Trim();
        var parts = title[(open + 1)..^1].Split(" - ", StringSplitOptions.TrimEntries);
        if (parts.Length != 4 || parts.Any(string.IsNullOrWhiteSpace)) return null;

        // Protect against future wlogs URL changes? Highly unlikely, but worth doing
        var className = parts[0];
        var realmName = parts[2];
        var region = parts[3];

        var query = HttpUtility.ParseQueryString(url.Query);
        var spec = string.IsNullOrWhiteSpace(query["spec"]) ? null : query["spec"];
        var metric = query["metric"]?.ToLowerInvariant() switch
        {
            "dps" => "dps",
            "hps" => "hps",
            _ => null
        };

        var progress = ParseProgress(embed.Footer?.Text);

        return new WarcraftLogsCandidate(id, name, className, realmName, region, spec, metric, progress);
    }

    private static RaidProgressLine? ParseProgress(string? footer)
    {
        if (string.IsNullOrWhiteSpace(footer)) return null;

        var match = ProgressPattern().Match(footer.Trim());
        if (!match.Success) return null;

        var kills = int.Parse(match.Groups["kills"].Value, CultureInfo.InvariantCulture);
        var total = int.Parse(match.Groups["total"].Value, CultureInfo.InvariantCulture);
        if (total == 0 || kills > total) return null;

        return new RaidProgressLine(kills, total, match.Groups["difficulty"].Value, match.Groups["raid"].Value);
    }

    [GeneratedRegex(@"^(?<kills>\d{1,2})/(?<total>\d{1,2})\s+(?<difficulty>Normal|Heroic|Mythic)\s+\((?<raid>[^)]+)\)$")]
    private static partial Regex ProgressPattern();
}