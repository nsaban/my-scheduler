namespace MyScheduler.Application.Contracts;

public sealed record PagedResult<T>
{
    public IReadOnlyCollection<T> Items { get; init; } = [];

    public int TotalCount { get; init; }

    public int Page { get; init; }

    public int PageSize { get; init; }
}
