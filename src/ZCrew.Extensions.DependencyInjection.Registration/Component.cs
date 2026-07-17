using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Entry point for registering a single concrete type against multiple services. For
///     <see cref="ServiceLifetime.Singleton"/> and <see cref="ServiceLifetime.Scoped"/> components, all services
///     resolved from the service provider will share the same instance.
/// </summary>
public static class Component
{
    /// <summary>
    ///     Begins registration from the specified <paramref name="type"/>. The component is registered against the
    ///     <paramref name="type"/> itself; services added with <c>As</c> are forwarded to it.
    /// </summary>
    /// <param name="type">The type to build a component from.</param>
    public static ServiceComponent From(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type type
    )
    {
        return new ServiceComponent(type);
    }

    /// <summary>
    ///     Begins registration from the specified type parameter <typeparamref name="T"/>. The component is registered
    ///     against the type itself; services added with <c>As</c> are forwarded to it.
    /// </summary>
    /// <typeparam name="T">The type to build a component from.</typeparam>
    public static ServiceComponent From<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T
    >()
    {
        return new ServiceComponent(typeof(T));
    }
}
