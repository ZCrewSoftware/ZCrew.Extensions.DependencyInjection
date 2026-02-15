using DecoratorEmailSample.Models;

namespace DecoratorEmailSample.Contracts;

public interface IEmailService
{
    bool SendEmail(Email email);
}
