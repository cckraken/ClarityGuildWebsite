using Discord;
using Discord.WebSocket;


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
            var token = config.GetValue<string>("Discord:BotToken");
            if (string.IsNullOrWhiteSpace(token))
                throw new InvalidOperationException("Discord:BotToken is missing or invalid.");

            _socketClient = client;
            _scopeFactory = scopeFactory;
            _logger = logger;
            _token = token;
        }

        public async Task StartAsync(CancellationToken ctoken)
        {
            _socketClient.Log += OnLogAsync;
            _socketClient.ButtonExecuted += OnButtonExecutedAsync;
            _socketClient.ModalSubmitted += OnModalSubmittedAsync;
            _socketClient.MessageReceived += OnApplicantMessageReceivedAsync;

            await _socketClient.LoginAsync(TokenType.Bot, _token);
            await _socketClient.StartAsync();
        }

        public async Task StopAsync(CancellationToken ctoken)
        {
            _socketClient.ButtonExecuted -= OnButtonExecutedAsync;
            _socketClient.ModalSubmitted -= OnModalSubmittedAsync;
            _socketClient.MessageReceived -= OnApplicantMessageReceivedAsync;
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
                await router.HandleModalSubmittedAsync(modal);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Modal {CustomId} failed. Intended recipient does not share a server with you or is not accepting DMs", modal.Data.CustomId);
            }
        }

        private async Task OnApplicantMessageReceivedAsync(IMessage message)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IApplicantMessagingService>();
                await service.HandleApplicantMessageAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured when trying to send a message to an applicants thread.");
            }
        }

        private Task OnLogAsync(LogMessage msg)
        {
            var level = msg.Severity switch
            {
                LogSeverity.Critical => LogLevel.Critical,
                LogSeverity.Error => LogLevel.Error,
                LogSeverity.Warning => LogLevel.Warning,
                LogSeverity.Info => LogLevel.Information,
                LogSeverity.Verbose => LogLevel.Debug,
                LogSeverity.Debug => LogLevel.Trace,
                _ => LogLevel.Information
            };

            _logger.Log(level, msg.Exception, "Discord {Source}: {Message}", msg.Source, msg.Message);
            return Task.CompletedTask;
        }
    }
}
