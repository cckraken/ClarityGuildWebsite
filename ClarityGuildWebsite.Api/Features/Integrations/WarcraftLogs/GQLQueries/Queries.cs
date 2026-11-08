namespace ClarityGuildWebsite.Api.Features.Integrations.WarcraftLogs.GQLQueries
{
    public static class Queries
    {
        public static string FirstKills(IEnumerable<BossConfig> bosses)
        {
            var fields = string.Join("\n", bosses.Select(b =>
                $"e{b.Id}: encounterRankings(encounterID: {b.Id}, difficulty: $difficulty, metric: $metric, partition: -1)"));

            return $$"""
            query FirstKills($id: Int!, $difficulty: Int!, $metric: CharacterRankingMetricType) {
              characterData {
                character(id: $id) {
                  name
                  {{fields}}
                }
              }
            }
            """;
        }
    }
}
