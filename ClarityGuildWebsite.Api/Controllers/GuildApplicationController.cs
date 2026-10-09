using ClarityGuildWebsite.Api.Data;
using ClarityGuildWebsite.Api.DTOs;
using ClarityGuildWebsite.Api.Mappings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class GuildApplicationController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public GuildApplicationController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    [EnableRateLimiting("appPostPolicy")]
    public async Task<IActionResult> Create(CreateApplicationDTO dto)
    {
        var application = dto.ToEntity();
        application.PublicId = Guid.NewGuid();
        application.TimestampUtc = DateTime.UtcNow;
        application.Status = ApplicationStatus.Pending;
        application.PostStatus = PostStatus.Pending;
        application.Retry = 0;

        _context.GuildApplications.Add(application);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { publicId = application.PublicId }, application.ToOfficerResponse());
    }

    [HttpGet("{publicId:guid}")]
    public async Task<IActionResult> GetById(Guid publicId)
    {
        var application = await _context.GuildApplications.FirstOrDefaultAsync(a => a.PublicId == publicId);

        if (application is null)
        {
            return NotFound();
        }
        return Ok(application.ToOfficerResponse());      
    }
}



