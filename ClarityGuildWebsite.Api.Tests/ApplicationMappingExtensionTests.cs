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
            WowClass = "testClassChoice",
            WowSpecs = "testSpecs",
            Age = 30,
            Country = "testCountry",
            WarcraftLogsLink = "testUrl",
            Tech = "testTech",
            Schedule = "testSchedule",
            Splits = true,
            Communication = "testCommunication",
            History = "testHistory",
            Screenshot = "testScreenshot",
            Vouch = "testVouch",
            Goals = true,
        };

        var result = cDto.ToEntity();
        Assert.Multiple(() =>
        {
            Assert.Equal(cDto.DiscordId, result.DiscordId);
            Assert.Equal(cDto.WowClass, result.WowClass);
            Assert.Equal(cDto.WowSpecs, result.WowSpecs);
            Assert.Equal(cDto.Age, result.Age);
            Assert.Equal(cDto.Country, result.Country);
            Assert.Equal(cDto.WarcraftLogsLink, result.WarcraftLogsLink);
            Assert.Equal(cDto.Tech, result.Tech);
            Assert.Equal(cDto.Schedule, result.Schedule);
            Assert.Equal(cDto.Splits, result.Splits);
            Assert.Equal(cDto.Communication, result.Communication);
            Assert.Equal(cDto.History, result.History);
            Assert.Equal(cDto.Screenshot, result.Screenshot);
            Assert.Equal(cDto.Vouch, result.Vouch);
            Assert.Equal(cDto.Goals, result.Goals);
        });
    }

    [Fact]
    public void ToOfficerResponse_CopiesAllFormFields()
    {
        GuildApplication application = new GuildApplication
        {
            DiscordId = "testId",
            WowClass = "testClassChoice",
            WowSpecs = "testSpecs",
            Age = 30,
            Country = "testCountry",
            WarcraftLogsLink = "testUrl",
            Tech = "testTech",
            Schedule = "testSchedule",
            Splits = true,
            Communication = "testCommunication",
            History = "testHistory",
            Screenshot = "testScreenshot",
            Vouch = "testVouch",
            Goals = true,
            Id = 1,
            Status = ApplicationStatus.Pending,
            PostStatus = PostStatus.Pending,
            TimestampUtc = DateTime.UtcNow,
        };

        var result = application.ToOfficerResponse();
        Assert.Multiple(() =>
        {
            Assert.Equal(application.DiscordId, result.DiscordId);
            Assert.Equal(application.WowClass, result.WowClass);
            Assert.Equal(application.WowSpecs, result.WowSpecs);
            Assert.Equal(application.Age, result.Age);
            Assert.Equal(application.Country, result.Country);
            Assert.Equal(application.WarcraftLogsLink, result.WarcraftLogsLink);
            Assert.Equal(application.Tech, result.Tech);
            Assert.Equal(application.Schedule, result.Schedule);
            Assert.Equal(application.Splits, result.Splits);
            Assert.Equal(application.Communication, result.Communication);
            Assert.Equal(application.History, result.History);
            Assert.Equal(application.Screenshot, result.Screenshot);
            Assert.Equal(application.Vouch, result.Vouch);
            Assert.Equal(application.Goals, result.Goals);
            Assert.Equal(application.Id, result.Id);
            Assert.Equal(application.Status, result.Status);
            Assert.Equal(application.PostStatus, result.PostStatus);
            Assert.Equal(application.TimestampUtc, result.TimestampUtc);
        });
    }

}
