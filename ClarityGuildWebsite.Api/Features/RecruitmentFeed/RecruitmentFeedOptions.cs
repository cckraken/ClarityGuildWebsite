using System.ComponentModel.DataAnnotations;

namespace ClarityGuildWebsite.Api.Features.RecruitmentFeed
{
    public sealed class RecruitmentFeedOptions
    {
        public const string SectionName = "RecruitmentFeed";

        public ulong IncomingChannelId { get; init; }
        public ulong OutgoingChannelId { get; init; }

        [MinLength(1)]
        public ulong[] AllowedWebhookIds { get; init; } = [];
    }
}
