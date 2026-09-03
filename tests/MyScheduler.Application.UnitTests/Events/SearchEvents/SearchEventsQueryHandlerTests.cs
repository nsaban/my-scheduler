using MyScheduler.Application.Contracts;
using MyScheduler.Application.Events.SearchEvents;
using MyScheduler.Application.UnitTests.Fakes;

namespace MyScheduler.Application.UnitTests.Events.SearchEvents;

public class SearchEventsQueryHandlerTests
{
    [Fact]
    public async Task Handle_PassesSearchTermThrough_AndReturnsQueriesResult()
    {
        var expected = new PagedResult<EventSummaryDto> { TotalCount = 1 };
        var queries = new FakeEventListingQueries { SearchResult = expected };
        var handler = new SearchEventsQueryHandler(queries);

        var result = await handler.Handle(new SearchEventsQuery("checkup", 1, 20), CancellationToken.None);

        Assert.Same(expected, result);
        Assert.Equal("checkup", queries.LastSearchTerm);
    }
}
