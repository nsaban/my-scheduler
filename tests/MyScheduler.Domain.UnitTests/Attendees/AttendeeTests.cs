using MyScheduler.Domain.Attendees;
using MyScheduler.Domain.Common;
using MyScheduler.Domain.ValueObjects;

namespace MyScheduler.Domain.UnitTests.Attendees;

public class AttendeeTests
{
    private static readonly EmailAddress SampleEmail = EmailAddress.Create("doctor@practice.com");
    private static readonly DateTime NowUtc = new(2026, 1, 1, 9, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Create_WhenValid_Succeeds()
    {
        var attendee = Attendee.Create("Dr. Smith", SampleEmail, NowUtc);

        Assert.Equal("Dr. Smith", attendee.Name);
        Assert.Equal(SampleEmail, attendee.Email);
        Assert.Equal(NowUtc, attendee.CreatedAtUtc);
        Assert.NotEqual(Guid.Empty, attendee.Id);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WhenNameMissing_Throws(string name)
    {
        Assert.Throws<DomainException>(() => Attendee.Create(name, SampleEmail, NowUtc));
    }

    [Fact]
    public void Create_WhenNameExceedsMaxLength_Throws()
    {
        var tooLong = new string('a', Attendee.MaxNameLength + 1);

        Assert.Throws<DomainException>(() => Attendee.Create(tooLong, SampleEmail, NowUtc));
    }

    [Fact]
    public void Create_AtMaxNameLength_Succeeds()
    {
        var name = new string('a', Attendee.MaxNameLength);

        var attendee = Attendee.Create(name, SampleEmail, NowUtc);

        Assert.Equal(Attendee.MaxNameLength, attendee.Name.Length);
    }
}
