using Discord;
using Discord.Rest;

namespace ClarityGuildWebsite.Api.Services
{
    public sealed class DiscordBotConnectionService
    {
        private readonly DiscordRestClient _restClient;
        private readonly string _token;
        private readonly SemaphoreSlim _loginLock = new(1, 1);

        public DiscordBotConnectionService(DiscordRestClient restClient, IConfiguration config)
        {
            _restClient = restClient;
            _token = config.GetValue<string>("Discord:BotToken") ?? throw new InvalidOperationException("Discord token is not configured/provided");
        }

        public async Task<DiscordRestClient> GetClientAsync(CancellationToken ctoken = default)
        {
            if (_restClient.LoginState == LoginState.LoggedIn)
                return _restClient;

            await _loginLock.WaitAsync(ctoken);
            try
            {
                if (_restClient.LoginState != LoginState.LoggedIn)
                    await _restClient.LoginAsync(TokenType.Bot, _token);
            }
            finally
            {
                _loginLock.Release();
            }
            return _restClient;
        }
    }
}