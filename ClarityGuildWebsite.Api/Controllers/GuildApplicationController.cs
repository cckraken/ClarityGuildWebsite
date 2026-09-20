using ClarityGuildWebsite.Api.Data;
using ClarityGuildWebsite.Api.DTOs;
using ClarityGuildWebsite.Api.Models;
using ClarityGuildWebsite.Api.Mappings;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<IActionResult> Create(CreateApplicationDTO dto)
    {
        var application = dto.ToEntity();
        application.TimestampUtc = DateTime.UtcNow;
        application.Status = ApplicationStatus.Pending;
        application.PostStatus = PostStatus.Pending;
        application.Retry = 0;

        _context.GuildApplications.Add(application);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = application.Id }, application.ToOfficerResponse());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var application = await _context.GuildApplications.FindAsync(id);

        if (application is null)
        {
            return NotFound();
        }
        return Ok(application.ToOfficerResponse());      
    }
}



