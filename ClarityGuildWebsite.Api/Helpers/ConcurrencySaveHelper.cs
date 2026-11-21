using ClarityGuildWebsite.Api.Data;
using ClarityGuildWebsite.Api.Models.Applications;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;

namespace ClarityGuildWebsite.Api.Helpers
{
    public static class ConcurrencySaveHelper
    {
        public static async Task<bool> TrySaveAsync(ApplicationDbContext context, GuildApplication application, SocketModal modal, ILogger logger, string action)
        {
            try
            {
                await context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                logger.LogWarning("Concurrent update on application {PublicId} during {Action}", application.PublicId, action);
                await modal.FollowupAsync("This application was just updated by another member of the recruitment team. Check the thread for what changed, then try again if it's still needed.", ephemeral: true);
                return false;
            }
        }
    }
}
