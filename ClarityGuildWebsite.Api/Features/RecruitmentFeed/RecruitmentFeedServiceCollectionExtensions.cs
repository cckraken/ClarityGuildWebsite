namespace ClarityGuildWebsite.Api.Features.RecruitmentFeed
{
    public static class RecruitmentFeedServiceCollectionExtensions
    {
        public static IServiceCollection AddRecruitmentFeed(this IServiceCollection services, IConfiguration config)
        {
            services.AddOptions<RecruitmentFeedOptions>()
            .Bind(config.GetSection(RecruitmentFeedOptions.SectionName))
             .ValidateDataAnnotations()
             .Validate(o => o.IncomingChannelId != 0, "RecruitmentFeed:IncomingChannelId is not configured.")
             .Validate(o => o.OutgoingChannelId != 0, "RecruitmentFeed:OutgoingChannelId is not configured.")
             .Validate(o => o.AllowedWebhookIds is { Length: > 0 }, "RecruitmentFeed:AllowedWebhookIds is not configured.")
             .ValidateOnStart();

            services.AddScoped<RecruitmentFeedHandler>();
            return services;
        }
    }
}
