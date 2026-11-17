using ClarityGuildWebsite.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace ClarityGuildWebsite.Api.Features.Applications
{
    public class PersonalDataDeleteWorker : BackgroundService
    {
        private const int RetentionMonths = 3;
        private static readonly TimeSpan Interval = TimeSpan.FromHours(24);
        private static readonly TimeSpan RetryDelay = TimeSpan.FromMinutes(2);
        private const int MaxAttempts = 3;
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
                for (var attempt = 1; attempt <= MaxAttempts; attempt++)
                {
                    try
                    {
                        await DeleteExpiredAsync(ctoken);
                        break;
                    }
                    catch (OperationCanceledException) when (ctoken.IsCancellationRequested)
                    {
                        throw;
                    }
                    catch (Exception ex) when (attempt < MaxAttempts)
                    {
                        _logger.LogWarning(ex, "Personal data deletion attempt {Attempt} failed; retrying in {Delay}", attempt, RetryDelay);
                        await Task.Delay(RetryDelay, ctoken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Personal data deletion failed after {Attempts} attempts", MaxAttempts);
                    }
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