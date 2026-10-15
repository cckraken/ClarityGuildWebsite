using ClarityGuildWebsite.Api.Data;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Eventing.Reader;


namespace ClarityGuildWebsite.Api.Services
{
    public class DiscordRetryWorker : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        const int MaxRetries = 5;

        public DiscordRetryWorker(ILogger<DiscordRetryWorker> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken ctoken)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromHours(2));
            _logger.LogInformation("DiscordRetryWorker is now starting.");

            while (!ctoken.IsCancellationRequested && await timer.WaitForNextTickAsync(ctoken))
            {
                DateTime cutOff = DateTime.UtcNow.AddMinutes(-10);
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var poster = scope.ServiceProvider.GetRequiredService<IApplicationDiscordPostingService>();

                    var toRetry = await context.GuildApplications
                        .Where(a => a.Retry < MaxRetries &&
                    (a.PostStatus == PostStatus.Failed ||
                    (a.PostStatus == PostStatus.Pending && a.TimestampUtc < cutOff)))
                        .ToListAsync(ctoken);

                    foreach (var application in toRetry)
                    {
                        try
                        {
                            application.Retry++;
                            await poster.PostAppToDiscord(application, ctoken);
                            if (application.PostStatus == PostStatus.Failed && MaxRetries >= 5)
                            {
                                application.PostStatus = PostStatus.Abandoned;
                                _logger.LogError("Final attempt reached for {PublicId}. Last error: {LastPostError}", application.PublicId, application.LastPostError);
                                await context.SaveChangesAsync(ctoken);
                            }
                        }
                        catch (OperationCanceledException) when (ctoken.IsCancellationRequested)
                        {
                            throw;
                        }
                    }
                }
                catch (OperationCanceledException) when (ctoken.IsCancellationRequested)
                {
                    _logger.LogInformation("App shutdown whilst service in progress.");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Retry run failed.");
                }
            }

        }
    }
}


