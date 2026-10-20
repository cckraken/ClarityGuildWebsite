using ClarityGuildWebsite.Api.Data;
using ClarityGuildWebsite.Api.DTOs;
using ClarityGuildWebsite.Api.Mappings;
using ClarityGuildWebsite.Api.Services;
using ClarityGuildWebsite.Api.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class GuildApplicationController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IApplicationDiscordPostingService _service;

    public GuildApplicationController(ApplicationDbContext context, IApplicationDiscordPostingService service)
    {
        _context = context;
        _service = service;
    }

    [Authorize]
    [HttpPost]
    [EnableRateLimiting("appPostPolicy")]
    public async Task<IActionResult> Create(CreateApplicationDTO dto, CancellationToken ctoken)
    {
        var application = dto.ToEntity();
        application.DiscordId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        application.PublicId = Guid.NewGuid();
        application.TimestampUtc = DateTime.UtcNow;
        application.Status = ApplicationStatus.Pending;
        application.PostStatus = PostStatus.Pending;
        application.Retry = 0;

        _context.GuildApplications.Add(application);
        await _context.SaveChangesAsync();

        await _service.PostAppToDiscord(application, ctoken);
        return CreatedAtAction(nameof(GetById), new { publicId = application.PublicId }, application.ToApplicantResponse());
    }

    [HttpGet("{publicId:guid}")]
    public async Task<IActionResult> GetById(Guid publicId)
    {
        var application = await _context.GuildApplications.FirstOrDefaultAsync(a => a.PublicId == publicId);

        if (application is null)
        {
            return NotFound();
        }
        return Ok(application.ToApplicantResponse());      
    }
}



