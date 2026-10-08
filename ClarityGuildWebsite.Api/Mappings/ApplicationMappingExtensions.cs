using ClarityGuildWebsite.Api.DTOs;
using ClarityGuildWebsite.Api.Models;

namespace ClarityGuildWebsite.Api.Mappings
{
    public static class ApplicationMappingExtensions
    {
        public static OfficerResponseDTO ToOfficerResponse(this GuildApplication guildApplication)
        {
            return new OfficerResponseDTO
            {      
                DiscordId = guildApplication.DiscordId,
                Country = guildApplication.Country,
                MainName = guildApplication.MainName,
                MainRealm = guildApplication.MainRealm,
                MainRole = guildApplication.MainRole,
                AltName = guildApplication.AltName,
                AltRealm = guildApplication.AltRealm,
                AltRole = guildApplication.AltRole,
                Screenshot = guildApplication.Screenshot,
                About = guildApplication.About,
                Tech = guildApplication.Tech,
                History = guildApplication.History,
                Extra = guildApplication.Extra,
                //Id To be added later
                Status = guildApplication.Status,
                PostStatus = guildApplication.PostStatus,
                TimestampUtc = guildApplication.TimestampUtc,
            };
        }

        public static GuildApplication ToEntity(this CreateApplicationDTO dto)
        {
            return new GuildApplication
            {
                DiscordId = dto.DiscordId,
                Country = dto.Country,
                MainName = dto.MainName,
                MainRealm = dto.MainRealm,
                MainRole = dto.MainRole,
                AltName= dto.AltName,
                AltRealm = dto.AltRealm,
                AltRole = dto.AltRole,
                Screenshot = dto.Screenshot,
                About = dto.About,
                Tech = dto.Tech,
                History = dto.History,
                Extra = dto.Extra,
            };
        }
    }
}
