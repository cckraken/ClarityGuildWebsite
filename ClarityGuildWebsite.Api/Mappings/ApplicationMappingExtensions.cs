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
                MainName = guildApplication.MainName,
                MainRealm = guildApplication.MainRealm,
                MainRole = guildApplication.MainRole,
                AltName = guildApplication.AltName,
                AltRealm = guildApplication.AltRealm,
                AltRole = guildApplication.AltRole,
                Screenshot = guildApplication.Screenshot,
                Schedule = guildApplication.Schedule,
                Splits = guildApplication.Splits,
                Goals = guildApplication.Goals,
                About = guildApplication.About,
                Tech = guildApplication.Tech,
                History = guildApplication.History,
                Extra = guildApplication.Extra,

                Id = guildApplication.Id,
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
                MainName = dto.MainName,
                MainRealm = dto.MainRealm,
                MainRole = dto.MainRole,
                AltName= dto.AltName,
                AltRealm = dto.AltRealm,
                AltRole = dto.AltRole,
                Screenshot = dto.Screenshot,
                Schedule = dto.Schedule,
                Splits = dto.Splits,
                Goals = dto.Goals,
                About = dto.About,
                Tech = dto.Tech,
                History = dto.History,
                Extra = dto.About,
            };
        }
    }
}
