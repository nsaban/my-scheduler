using MyScheduler.Domain.Common;
using MyScheduler.Domain.ValueObjects;

namespace MyScheduler.Domain.UnitTests.ValueObjects;

public class EmailAddressTests
{
    [Theory]
    [InlineData("doctor@practice.com")]
    [InlineData("  doctor@practice.com  ")]
    public void Create_WhenValid_TrimsAndSucceeds(string input)
    {
        var email = EmailAddress.Create(input);

        Assert.Equal("doctor@practice.com", email.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("not-an-email")]
    [InlineData("missing-domain@")]
    [InlineData("@missing-local.com")]
    public void Create_WhenInvalidShape_Throws(string input)
    {
        Assert.Throws<DomainException>(() => EmailAddress.Create(input));
    }

    [Fact]
    public void Create_WhenExceedsMaxLength_Throws()
    {
        var tooLong = new string('a', EmailAddress.MaxLength) + "@practice.com";

        Assert.Throws<DomainException>(() => EmailAddress.Create(tooLong));
    }

    [Fact]
    public void Create_AtMaxLength_Succeeds()
    {
        var localPart = new string('a', EmailAddress.MaxLength - "@practice.com".Length);
        var value = $"{localPart}@practice.com";

        var email = EmailAddress.Create(value);

        Assert.Equal(EmailAddress.MaxLength, email.Value.Length);
    }
}
