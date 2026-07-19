using RegistrationSample.Health;
using RegistrationSample.Printing;
using ZCrew.Extensions.DependencyInjection.Registration;

Console.WriteLine(
    """
    Registration Sample
    ===================
    Two ways ZCrew.Extensions.DependencyInjection.Registration turns your classes into service registrations.

      1. Health checks - a reflection-based convention scan with Classes.FromThisAssembly(), for similar services.
      2. Services      - the compile-time [Service] source generator, for types that declare their own service types, lifetime, and key.
    """
);
Console.WriteLine();

// csharpier-ignore-start
// The examples just print whatever is between the opening { and closing }
RegistrationPrinter.Print(
    "Health checks using assembly scanning",
    services =>
    {
services.AddScoped(Classes
    .FromThisAssembly()
    .InSameNamespaceAs<IHealthCheck>()
    .AsInterface()
    .Keyed());
    });

RegistrationPrinter.Print(
    "[Service] source generator registration",
    services =>
    {
services.Add(Services.FromThisAssembly());
    });
// csharpier-ignore-end
