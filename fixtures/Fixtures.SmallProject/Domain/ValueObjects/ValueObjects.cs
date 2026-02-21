namespace Fixtures.SmallProject.Domain.ValueObjects;

public class Address : IEquatable<Address>
{
    public bool Equals(Address? other) => false;
}

public class EmailAddress : IEquatable<EmailAddress>
{
    public bool Equals(EmailAddress? other) => false;
}

public class Money : IEquatable<Money>, IComparable<Money>
{
    public bool Equals(Money? other) => false;

    public int CompareTo(Money? other) => 0;
}
