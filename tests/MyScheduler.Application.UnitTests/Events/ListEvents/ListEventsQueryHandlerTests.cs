using MyScheduler.Application.Contracts;
using MyScheduler.Application.Events.ListEvents;
using MyScheduler.Application.UnitTests.Fakes;
using MyScheduler.Domain.Scheduling;

namespace MyScheduler.Application.UnitTests.Events.ListEvents;

public class ListEventsQueryHandlerTests
{
    [Fact]
    public async Task Handle_TranslatesQueryIntoFilter_AndReturnsQueriesResult()
    {
        var organizerId = Guid.NewGuid();
        var attendeeId = Guid.NewGuid();
        var expected = new PagedResult<EventSummaryDto> { TotalCount = 3, Page = 2, PageSize = 10 };
        var queries = new FakeEventListingQueries { ListResult = expected };
        var handler = new ListEventsQueryHandler(queries);

        var fromUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var toUtc = fromUtc.AddDays(7);
        var query = new ListEventsQuery(fromUtc, toUtc, EventStatus.Scheduled, organizerId, attendeeId, 2, 10);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Same(expected, result);
        Assert.NotNull(queries.LastFilter);
        Assert.Equal(fromUtc, queries.LastFilter!.FromUtc);
        Assert.Equal(toUtc, queries.LastFilter.ToUtc);
        Assert.Equal(EventStatus.Scheduled, queries.LastFilter.Status);
        Assert.Equal(organizerId, queries.LastFilter.OrganizerId);
        Assert.Equal(attendeeId, queries.LastFilter.AttendeeId);
        Assert.Equal(2, queries.LastFilter.Page);
        Assert.Equal(10, queries.LastFilter.PageSize);
    }
}
