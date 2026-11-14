namespace ClarityGuildWebsite.Api.Features.Integrations.WarcraftLogs
{
    public static class WarcraftLogsServiceCollectionExtensions
    {
        public static IServiceCollection AddWarcraftLogs(this IServiceCollection services, IConfiguration config)
        {
            services.AddOptions<WarcaftLogsOptions>()
                .Bind(config.GetSection(WarcaftLogsOptions.SectionName))
                .ValidateDataAnnotations()
                .Validate(o => !string.IsNullOrWhiteSpace(o.ClientId), "WarcraftLogs:ClientId is not configured.")
                .Validate(o => !string.IsNullOrWhiteSpace(o.ClientSecret), "WarcraftLogs:ClientSecret is not configured.")
                .Validate(o => o.Bosses.All(b => b.Id > 0 && !string.IsNullOrWhiteSpace(b.Name)), "WarcraftLogs:Bosses is not configured.")
                .ValidateOnStart();

            services.AddSingleton<WarcraftLogsClient>();
            return services;
        }
    }
}
