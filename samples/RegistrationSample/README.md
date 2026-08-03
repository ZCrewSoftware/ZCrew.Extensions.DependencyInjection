# Registration sample

This sample shows the two ways `ZCrew.Extensions.DependencyInjection.Registration` turns your classes into service registrations, and prints what each one registered so you can read the registration code next to its result.

## What's in here

| Folder      | Approach                                                                           | Good for                                                          |
|-------------|------------------------------------------------------------------------------------|-------------------------------------------------------------------|
| `Health/`   | Reflection scan: `Classes.FromThisAssembly().InSameNamespaceAs<T>().AsInterface()` | A family of types that share a shape, here `IHealthCheck`         |
| `Services/` | Compile-time `[Service]` source generator: `Services.FromThisAssembly()`           | Types that each declare their own lifetime, key and service types |

Both paths scan the same assembly but never overlap. The health checks carry no `[Service]` attribute, and the service types live outside the `IHealthCheck` namespace.

`Printing/RegistrationPrinter.cs` runs each approach against a fresh `ServiceCollection` and prints a table of the resulting `ServiceDescriptor`s: lifetime, key, service type, implementation.

## Setting it up

One package reference covers both approaches:

```xml
<ItemGroup>
  <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="10.0.0" />
  <PackageReference Include="ZCrew.Extensions.DependencyInjection.Registration" Version="3.0.0" />
</ItemGroup>
```

That's the whole setup, including the `[Service]` source generator. The generator ships inside the package as an analyzer, so NuGet wires it into the compiler for you. You don't need a second package, and you don't need `OutputItemType="Analyzer"` or any `PrivateAssets` juggling, which is what you'd normally write when pulling a generator in from a project reference.

To check it's actually loaded, look for `ZCrew.Extensions.DependencyInjection.Generator` under Dependencies → Analyzers in your IDE. If `[Service]` doesn't resolve or `Services.FromThisAssembly()` comes back as `CS0103`, the analyzer isn't loaded and no amount of `using` will fix it.

## Running it

```bash
dotnet run --project samples/RegistrationSample/RegistrationSample/RegistrationSample.csproj
```

## Example output

```
Registration Sample
===================
Two ways ZCrew.Extensions.DependencyInjection.Registration turns your classes into service registrations.

  1. Health checks - a reflection-based convention scan with Classes.FromThisAssembly(), for similar services.
  2. Services      - the compile-time [Service] source generator, for types that declare their own service types, lifetime, and key.

Health checks using assembly scanning
=====================================

services.AddScoped(Classes
    .FromThisAssembly()
    .InSameNamespaceAs<IHealthCheck>()
    .AsInterface()
    .Keyed());

 Lifetime | Key       | Service      | Implementation
----------|-----------|--------------|----------------------
 Scoped   | Database  | IHealthCheck | DatabaseHealthCheck
 Scoped   | DiskSpace | IHealthCheck | DiskSpaceHealthCheck
 Scoped   | Network   | IHealthCheck | NetworkHealthCheck
 Scoped   |           | IDisposable  | NetworkHealthCheck

[Service] source generator registration
=======================================

services.Add(Services.FromThisAssembly());

 Lifetime  | Key      | Service             | Implementation
-----------|----------|---------------------|---------------------
 Scoped    |          | GreetingService     | GreetingService
 Scoped    |          | IGreetingService    | (factory)
 Transient |          | GuidIdGenerator     | GuidIdGenerator
 Transient |          | IIdGenerator        | GuidIdGenerator
 Singleton |          | SendGridEmailSender | SendGridEmailSender
 Singleton | sendgrid | IEmailSender        | (factory)
 Singleton |          | SmtpEmailSender     | SmtpEmailSender
 Singleton | smtp     | IEmailSender        | (factory)
 Singleton |          | SystemClock         | SystemClock
```

### Reading the health check table

- The keys come from `.Keyed()`, which strips the service name off the implementation name. `DatabaseHealthCheck` registered as `IHealthCheck` leaves `Database`.
- `NetworkHealthCheck` shows up twice because it also implements `IDisposable`, and `AsInterface()` registers every top-level interface. Its `IDisposable` row is unkeyed, since `NetworkHealthCheck` doesn't end in `Disposable` and there's nothing to strip. Use `AsAllNonSystemInterfaces()` if you'd rather `System.*` interfaces stayed out of it.

### Reading the generator table

- One attribute becomes one or two rows. The implementation is always registered against itself, plus each service type it lists.
- `(factory)` marks a forwarded registration. For `Singleton` and `Scoped` services with more than one service type, the extras forward to the concrete registration so they all resolve to the same instance.
- Transients don't forward. Each service type is registered independently, which is why `IIdGenerator` points straight at `GuidIdGenerator` rather than a factory.
- Keys (`smtp`, `sendgrid`) sit on the `[As<IEmailSender>("smtp")]` attribute, so they land on the forwarded service type. The concrete `SmtpEmailSender` row stays unkeyed because no `[Keyed]` was applied to the implementation itself.

## The registration code

```csharp
// Health checks. One convention covers the whole IHealthCheck family:
services.AddScoped(Classes
    .FromThisAssembly()
    .InSameNamespaceAs<IHealthCheck>()
    .AsInterface()
    .Keyed());

// Services. Each type declares itself with [Service]; the generator gathers them up:
services.Add(Services.FromThisAssembly());
```
