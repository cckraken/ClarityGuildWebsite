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
                WowClass = guildApplication.WowClass,
                WowSpecs = guildApplication.WowSpecs,
                Age = guildApplication.Age,
                Country = guildApplication.Country,
                WarcraftLogsLink = guildApplication.WarcraftLogsLink,
                Tech = guildApplication.Tech,
                Schedule = guildApplication.Schedule,
                Splits = guildApplication.Splits,
                Communication = guildApplication.Communication,
                History = guildApplication.History,
                Screenshot = guildApplication.Screenshot,
                Vouch = guildApplication.Vouch,
                Goals = guildApplication.Goals,

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
                WowClass = dto.WowClass,
                WowSpecs = dto.WowSpecs,
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
            };
        }
    }
}
