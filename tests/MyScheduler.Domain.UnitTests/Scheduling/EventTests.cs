using MyScheduler.Domain.Common;
using MyScheduler.Domain.Scheduling;
using MyScheduler.Domain.Scheduling.DomainEvents;
using MyScheduler.Domain.ValueObjects;

namespace MyScheduler.Domain.UnitTests.Scheduling;

public class EventTests
{
    private static readonly DateTime NowUtc = new(2026, 1, 1, 9, 0, 0, DateTimeKind.Utc);
    private static readonly DateTimeRange SampleRange = new(NowUtc, NowUtc.AddHours(1));
    private static readonly Guid OrganizerId = Guid.NewGuid();
    private static readonly Guid AttendeeId = Guid.NewGuid();

    [Fact]
    public void Schedule_WhenValid_CreatesEventWithOrganizerAndAttendees()
    {
        var @event = Event.Schedule(
            "Annual Checkup",
            "Routine checkup",
            SampleRange,
            OrganizerId,
            [AttendeeId],
            NowUtc);

        Assert.Equal(EventStatus.Scheduled, @event.Status);
        Assert.Equal(2, @event.EventAttendees.Count);
        Assert.Contains(@event.EventAttendees, ea => ea.AttendeeId == OrganizerId);
        Assert.Contains(@event.EventAttendees, ea => ea.AttendeeId == AttendeeId);
        Assert.All(@event.EventAttendees, ea => Assert.Equal(ResponseStatus.Pending, ea.ResponseStatus));
        Assert.Contains(@event.DomainEvents, e => e is EventCreatedDomainEvent);
    }

    [Fact]
    public void Schedule_WhenOrganizerAlsoListedAsAttendee_IsNotDuplicated()
    {
        var @event = Event.Schedule(
            "Annual Checkup",
            null,
            SampleRange,
            OrganizerId,
            [OrganizerId, AttendeeId],
            NowUtc);

        Assert.Equal(2, @event.EventAttendees.Count);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Schedule_WhenTitleMissing_Throws(string title)
    {
        Assert.Throws<DomainException>(() =>
            Event.Schedule(title, null, SampleRange, OrganizerId, [], NowUtc));
    }

    [Fact]
    public void Schedule_WhenTitleExceedsMaxLength_Throws()
    {
        var tooLong = new string('a', Event.MaxTitleLength + 1);

        Assert.Throws<DomainException>(() =>
            Event.Schedule(tooLong, null, SampleRange, OrganizerId, [], NowUtc));
    }

    [Fact]
    public void Schedule_WhenDescriptionExceedsMaxLength_Throws()
    {
        var tooLong = new string('a', Event.MaxDescriptionLength + 1);

        Assert.Throws<DomainException>(() =>
            Event.Schedule("Title", tooLong, SampleRange, OrganizerId, [], NowUtc));
    }

    [Fact]
    public void UpdateDetails_WhenScheduled_UpdatesFieldsAndRaisesEvent()
    {
        var @event = Event.Schedule("Title", null, SampleRange, OrganizerId, [], NowUtc);
        var updatedAt = NowUtc.AddHours(1);
        var newRange = new DateTimeRange(updatedAt, updatedAt.AddHours(1));

        @event.UpdateDetails("New Title", "New Description", newRange, updatedAt);

        Assert.Equal("New Title", @event.Title);
        Assert.Equal("New Description", @event.Description);
        Assert.Equal(newRange, @event.TimeRange);
        Assert.Equal(updatedAt, @event.UpdatedAtUtc);
        Assert.Contains(@event.DomainEvents, e => e is EventUpdatedDomainEvent);
    }

    [Fact]
    public void UpdateDetails_WhenCancelled_Throws()
    {
        var @event = Event.Schedule("Title", null, SampleRange, OrganizerId, [], NowUtc);
        @event.Cancel(NowUtc);

        Assert.Throws<DomainException>(() =>
            @event.UpdateDetails("New Title", null, SampleRange, NowUtc));
    }

    [Fact]
    public void Cancel_WhenScheduled_SetsStatusAndRaisesEvent()
    {
        var @event = Event.Schedule("Title", null, SampleRange, OrganizerId, [], NowUtc);

        @event.Cancel(NowUtc.AddHours(1));

        Assert.Equal(EventStatus.Cancelled, @event.Status);
        Assert.Contains(@event.DomainEvents, e => e is EventCancelledDomainEvent);
    }

    [Fact]
    public void Cancel_WhenAlreadyCancelled_Throws()
    {
        var @event = Event.Schedule("Title", null, SampleRange, OrganizerId, [], NowUtc);
        @event.Cancel(NowUtc);

        Assert.Throws<DomainException>(() => @event.Cancel(NowUtc));
    }

    [Fact]
    public void RecordAttendeeResponse_WhenInvitee_UpdatesResponseAndRaisesEvent()
    {
        var @event = Event.Schedule("Title", null, SampleRange, OrganizerId, [AttendeeId], NowUtc);

        @event.RecordAttendeeResponse(AttendeeId, ResponseStatus.Accepted, NowUtc.AddMinutes(5));

        var eventAttendee = Assert.Single(@event.EventAttendees, ea => ea.AttendeeId == AttendeeId);
        Assert.Equal(ResponseStatus.Accepted, eventAttendee.ResponseStatus);
        Assert.Equal(NowUtc.AddMinutes(5), eventAttendee.RespondedAtUtc);
        Assert.Contains(@event.DomainEvents, e => e is AttendeeResponseRecordedDomainEvent);
    }

    [Fact]
    public void RecordAttendeeResponse_WhenNotInvited_Throws()
    {
        var @event = Event.Schedule("Title", null, SampleRange, OrganizerId, [], NowUtc);
        var uninvitedAttendeeId = Guid.NewGuid();

        Assert.Throws<DomainException>(() =>
            @event.RecordAttendeeResponse(uninvitedAttendeeId, ResponseStatus.Accepted, NowUtc));
    }

    [Fact]
    public void RecordAttendeeResponse_WhenEventCancelled_Throws()
    {
        var @event = Event.Schedule("Title", null, SampleRange, OrganizerId, [AttendeeId], NowUtc);
        @event.Cancel(NowUtc);

        Assert.Throws<DomainException>(() =>
            @event.RecordAttendeeResponse(AttendeeId, ResponseStatus.Accepted, NowUtc));
    }
}
