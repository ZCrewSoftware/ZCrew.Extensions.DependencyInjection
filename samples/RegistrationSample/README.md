# Registration Sample

This sample demonstrates the two ways `ZCrew.Extensions.DependencyInjection.Registration` turns your classes into service registrations, and prints what each approach registered so you can read **the registration code next to its result**.

## Overview

| Folder      | Approach                                                    | Good for                                                       |
|-------------|-------------------------------------------------------------|----------------------------------------------------------------|
| `Health/`   | Reflection scan: `Classes.FromThisAssembly().InSameNamespaceAs<T>().AsInterface()` | A family of types that share a shape (here, `IHealthCheck`).    |
| `Services/` | Compile-time `[Service]` source generator: `Services.FromThisAssembly()`           | Distinct types that each declare their own lifetime/key/types.  |

Both paths scan the same assembly but never overlap: the health checks carry no `[Service]` attribute, and the service types live outside `IHealthCheck`'s namespace.

`Printing/RegistrationPrinter.cs` runs each approach against a fresh `ServiceCollection` and prints a plain-text table of the resulting `ServiceDescriptor`s (lifetime, key, service type, implementation).

## Running the sample

```bash
dotnet run --project samples/RegistrationSample/RegistrationSample/RegistrationSample.csproj
```

> **Note:** This sample currently references the `Registration` and `Generator` projects locally (the latter wired as an analyzer) because the `[Service]` source generator has not shipped in the NuGet package yet. Once the v3 package is published, the two `ProjectReference`s can be replaced with a single `PackageReference` to `ZCrew.Extensions.DependencyInjection.Registration`.

## Example output

```
1. Health checks - Classes.FromThisAssembly().InSameNamespaceAs<T>()
===================================================================

Every concrete class in the same namespace as IHealthCheck is registered against the interface it implements. Drop a new health check into the Health folder and it joins the set with no registration change.

Registration code

    services.AddScoped(
        Classes.FromThisAssembly()
            .InSameNamespaceAs<IHealthCheck>()
            .AsInterface());

Registered services (3)

    Lifetime  Key  Service       Implementation
    --------  ---  ------------  --------------------
    Scoped    -    IHealthCheck  DatabaseHealthCheck
    Scoped    -    IHealthCheck  DiskSpaceHealthCheck
    Scoped    -    IHealthCheck  NetworkHealthCheck

2. Services - [Service] source generator
========================================

Each type in the Services folder carries its own [Service] attribute declaring its service types, lifetime, and key. The generator collects them into Services.FromThisAssembly() at compile time, so there is no assembly scanning at startup.

Registration code

    services.Add(Services.FromThisAssembly());

Registered services (9)

    Lifetime   Key       Service           Implementation
    ---------  --------  ----------------  -------------------
    Scoped     -         GreetingService   GreetingService
    Scoped     -         IGreetingService  (factory)
    Transient  -         GuidIdGenerator   GuidIdGenerator
    Transient  -         IIdGenerator      GuidIdGenerator
    Singleton  sendgrid  SendGridEmailSender  SendGridEmailSender
    Singleton  sendgrid  IEmailSender      (factory)
    Singleton  smtp      SmtpEmailSender   SmtpEmailSender
    Singleton  smtp      IEmailSender      (factory)
    Singleton  -         SystemClock       SystemClock
```

### Reading the table

- **One attribute becomes one or two rows.** The implementation is always registered against itself, plus each listed service type.
- **`(factory)`** marks a *forwarded* registration. For `Singleton`/`Scoped` services with more than one service type, the extra service types forward to the concrete registration so they all resolve to one shared instance.
- **Transient forwards directly.** A `Transient` lifetime registers each service type independently, so `IIdGenerator` points straight at `GuidIdGenerator` rather than a factory.
- **Keys** (`smtp`, `sendgrid`) carry through to the implementation and every forwarded service type.

## Key registration code

```csharp
// Health checks - one convention covers the whole IHealthCheck family:
services.AddScoped(
    Classes.FromThisAssembly()
        .InSameNamespaceAs<IHealthCheck>()
        .AsInterface());

// Services - each type declares itself with [Service]; the generator gathers them:
services.Add(Services.FromThisAssembly());
```
