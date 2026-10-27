using ClarityGuildWebsite.Api.Data;
using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;

namespace ClarityGuildWebsite.Api.Services.DiscordBotInteractions
{
    public class DiscordGatewayService : IHostedService
    {
        private readonly string _token;
        private readonly DiscordSocketClient _socketClient;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger _logger;

        public DiscordGatewayService(IConfiguration config, DiscordSocketClient client, IServiceScopeFactory scopeFactory, ILogger<DiscordGatewayService> logger)
        {
            _token = config.GetValue<string>("Discord:BotToken");
            if (string.IsNullOrWhiteSpace(_token))
                throw new InvalidOperationException("Discord:BotToken is missing or invalid.");

            _socketClient = client;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken ctoken)
        {
            _socketClient.Log += OnLogAsync;
            _socketClient.ButtonExecuted += OnButtonExecutedAsync;
            _socketClient.ModalSubmitted += OnModalSubmittedAsync;

            await _socketClient.LoginAsync(TokenType.Bot, _token);
            await _socketClient.StartAsync();
        }

        public async Task StopAsync(CancellationToken ctoken)
        {
            _socketClient.ButtonExecuted -= OnButtonExecutedAsync;
            _socketClient.Log -= OnLogAsync;

            await _socketClient.StopAsync();
            await _socketClient.LogoutAsync();
        }

        private async Task OnButtonExecutedAsync(SocketMessageComponent component)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var router = scope.ServiceProvider.GetRequiredService<DiscordInteractionRouter>();
                await router.HandleButtonInteractionAsync(component);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Button {CustomId} failed", component.Data.CustomId);
            }
        }

        private async Task OnModalSubmittedAsync(SocketModal modal)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var router = scope.ServiceProvider.GetRequiredService<DiscordInteractionRouter>();
                await router.HandleModalSubmitted(modal, modal.User);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Modal {CustomId} failed. Intended recipient does not share a server with you or is not accepting DMs", modal.Data.CustomId);
            }
        }

        private Task OnLogAsync(LogMessage msg)
        {
            _logger.LogError(msg.Exception, "Discord log: {Source} - {Message}", msg.Source, msg.Message);
            return Task.CompletedTask;
        }
    }
}
