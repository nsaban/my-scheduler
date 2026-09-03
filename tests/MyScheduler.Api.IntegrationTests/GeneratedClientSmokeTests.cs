using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
using MyScheduler.Client;
using MyScheduler.Client.Models;
using MyScheduler.Domain.Attendees;
using MyScheduler.Domain.ValueObjects;
using MyScheduler.Persistence;

namespace MyScheduler.Api.IntegrationTests;

public class GeneratedClientSmokeTests(ApiWebApplicationFactory factory) : IClassFixture<ApiWebApplicationFactory>
{
    [Fact]
    public async Task CreateEvent_ThenGetById_RoundTripsThroughGeneratedClient()
    {
        var client = CreateClient();
        var organizerId = await SeedOrganizerAsync();

        var startUtc = DateTimeOffset.UtcNow.AddDays(1);
        var eventId = await client.Events.PostAsync(new CreateEventCommand
        {
            Title = "Checkup",
            Description = "Routine visit",
            StartTimeUtc = startUtc,
            EndTimeUtc = startUtc.AddHours(1),
            OrganizerId = organizerId,
            AttendeeIds = [],
        });

        Assert.NotNull(eventId);

        var fetched = await client.Events[eventId!.Value].GetAsync();

        Assert.NotNull(fetched);
        Assert.Equal("Checkup", fetched!.Title);
        Assert.Equal(organizerId, fetched.OrganizerId);
    }

    [Fact]
    public async Task ListEvents_ReturnsPagedResult()
    {
        var client = CreateClient();

        var result = await client.Events.GetAsync();

        Assert.NotNull(result);
    }

    private MySchedulerClient CreateClient()
    {
        var httpClient = factory.CreateClient();
        var adapter = new HttpClientRequestAdapter(new AnonymousAuthenticationProvider(), httpClient: httpClient)
        {
            BaseUrl = httpClient.BaseAddress!.ToString(),
        };

        return new MySchedulerClient(adapter);
    }

    private async Task<Guid> SeedOrganizerAsync()
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var organizer = Attendee.Create("Dr. Smith", EmailAddress.Create($"{Guid.NewGuid()}@practice.com"), DateTime.UtcNow);
        dbContext.Attendees.Add(organizer);
        await dbContext.SaveChangesAsync();

        return organizer.Id;
    }
}
