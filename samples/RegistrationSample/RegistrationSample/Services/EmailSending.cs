using ZCrew.Extensions.DependencyInjection.Registration;

namespace RegistrationSample.Services;

/// <summary>
///     Sends an email message.
/// </summary>
public interface IEmailSender
{
    /// <summary>
    ///     Sends <paramref name="body"/> to <paramref name="to"/>.
    /// </summary>
    /// <param name="to">The recipient address.</param>
    /// <param name="body">The message body.</param>
    void Send(string to, string body);
}

/// <summary>
///     Registered against <see cref="IEmailSender"/> under the <c>"smtp"</c> key. Resolve it with
///     <c>GetRequiredKeyedService&lt;IEmailSender&gt;("smtp")</c>.
/// </summary>
[Service, As<IEmailSender>("smtp")]
public sealed class SmtpEmailSender : IEmailSender
{
    /// <inheritdoc />
    public void Send(string to, string body) => Console.WriteLine($"[smtp] to {to}: {body}");
}

/// <summary>
///     Registered against <see cref="IEmailSender"/> under the <c>"sendgrid"</c> key. Two senders share one
///     contract, told apart by their keys.
/// </summary>
[Service, As<IEmailSender>("sendgrid")]
public sealed class SendGridEmailSender : IEmailSender
{
    /// <inheritdoc />
    public void Send(string to, string body) => Console.WriteLine($"[sendgrid] to {to}: {body}");
}
