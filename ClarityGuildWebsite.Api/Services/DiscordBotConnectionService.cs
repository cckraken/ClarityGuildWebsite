using Discord;
using Discord.Rest;

namespace ClarityGuildWebsite.Api.Services
{
    public sealed class DiscordBotConnectionService
    {
        private readonly DiscordRestClient _client;
        private readonly string _token;
        private readonly SemaphoreSlim _loginLock = new(1, 1);

        public DiscordBotConnectionService(DiscordRestClient client, IConfiguration config)
        {
            _client = client;

            _token = config.GetValue<string>("Discord:BotToken") ?? throw new InvalidOperationException("Discord token is not configured/provided");
        }

        public async Task<DiscordRestClient> GetClientAsync()
        {
            if (_client.LoginState == LoginState.LoggedIn)
                return _client;

            await _loginLock.WaitAsync();
            try
            {
                if (_client.LoginState != LoginState.LoggedIn)
                    await _client.LoginAsync(TokenType.Bot, _token);
            }
            finally
            {
                _loginLock.Release();
            }
            return _client;
        }
    }
}