using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Marks a type as a service. Every service in the assembly is collected and can be registered with
///     <c>Services.FromThisAssembly()</c>. Each attribute instance is one registration: the implementation is
///     registered against itself plus any <see cref="ServiceTypes"/> (resolving to a single shared instance for
///     <see cref="ServiceLifetime.Singleton"/> and <see cref="ServiceLifetime.Scoped"/> lifetimes), with the
///     <see cref="Lifetime"/> and <see cref="Key"/> applied.
/// </summary>
/// <example>
///     Declare one or more registrations on the implementation:
///     <code>
///     [Service]
///     public class Clock;
///     <br/>
///     [Service(typeof(IFoo), typeof(IBar), Lifetime = ServiceLifetime.Scoped)]
///     public class FooBar : IFoo, IBar;
///     <br/>
///     [Service(typeof(IEmailSender), Key = "smtp")]
///     [Service(typeof(IEmailSender), Key = "ses")]
///     public class MyService : IFoo, IBar, IEmailSender;
///     </code>
///     Then you can you use the <c>ZCrew.Extensions.DependencyInjection.Registration.Services</c> entry point:
///     <code>
///     using ZCrew.Extensions.DependencyInjection.Registration;
///     public static class ServiceCollectionExtensions
///     {
///         public static void AddServices(this IServiceCollection services)
///         {
///             // Registers MyService based on the attributes
///             //   - Singleton Clock
///             //   - Scoped FooBar as IFoo, IBar
///             //   - Singleton Keyed "smtp" MyService as IEmailSender
///             //   - Singleton Keyed "ses" MyService as IEmailSender
///             services.Add(Services.FromThisAssembly());
///         }
///     }
///     </code>
/// </example>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = false)]
public sealed class ServiceAttribute : Attribute
{
    /// <summary>
    ///     Initializes a new <see cref="ServiceAttribute"/> registering the implementation against itself and the
    ///     given <paramref name="serviceTypes"/>.
    /// </summary>
    /// <param name="serviceTypes">The service types to register the implementation against, beyond itself.</param>
    public ServiceAttribute(params Type[] serviceTypes)
    {
        ServiceTypes = serviceTypes ?? [];
    }

    /// <summary>
    ///     The service types the implementation is registered against, beyond itself.
    /// </summary>
    public IReadOnlyList<Type> ServiceTypes { get; }

    /// <summary>
    ///     The lifetime for the registration. Defaults to <see cref="ServiceLifetime.Singleton"/>.
    /// </summary>
    public ServiceLifetime Lifetime { get; init; } = ServiceLifetime.Singleton;

    /// <summary>
    ///     The optional service key. When set, the implementation and its service types are registered as keyed
    ///     services under this key.
    /// </summary>
    public object? Key { get; init; }
}
