using ClarityGuildWebsite.Api.Services;
using Discord.Rest;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/discord/interactions")]
public class DiscordInteractionsController : ControllerBase
{
    private readonly DiscordBotConnectionService _connection;
    private readonly ILogger _logger;
    private readonly string _publicKey;
    public DiscordInteractionsController(DiscordBotConnectionService connection, IConfiguration config, ILogger<DiscordInteractionsController> logger)
    {
        _connection = connection;
        _logger = logger;
        _publicKey = config.GetValue<string>("Discord:PublicKey") ?? throw new InvalidOperationException("Discord:PublicKey is not configured.");
    }

    [HttpPost]
    public async Task<IActionResult> Receive(CancellationToken ctoken)
    {
        var signature = Request.Headers["X-Signature-Ed25519"].ToString();
        var timestamp = Request.Headers["X-Signature-Timestamp"].ToString();
        if (string.IsNullOrEmpty(signature) || string.IsNullOrEmpty(timestamp))
        {
            _logger.LogWarning("Rejected Discord interaction: {Reason}", "missing signature headers");
            return Unauthorized();
        }

        using var buffer = new MemoryStream();
        await Request.Body.CopyToAsync(buffer, ctoken);
        var body = buffer.ToArray();

        var client = await _connection.GetClientAsync();
        RestInteraction interaction;
        try
        {
            interaction = await client.ParseHttpInteractionAsync(_publicKey, signature, timestamp, body);
        }
        catch (BadSignatureException)
        {
            _logger.LogWarning("Rejected Discord interaction: {Reason}", "bad signature");
            return Unauthorized();
        }

        if (interaction is RestPingInteraction ping)
        {
            return Content(ping.AcknowledgePing(), "application/json");
        }

        _logger.LogWarning("Unhandled Discord interaction type {Type}", interaction.Type);
        return BadRequest();
    }
}
