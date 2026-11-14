namespace ClarityGuildWebsite.Api.Features.Integrations.WarcraftLogs
{
    public static class WarcraftLogsServiceCollectionExtensions
    {
        public static IServiceCollection AddWarcraftLogs(this IServiceCollection services, IConfiguration config)
        {
            services.AddOptions<WarcraftLogsOptions>()
                .Bind(config.GetSection(WarcraftLogsOptions.SectionName))
                .Validate(o => !string.IsNullOrWhiteSpace(o.ClientId), "WarcraftLogs:ClientId is not configured.")
                .Validate(o => !string.IsNullOrWhiteSpace(o.ClientSecret), "WarcraftLogs:ClientSecret is not configured.")
                .Validate(o => o.Bosses is { Length: > 0 }, "WarcraftLogs:Bosses is not configured.")
                .Validate(o => o.Bosses.All(b => b.Id > 0 && !string.IsNullOrWhiteSpace(b.Name)),
                    "WarcraftLogs:Bosses contains an entry without a valid Id or Name.")
                .ValidateOnStart();

            services.AddHttpClient(WarcraftLogsOptions.SectionName, c => c.Timeout = TimeSpan.FromSeconds(10));
            services.AddHttpClient<WarcraftLogsClient>(c =>
            {
                c.BaseAddress = new Uri("https://www.warcraftlogs.com/api/v2/");
                c.Timeout = TimeSpan.FromSeconds(10);
            });
            services.AddSingleton<WarcraftLogsTokenService>();
            return services;
        }
    }
}
