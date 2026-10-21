using ClarityGuildWebsite.Api.DTOs;
using ClarityGuildWebsite.Api.Mappings;
using ClarityGuildWebsite.Api.Models;

namespace ClarityGuildWebsite.Api.Tests;

public class ApplicationMappingExtensionTests
{
    [Fact]
    public void ToEntity_CopiesAllFormFields()
    {
        string testDiscordId = "123456";
        string testDiscordName = "TestDiscName";
        CreateApplicationDTO cDto = new CreateApplicationDTO()
        {
            Country = "TestCountry",
            MainName = "TestName",
            MainRealm = "TestRealm",
            MainRole = "TestRole",
            AltName = "AltName",
            AltRealm = "AltRealm",
            AltRole = "AltRole",
            Screenshot = "TestScreenshot",
            About = "TestAbout",
            Tech = "TestTech",
            History = "TestHistory",
            Extra = "TestExtra",
        };

        var result = cDto.ToEntity(testDiscordId, testDiscordName);
        Assert.Multiple(() =>
        {
            Assert.Equal(cDto.Country, result.Country);
            Assert.Equal(cDto.MainName, result.MainName);
            Assert.Equal(cDto.MainRealm, result.MainRealm);
            Assert.Equal(cDto.MainRole, result.MainRole);
            Assert.Equal(cDto.AltName, result.AltName);
            Assert.Equal(cDto.AltRealm, result.AltRealm);
            Assert.Equal(cDto.AltRole, result.AltRole);
            Assert.Equal(cDto.Screenshot, result.Screenshot);
            Assert.Equal(cDto.About, result.About);
            Assert.Equal(cDto.Tech, result.Tech);
            Assert.Equal(cDto.History, result.History);
            Assert.Equal(cDto.Extra, result.Extra);
        });
    }

    [Fact]
    public void ToOfficerResponse_CopiesAllFormFields()
    {
        GuildApplication application = new GuildApplication
        {
            DiscordId = "discordId",
            DiscordName = "TestDiscName",
            PublicId = Guid.NewGuid(),
            Country = "TestCountry",
            MainName = "TestName",
            MainRealm = "TestRealm",
            MainRole = "TestRole",
            AltName = "AltName",
            AltRealm = "AltRealm",
            AltRole = "AltRole",
            Screenshot = "TestScreenshot",
            About = "TestAbout",
            Tech = "TestTech",
            History = "TestHistory",
            Extra = "TestExtra",
            Id = 1,
            Status = ApplicationStatus.Pending,
            PostStatus = PostStatus.Pending,
            TimestampUtc = DateTime.UtcNow,
        };

        var result = application.ToOfficerResponse();
        Assert.Multiple(() =>
        {
            Assert.Equal(application.DiscordId, result.DiscordId);
            Assert.Equal(application.DiscordName, result.DiscordName);
            Assert.Equal(application.Country, result.Country);
            Assert.Equal(application.MainName, result.MainName);
            Assert.Equal(application.MainRealm, result.MainRealm);
            Assert.Equal(application.MainRole, result.MainRole);
            Assert.Equal(application.AltName, result.AltName);
            Assert.Equal(application.AltRealm, result.AltRealm);
            Assert.Equal(application.AltRole, result.AltRole);
            Assert.Equal(application.Screenshot, result.Screenshot);
            Assert.Equal(application.About, result.About);
            Assert.Equal(application.Tech, result.Tech);
            Assert.Equal(application.History, result.History);
            Assert.Equal(application.Extra, result.Extra);

            Assert.Equal(application.Status, result.Status);
            Assert.Equal(application.PostStatus, result.PostStatus);
            Assert.Equal(application.TimestampUtc, result.TimestampUtc);
        });
    }

}
