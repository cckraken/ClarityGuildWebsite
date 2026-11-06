namespace ClarityGuildWebsite.Api.Models.Feeds.WarcraftLogs.GQLQueries
{
    public static class Queries
    {
        public const string FirstKillsData = """
query FirstKills($id: Int!, $difficulty: Int!, $metric: CharacterRankingMetricType) {
  characterData {
    character(id: $id) {
      name
      e3470: encounterRankings(encounterID: 3470, difficulty: $difficulty, metric: $metric, partition: -1)
      e3445: encounterRankings(encounterID: 3445, difficulty: $difficulty, metric: $metric, partition: -1)
      e3455: encounterRankings(encounterID: 3455, difficulty: $difficulty, metric: $metric, partition: -1)
      e3497: encounterRankings(encounterID: 3497, difficulty: $difficulty, metric: $metric, partition: -1)
      e3420: encounterRankings(encounterID: 3420, difficulty: $difficulty, metric: $metric, partition: -1)
      e3421: encounterRankings(encounterID: 3421, difficulty: $difficulty, metric: $metric, partition: -1)
      e3429: encounterRankings(encounterID: 3429, difficulty: $difficulty, metric: $metric, partition: -1)
      e3492: encounterRankings(encounterID: 3492, difficulty: $difficulty, metric: $metric, partition: -1)
    }
  }
}
""";

    }
}
