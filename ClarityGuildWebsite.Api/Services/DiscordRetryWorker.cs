using ClarityGuildWebsite.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace ClarityGuildWebsite.Api.Services
{
    public class DiscordRetryWorker : IDiscordRetryWorker
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public DiscordRetryWorker(ApplicationDbContext context, ILogger<DiscordRetryWorker> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken ctoken)
        {
            var postFailures = await _context.GuildApplications.FromSql($"EXECUTE dbo.GetFailedDiscordPosts").ToListAsync(ctoken);
            
        }

        public Task StopAsync(CancellationToken ctoken)
        {
            throw new NotImplementedException();
        }
    }
}
