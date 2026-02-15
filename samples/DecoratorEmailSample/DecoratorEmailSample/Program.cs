using System.Diagnostics.CodeAnalysis;
using DecoratorEmailSample.Contracts;
using DecoratorEmailSample.Models;
using DecoratorEmailSample.Services;
using Microsoft.Extensions.DependencyInjection;
using ZCrew.Extensions.DependencyInjection;

Console.WriteLine(
    """
    Decorator Email Sample
    ----------------------
    The intent of this sample is to demonstrate the ability to apply decorators to services when using dependency
    injection by mocking an email service. There is one base service 'EmailService' which will log the email contents.
    There are two decorators: 'FilteredEmailService' and 'LoggingEmailService' which have been set up to filter out any
    emails that end in "@contoso.com" and log the email information with a trace ID respectively.

    You will be prompted for a email address and then for a short message. You may type "quit" to exit.
    """
);
Console.WriteLine();

var serviceCollection = new ServiceCollection();
serviceCollection.AddSingleton<IEmailService, EmailService>();
serviceCollection.AddSingletonDecorator<IEmailService>((_, next) => new FilteredEmailService(next, "@contoso.com"));
serviceCollection.AddScopedDecorator<IEmailService, LoggingEmailService>();

while (true)
{
    try
    {
        if (!TryPrompt("Enter an email address: ", out var email))
        {
            break;
        }

        if (!TryPrompt("Enter a short message: ", out var content))
        {
            break;
        }

        Console.WriteLine();

        using var scope = serviceCollection.BuildServiceProvider();
        var emailService = scope.GetRequiredService<IEmailService>();
        emailService.SendEmail(new Email(email, content));

        Console.WriteLine();
    }
    catch (Exception e)
    {
        Console.WriteLine($"Unexpected error: {e.Message}");
        break;
    }
}

return;

bool TryPrompt(string message, [NotNullWhen(true)] out string? input)
{
    do
    {
        Console.WriteLine(message);
        input = Console.ReadLine();
    } while (input == null);

    if (input.Equals("quit", StringComparison.InvariantCultureIgnoreCase))
    {
        input = null;
        return false;
    }

    return true;
}
