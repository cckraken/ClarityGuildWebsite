using ClarityGuildWebsite.Api.Features.DiscordBot.Interactions;
using ClarityGuildWebsite.Api.Features.DiscordBot.Messaging;
using ClarityGuildWebsite.Api.Features.DiscordBot.SlashCommands;
using ClarityGuildWebsite.Api.Features.RecruitmentFeed;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace ClarityGuildWebsite.Api.Features.DiscordBot
{
    public class DiscordGatewayService : IHostedService
    {
        private readonly string _token;
        private const string DiscordNetLogCategory = "Discord.Net"; // Referenced by "Discord.Net" in appsettings.json Logging:LogLevel
        private readonly DiscordSocketClient _socketClient;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger _logger;
        private readonly ILogger _discordNetLogger;

        public DiscordGatewayService(IOptions<DiscordBotOptions> botOptions, DiscordSocketClient client, IServiceScopeFactory scopeFactory, ILogger<DiscordGatewayService> logger, ILoggerFactory loggerFactory)
        {
            var token = botOptions.Value.BotToken;

            _socketClient = client;
            _scopeFactory = scopeFactory;
            _logger = logger;
            _discordNetLogger = loggerFactory.CreateLogger(DiscordNetLogCategory);
            _token = token;
        }

        public async Task StartAsync(CancellationToken ctoken)
        {
            _socketClient.MessageReceived += OnWebhookMessageReceivedAsync;
            _socketClient.MessageReceived += OnApplicantMessageReceivedAsync;
            _socketClient.Log += OnLogAsync;
            _socketClient.ButtonExecuted += OnButtonExecutedAsync;
            _socketClient.ModalSubmitted += OnModalSubmittedAsync;
            _socketClient.SlashCommandExecuted += OnSlashCommandExecutedAsync;
            _socketClient.Ready += OnReadyAsync;

            await _socketClient.LoginAsync(TokenType.Bot, _token);
            await _socketClient.StartAsync();
        }

        // Runs the work off Discord.Net's gateway task, so a slow handler can't delay other events
        private Task RunInBackground(string label, Func<IServiceProvider, Task> work, bool logTiming = true)
        {
            _ = Task.Run(async () =>
            {
                var stopwatch = Stopwatch.StartNew();
                if (logTiming)
                    _logger.LogInformation("Handling {Event}", label);
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    await work(scope.ServiceProvider);
                    if (logTiming)
                        _logger.LogInformation("Handled {Event} in {ElapsedMs} ms", label, stopwatch.ElapsedMilliseconds);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "{Event} failed after {ElapsedMs} ms", label, stopwatch.ElapsedMilliseconds);
                }
            });

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken ctoken)
        {
            _socketClient.MessageReceived -= OnWebhookMessageReceivedAsync;
            _socketClient.ButtonExecuted -= OnButtonExecutedAsync;
            _socketClient.ModalSubmitted -= OnModalSubmittedAsync;
            _socketClient.MessageReceived -= OnApplicantMessageReceivedAsync;
            _socketClient.SlashCommandExecuted -= OnSlashCommandExecutedAsync;
            _socketClient.Ready -= OnReadyAsync;
            _socketClient.Log -= OnLogAsync;

            await _socketClient.StopAsync();
            await _socketClient.LogoutAsync();
        }

        private Task OnButtonExecutedAsync(SocketMessageComponent component) =>
                RunInBackground("Button", sp => sp.GetRequiredService<DiscordInteractionRouter>().HandleButtonInteractionAsync(component));

        private Task OnModalSubmittedAsync(SocketModal modal) =>
                RunInBackground("Modal", sp => sp.GetRequiredService<DiscordInteractionRouter>().HandleModalSubmittedAsync(modal));

        private Task OnApplicantMessageReceivedAsync(SocketMessage message) =>
                RunInBackground("DM", sp => sp.GetRequiredService<IApplicantMessagingService>().HandleApplicantMessageAsync(message), logTiming: false);

        private Task OnSlashCommandExecutedAsync(SocketSlashCommand command) =>
            RunInBackground("SlashCommands", sp => sp.GetRequiredService<DiscordSlashCommandsRouter>().HandleSlashCommandExecutedAsync(command));
        private Task OnReadyAsync() =>
            RunInBackground("Ready", sp => sp.GetRequiredService<DiscordSlashCommands>().RegisterSlashCommandsAsync());
        private Task OnWebhookMessageReceivedAsync(SocketMessage message) =>
            RunInBackground("RecruitmentFeed", sp => sp.GetRequiredService<RecruitmentFeedHandler>().HandleWebhookReceivedAsync(message), logTiming: false);

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

            _discordNetLogger.Log(level, msg.Exception, "Discord {Source}: {Message}", msg.Source, msg.Message);
            return Task.CompletedTask;
        }
    }
}
