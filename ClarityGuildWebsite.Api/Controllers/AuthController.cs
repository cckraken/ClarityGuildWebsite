using AspNet.Security.OAuth.Discord;
using ClarityGuildWebsite.Api.DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly string[] _allowedOrigins;

    public AuthController(IConfiguration config)
    {
        var origins = config.GetSection("Cors:AllowedOrigins").Get<string[]>();

        if (origins is null || origins.Length == 0)
            throw new InvalidOperationException("Cors:AllowedOrigins is missing or empty.");

        _allowedOrigins = origins;
    }

    [HttpGet("login")]
    public IActionResult LoginWithDiscord([FromQuery] string? returnUrl)
    {
        var safeReturnUrl = IsAllowedReturnUrl(returnUrl) ? returnUrl : _allowedOrigins[0];
        var props = new AuthenticationProperties { RedirectUri = safeReturnUrl };
        return Challenge(props, DiscordAuthenticationDefaults.AuthenticationScheme);
    }

    private bool IsAllowedReturnUrl(string? returnUrl)
    {
        if (string.IsNullOrWhiteSpace(returnUrl)) return false;

        if (!Uri.TryCreate(returnUrl, UriKind.Absolute, out var uri)) return false;

        var origin = uri.GetLeftPart(UriPartial.Authority);
        return _allowedOrigins.Any(allowedOrigin =>
            allowedOrigin.Equals(origin, StringComparison.OrdinalIgnoreCase));
    }

    [Authorize]
    [HttpGet("me")]
    public ActionResult<DiscordUserDTO> Me()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userName = User.FindFirstValue(ClaimTypes.Name);

        if (id is null || userName is null) return Unauthorized();

        return Ok(new DiscordUserDTO(id, userName));
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return NoContent();
    }
}

