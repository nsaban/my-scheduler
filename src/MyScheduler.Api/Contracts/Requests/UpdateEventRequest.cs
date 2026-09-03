namespace MyScheduler.Api.Contracts.Requests;

public sealed record UpdateEventRequest(string Title, string? Description, DateTime StartTimeUtc, DateTime EndTimeUtc);
