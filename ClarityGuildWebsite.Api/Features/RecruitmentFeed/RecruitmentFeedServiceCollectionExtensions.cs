namespace ClarityGuildWebsite.Api.Features.RecruitmentFeed
{
    public static class RecruitmentFeedServiceCollectionExtensions
    {
        public static IServiceCollection AddRecruitmentFeed (this IServiceCollection services, IConfiguration config)
        {
            services.AddOptions<RecruitmentFeedOptions>()
            .Bind(config.GetSection(RecruitmentFeedOptions.SectionName))
            .ValidateDataAnnotations()
            .Validate(o => o.IncomingChannelId != 0, "RecruitmentFeed:IncomingChannelId is not configured.")
            .ValidateOnStart();

            services.AddScoped<RecruitmentFeedHandler>();
            return services;
        }
    }
}
