using ClarityGuildWebsite.Api.DTOs;
using ClarityGuildWebsite.Api.Mappings;
using ClarityGuildWebsite.Api.Models;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace ClarityGuildWebsite.Api.Tests;

public class ApplicationMappingExtensionTests
{
    [Fact]
    public void ToEntity_CopiesAllFormFields()
    {
        CreateApplicationDTO cDto = new CreateApplicationDTO
        {
            DiscordId = "testId",
            MainName = "TestName",
            MainRealm = "TestRealm",
            MainRole = "TestRole",
            AltName = "AltName",
            AltRealm = "AltRealm",
            AltRole = "AltRole",
            Screenshot = "testScreenshot",
            Schedule = "testSchedule",
            Splits = true,
            Goals = true,
            About = "testAbout",
            Tech = "testTech",
            History = "testHistory",
            Extra = "testExtra",
        };

        var result = cDto.ToEntity();
        Assert.Multiple(() =>
        {
            Assert.Equal(cDto.DiscordId, result.DiscordId);
            Assert.Equal(cDto.MainName, result.MainName);
            Assert.Equal(cDto.MainRealm, result.MainRealm);
            Assert.Equal(cDto.MainRole, result.MainRole);
            Assert.Equal(cDto.AltName, result.AltName);
            Assert.Equal(cDto.AltRealm, result.AltRealm);
            Assert.Equal(cDto.AltRole, result.AltRole);
            Assert.Equal(cDto.Screenshot, result.Screenshot);
            Assert.Equal(cDto.Schedule, result.Schedule);
            Assert.Equal(cDto.Splits, result.Splits);
            Assert.Equal(cDto.Goals, result.Goals);
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
            DiscordId = "testId",
            MainName = "TestName",
            MainRealm = "TestRealm",
            MainRole = "TestRole",
            AltName = "AltName",
            AltRealm = "AltRealm",
            AltRole = "AltRole",
            Screenshot = "testScreenshot",
            Schedule = "testSchedule",
            Splits = true,
            Goals = true,
            About = "testAbout",
            Tech = "testTech",
            History = "testHistory",
            Extra = "testExtra",
            Id = 1,
            Status = ApplicationStatus.Pending,
            PostStatus = PostStatus.Pending,
            TimestampUtc = DateTime.UtcNow,
        };

        var result = application.ToOfficerResponse();
        Assert.Multiple(() =>
        {
            Assert.Equal(application.DiscordId, result.DiscordId);
            Assert.Equal(application.MainName, result.MainName);
            Assert.Equal(application.MainRealm, result.MainRealm);
            Assert.Equal(application.MainRole, result.MainRole);
            Assert.Equal(application.AltName, result.AltName);
            Assert.Equal(application.AltRealm, result.AltRealm);
            Assert.Equal(application.AltRole, result.AltRole);
            Assert.Equal(application.Screenshot, result.Screenshot);
            Assert.Equal(application.Schedule, result.Schedule);
            Assert.Equal(application.Splits, result.Splits);
            Assert.Equal(application.Goals, result.Goals);
            Assert.Equal(application.About, result.About);
            Assert.Equal(application.Tech, result.Tech);
            Assert.Equal(application.History, result.History);
            Assert.Equal(application.Extra, result.Extra);

            Assert.Equal(application.Id, result.Id);
            Assert.Equal(application.Status, result.Status);
            Assert.Equal(application.PostStatus, result.PostStatus);
            Assert.Equal(application.TimestampUtc, result.TimestampUtc);
        });
    }

}
