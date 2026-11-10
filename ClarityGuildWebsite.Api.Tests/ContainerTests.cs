using ClarityGuildWebsite.Api.Features.DiscordBot.Interactions;
using ClarityGuildWebsite.Api.Features.DiscordBot.Messaging;
using ClarityGuildWebsite.Api.Features.DiscordBot.SlashCommands;
using ClarityGuildWebsite.Api.Features.Notifications;
using ClarityGuildWebsite.Api.Features.RecruitmentFeed;
using Microsoft.Extensions.DependencyInjection;

namespace ClarityGuildWebsite.Api.Tests
{
    public class ContainerTests : IClassFixture<ProductionConfigFactory>
    {
        private readonly ProductionConfigFactory _factory;
        public ContainerTests(ProductionConfigFactory factory) => _factory = factory;
        // Add every type the gateway resolves in RunInBackground here
        [Theory]
        [InlineData(typeof(DiscordInteractionRouter))]
        [InlineData(typeof(DiscordSlashCommandsRouter))]
        [InlineData(typeof(DiscordSlashCommands))]
        [InlineData(typeof(IApplicantMessagingService))]
        [InlineData(typeof(RecruitmentFeedHandler))]
        [InlineData(typeof(IApplicationNotifier))]
        public void Every_gateway_handler_can_be_created_with_production_config(Type type)
        {
            using var scope = _factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService(type);   // Throws if it can't be created. Running dotnet test before any push/publish etc ensures no dependencies are missing
            Assert.NotNull(service);
        }
    }
}
