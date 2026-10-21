using ClarityGuildWebsite.Api.Data;
using Discord.Rest;
using Microsoft.EntityFrameworkCore;

namespace ClarityGuildWebsite.Api.Services
{
    public class PersonalDataDeleteWorker : BackgroundService
    {
        private const int RetentionMonths = 3;
        private static readonly TimeSpan Interval = TimeSpan.FromHours(24);
        private readonly ILogger<PersonalDataDeleteWorker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public PersonalDataDeleteWorker(ILogger<PersonalDataDeleteWorker> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken ctoken)
        {
            _logger.LogInformation("PersonalDataDeleteWorker starting. Retention: {RetentionMonths} months", RetentionMonths);

            using var timer = new PeriodicTimer(Interval);

            do
            {
                try
                {
                    await DeleteExpiredAsync(ctoken);
                }
                catch (OperationCanceledException) when (ctoken.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Scheduled personal data deletion failed");
                }
            }
            while (await timer.WaitForNextTickAsync(ctoken));
        }

        private async Task DeleteExpiredAsync(CancellationToken ctoken)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var cutoff = DateTime.UtcNow.AddMonths(-RetentionMonths);

            var deletedDb = await context.GuildApplications
                .Where(a => a.TimestampUtc < cutoff)
                .ExecuteDeleteAsync(ctoken);

            _logger.LogInformation("Deleted {Count} applications submitted before {Cutoff:u}", deletedDb, cutoff);
        }
    }
}