using ClarityGuildWebsite.Api.Data;
using ClarityGuildWebsite.Api.DTOs;
using ClarityGuildWebsite.Api.Mappings;
using ClarityGuildWebsite.Api.Services;
using ClarityGuildWebsite.Api.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class GuildApplicationController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IApplicationNotifier _notifier;
    private readonly ILogger<GuildApplicationController> _logger;

    public GuildApplicationController(ApplicationDbContext context, IApplicationNotifier notifier, ILogger<GuildApplicationController> logger)
    {
        _context = context;
        _notifier = notifier;
        _logger = logger;
    }

    [HttpPost]
    [EnableRateLimiting("appPostPolicy")]
    public async Task<IActionResult> Create(CreateApplicationDTO dto, CancellationToken ctoken)
    {
        var application = dto.ToEntity();
        application.PublicId = Guid.NewGuid();
        application.TimestampUtc = DateTime.UtcNow;
        application.Status = ApplicationStatus.Pending;
        application.PostStatus = PostStatus.Pending;
        application.Retry = 0;

        _context.GuildApplications.Add(application);
        await _context.SaveChangesAsync();

        var result = await _notifier.NotifyNewApplicationAsync(application, ctoken);
        application.LastAttemptedUtc = DateTime.UtcNow;
        if (result.Success)
        {
            application.PostStatus = PostStatus.Delivered;
            application.DiscordMessageId = result.MessageId;
            application.LastPostError = null;
            _logger.LogInformation("Post successful for {PublicId} : {DiscordMessageId}", application.PublicId, application.DiscordMessageId);
        }
        else
        {
            application.PostStatus = PostStatus.Failed;
            application.LastPostError = string.IsNullOrWhiteSpace(result.Error) ? "Unknown Error" : ApplicationHelpers.Truncate(result.Error, 250);
            _logger.LogWarning("Post failed for {PublicId}: {Error}", application.PublicId, result.Error);
        }
        await _context.SaveChangesAsync();
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



