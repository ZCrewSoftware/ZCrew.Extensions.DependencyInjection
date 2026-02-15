# Decorator Email Sample

This sample demonstrates how to use `ZCrew.Extensions.DependencyInjection` to apply the decorator pattern to a mock email service.

## Overview

An `IEmailService` is registered as a singleton, then two decorators are stacked on top of it:

1. **`FilteredEmailService`** — blocks emails sent to addresses ending in `@contoso.com`. Registered via a factory delegate with `AddSingletonDecorator` so the blocked domain can be passed to the constructor.
2. **`LoggingEmailService`** — wraps each call with a trace ID and logs whether the email was sent. Registered with `AddScopedDecorator` so a new trace ID is generated per scope.

When the service is resolved, the call chain is:

```
LoggingEmailService → FilteredEmailService → EmailService
```

## Running the sample

```bash
dotnet run --project samples/DecoratorEmailSample/DecoratorEmailSample/DecoratorEmailSample.csproj
```

The app prompts for an email address and a message. Type `quit` to exit.

### Example output

```
Enter an email address:
user@example.com
Enter a short message:
Hello!

[3fa85f64-5717-4562-b3fc-2c963f66afa6] Sending email...
Sending email to 'user@example.com': 'Hello!'.
[3fa85f64-5717-4562-b3fc-2c963f66afa6] Email sent!
```

Sending to a blocked domain:

```
Enter an email address:
user@contoso.com
Enter a short message:
Hello!

[8b2e4f1a-9c3d-4e5f-a6b7-1234567890ab] Sending email...
Blocked email to 'user@contoso.com' since it ends with '@contoso.com'.
[8b2e4f1a-9c3d-4e5f-a6b7-1234567890ab] Email was not sent.
```

## Key registration code

```csharp
var serviceCollection = new ServiceCollection();

// Register the base service
serviceCollection.AddSingleton<IEmailService, EmailService>();

// Add a singleton decorator using a factory (to pass the blocked domain)
serviceCollection.AddSingletonDecorator<IEmailService>(
    (_, next) => new FilteredEmailService(next, "@contoso.com"));

// Add a scoped decorator using a type registration
serviceCollection.AddScopedDecorator<IEmailService, LoggingEmailService>();
```