using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace ClarityGuildWebsite.Api.Tests
{
    public sealed class ProductionConfigFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Load appsettings.json the way production does: no Development overrides.
            builder.UseEnvironment(Environments.Production);

            // Secrets and per-environment values that production gets from App Service.
            // UseSetting (not AddInMemoryCollection), because Program.cs reads some of these immediately.
            builder.UseSetting("Discord:BotToken", "fake-token");
            builder.UseSetting("Discord:ClientSecret", "fake-secret");
            builder.UseSetting("Discord:WebhookUrl", "https://example.invalid/webhook");
            builder.UseSetting("WarcraftLogs:ClientId", "fake-id");
            builder.UseSetting("WarcraftLogs:ClientSecret", "fake-secret");
            builder.UseSetting("ConnectionStrings:ClarityDb", "Server=fake;Database=fake;");
            builder.UseSetting("Cors:AllowedOrigins:0", "https://test.example");

            // Turn on the strict checks production normally has off.
            builder.UseDefaultServiceProvider(o => { o.ValidateOnBuild = true; o.ValidateScopes = true; });
            // Don't start the background services: the gateway would try to log in to Discord,
            // the workers would hit the database.
            builder.ConfigureTestServices(services => services.RemoveAll<IHostedService>());
        }
    }
}
