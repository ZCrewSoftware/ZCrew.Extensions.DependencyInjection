# Decorator email sample

This sample applies the decorator pattern to a mock email service with `ZCrew.Extensions.DependencyInjection`.

## What it does

An `IEmailService` is registered as a singleton, then two decorators are stacked on top of it:

1. `FilteredEmailService` blocks anything sent to an address ending in `@contoso.com`. It's registered through a factory with `AddSingletonDecorator` so the blocked domain can be passed to the constructor.
2. `LoggingEmailService` wraps each call with a trace ID and logs whether the email went out. It's registered with `AddScopedDecorator` so each scope gets a new trace ID.

Resolving the service gives you this call chain:

```
LoggingEmailService → FilteredEmailService → EmailService
```

## Setting it up

```xml
<ItemGroup>
  <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="10.0.0" />
  <PackageReference Include="ZCrew.Extensions.DependencyInjection" Version="3.0.0" />
</ItemGroup>
```

The decorator package is plain library code, so there's no analyzer or source generator in play and nothing else to configure. Add the reference, add `using ZCrew.Extensions.DependencyInjection;`, and the `AddDecorator` methods show up on `IServiceCollection`.

## Running it

```bash
dotnet run --project samples/DecoratorEmailSample/DecoratorEmailSample/DecoratorEmailSample.csproj
```

It asks for an email address and a message. Type `quit` to exit.

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

## The registration code

```csharp
var serviceCollection = new ServiceCollection();

// The service being decorated
serviceCollection.AddSingleton<IEmailService, EmailService>();

// A singleton decorator built by a factory, so we can pass the blocked domain
serviceCollection.AddSingletonDecorator<IEmailService>(
    (_, next) => new FilteredEmailService(next, "@contoso.com"));

// A scoped decorator registered by type
serviceCollection.AddScopedDecorator<IEmailService, LoggingEmailService>();
```
