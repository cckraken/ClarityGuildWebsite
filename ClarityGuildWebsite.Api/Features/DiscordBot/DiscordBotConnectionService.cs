using Discord;
using Discord.Rest;
using Microsoft.Extensions.Options;

namespace ClarityGuildWebsite.Api.Features.DiscordBot
{
    public sealed class DiscordBotConnectionService
    {
        private readonly DiscordRestClient _restClient;
        private readonly string _token;
        private readonly SemaphoreSlim _loginLock = new(1, 1);

        public DiscordBotConnectionService(DiscordRestClient restClient, IOptions<DiscordBotOptions> discordOptions)
        {
            _restClient = restClient;
            _token = discordOptions.Value.BotToken;
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