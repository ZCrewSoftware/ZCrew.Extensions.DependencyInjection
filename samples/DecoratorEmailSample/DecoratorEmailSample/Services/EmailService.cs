using DecoratorEmailSample.Contracts;
using DecoratorEmailSample.Models;

namespace DecoratorEmailSample.Services;

public class EmailService : IEmailService
{
    public bool SendEmail(Email email)
    {
        Console.WriteLine($"Sending email to '{email.EmailAddress}': '{email.Content}'.");
        return true;
    }
}
