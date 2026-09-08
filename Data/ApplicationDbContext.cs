using Microsoft.EntityFrameworkCore;
using ClarityGuildWebsite.Api.Models;

namespace ClarityGuildWebsite.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {            
        }

        public DbSet<GuildApplication> GuildApplications { get; set; }
    }
}
