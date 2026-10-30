using System.Text;

namespace ClarityGuildWebsite.Api.Helpers
{
    public static class DiscordMessageChunker
    {
        // Splits lines into messages under Discord's 2,000-character limit, without breaking a line (URL) in half.
        public static List<string> ChunkLines(IEnumerable<string> lines, int maxLength = 2000)
        {
            var chunks = new List<string>();
            var current = new StringBuilder();

            foreach (var line in lines)
            {
                if (current.Length > 0 && current.Length + line.Length + 1 > maxLength)
                {
                    chunks.Add(current.ToString());
                    current.Clear();
                }
                current.AppendLine(line).Append('\n');
            }

            if (current.Length > 0) chunks.Add(current.ToString());
            return chunks;
        }
    }
}
