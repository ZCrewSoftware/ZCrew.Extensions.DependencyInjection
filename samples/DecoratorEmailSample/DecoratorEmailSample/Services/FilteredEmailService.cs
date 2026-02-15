using DecoratorEmailSample.Contracts;
using DecoratorEmailSample.Models;

namespace DecoratorEmailSample.Services;

public class FilteredEmailService : IEmailService
{
    private readonly IEmailService next;
    private readonly string blockedDomain;

    public FilteredEmailService(IEmailService next, string blockedDomain)
    {
        this.next = next;
        this.blockedDomain = blockedDomain;
    }

    public bool SendEmail(Email email)
    {
        if (email.EmailAddress.EndsWith(this.blockedDomain))
        {
            Console.WriteLine($"Blocked email to '{email.EmailAddress}' since it ends with '{this.blockedDomain}'.");
            return false;
        }

        return this.next.SendEmail(email);
    }
}
