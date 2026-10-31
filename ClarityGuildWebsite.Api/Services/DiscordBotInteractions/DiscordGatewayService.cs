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
            // Cheap query on bot re-connect so a legit button click doesn't pay for the initial connection 
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                await context.GuildApplications.AnyAsync(ctoken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Database warm-up failed; continuing startup");
            }

            _socketClient.Log += OnLogAsync;
            _socketClient.ButtonExecuted += OnButtonExecutedAsync;
            _socketClient.ModalSubmitted += OnModalSubmittedAsync;
            _socketClient.MessageReceived += OnApplicantMessageReceivedAsync;

            await _socketClient.LoginAsync(TokenType.Bot, _token);
            await _socketClient.StartAsync();
        }

        // Runs the work off Discord.Net's gateway task, so a slow handler can't delay other events
        private Task RunInBackground(string eventName, Func<IServiceProvider, Task> work)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    await work(scope.ServiceProvider);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "{Event} handler failed", eventName);
                }
            });

            return Task.CompletedTask;
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

        private Task OnButtonExecutedAsync(SocketMessageComponent component) =>
                RunInBackground("Button", sp => sp.GetRequiredService<DiscordInteractionRouter>().HandleButtonInteractionAsync(component));

        private Task OnModalSubmittedAsync(SocketModal modal) =>
                RunInBackground("Modal", sp => sp.GetRequiredService<DiscordInteractionRouter>().HandleModalSubmittedAsync(modal));

        private Task OnApplicantMessageReceivedAsync(SocketMessage message) =>
                RunInBackground("DM", sp => sp.GetRequiredService<IApplicantMessagingService>().HandleApplicantMessageAsync(message));

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
