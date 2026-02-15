using DecoratorEmailSample.Contracts;
using DecoratorEmailSample.Models;

namespace DecoratorEmailSample.Services;

public class LoggingEmailService : IEmailService
{
    private readonly Guid traceId = Guid.NewGuid();
    private readonly IEmailService next;

    public LoggingEmailService(IEmailService next)
    {
        this.next = next;
    }

    public bool SendEmail(Email email)
    {
        Console.WriteLine($"[{this.traceId}] Sending email...");
        var result = this.next.SendEmail(email);

        var message = result ? "Email sent!" : "Email was not sent.";
        Console.WriteLine($"[{this.traceId}] {message}");
        return result;
    }
}
