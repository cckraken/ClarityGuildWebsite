using ClarityGuildWebsite.Api.Models.Applications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClarityGuildWebsite.Api.Data
{
    public class ApplicationConfig : IEntityTypeConfiguration<GuildApplication>
    {
        public void Configure(EntityTypeBuilder<GuildApplication> builder)
        {
            builder.ToTable("GuildApplication");
            builder.HasKey(k => k.PublicId);

            // Form facing fields
            builder.Property(p => p.DiscordId).HasMaxLength(50).IsRequired();
            builder.Property(p => p.DiscordName).HasMaxLength(50).IsRequired();
            builder.Property(p => p.Country).HasMaxLength(50).IsRequired();
            builder.Property(p => p.Age);

            builder.Property(p => p.MainName).HasMaxLength(50).IsRequired();
            builder.Property(p => p.MainRealm).HasMaxLength(50).IsRequired();
            builder.Property(p => p.MainRole).HasMaxLength(50).IsRequired();

            builder.Property(p => p.AltName).HasMaxLength(50).IsRequired();
            builder.Property(p => p.AltRealm).HasMaxLength(50).IsRequired();
            builder.Property(p => p.AltRole).HasMaxLength(50).IsRequired();

            builder.Property(p => p.Preparation).HasMaxLength(1024);
            builder.Property(p => p.About).HasMaxLength(2000).IsRequired();
            builder.Property(p => p.Tech).HasMaxLength(500).IsRequired();
            builder.Property(p => p.History).HasMaxLength(2000).IsRequired();
            builder.Property(p => p.Extra).HasMaxLength(2000);

            // DB only
            builder.Property(p => p.Id).ValueGeneratedOnAdd();
            builder.Property(p => p.PublicId).IsRequired();
            builder.Property(p => p.TimestampUtc).IsRequired();
            builder.Property(p => p.DiscordMessageId).HasMaxLength(50);
            builder.Property(p => p.DiscordThreadId).HasMaxLength(50);
            builder.Property(p => p.LastPostError).HasMaxLength(250);
            builder.Property(p => p.LastAttemptedUtc);
            builder.Property(p => p.LastRecruitmentTeamMessagedUtc);
            builder.Property(p => p.Retry).IsRequired();
            builder.Property(p => p.Status).IsRequired();
            builder.Property(p => p.PostStatus).IsRequired();

            builder.Property(p => p.RowVersion).IsRowVersion();

            builder.HasIndex(i => i.DiscordThreadId).IsUnique();
        }
    }
}
