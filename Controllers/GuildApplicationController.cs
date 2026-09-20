using ClarityGuildWebsite.Api.Data;
using ClarityGuildWebsite.Api.DTOs;
using ClarityGuildWebsite.Api.Models;
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
        var application = new GuildApplication
        {
            //form facing
            DiscordId = dto.DiscordId,
            Age = dto.Age,
            Country = dto.Country,
            WarcraftLogsLink = dto.WarcraftLogsLink,
            Tech = dto.Tech,
            Schedule = dto.Schedule,
            Splits = dto.Splits,
            Communication = dto.Communication,
            History = dto.History,
            Screenshot = dto.Screenshot,
            Vouch = dto.Vouch,
            Goals = dto.Goals,

            //db specific
            TimestampUtc = DateTime.UtcNow,
            Retry = 0,
            PostStatus = PostStatus.Pending,
            Status = ApplicationStatus.Pending
        };
        _context.GuildApplications.Add(application);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = application.Id }, application);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var application = await _context.GuildApplications.FindAsync(id);

        if (application is null)
        {
            return NotFound();
        }

        var officerResponse = new OfficerResponseDTO
        {
            DiscordId = application.DiscordId,
            Age = application.Age,
            Country = application.Country,
            WarcraftLogsLink = application.WarcraftLogsLink,
            Tech = application.Tech,
            Schedule = application.Schedule,
            Splits = application.Splits,
            Communication = application.Communication,
            History = application.History,
            Screenshot = application.Screenshot,
            Vouch = application.Vouch,
            Goals = application.Goals,

            Id = application.Id,
            Status = application.Status,
            PostStatus = application.PostStatus,
            TimestampUtc = application.TimestampUtc

        };

        return Ok(officerResponse);
    }
}



