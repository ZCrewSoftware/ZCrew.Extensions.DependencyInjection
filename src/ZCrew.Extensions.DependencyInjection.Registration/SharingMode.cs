namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Controls how a single implementation registered against multiple service types shares its instance,
///     mirroring Castle Windsor's "shared component" model.
/// </summary>
public enum SharingMode
{
    /// <summary>
    ///     Default for <see cref="Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton"/> and
    ///     <see cref="Microsoft.Extensions.DependencyInjection.ServiceLifetime.Scoped"/>. The implementation is
    ///     registered once and every selected service type resolves to that single instance.
    /// </summary>
    SharedComponent,

    /// <summary>
    ///     Every service type is registered as a factory that resolves the implementation via
    ///     <see cref="System.IServiceProvider"/>. The implementation must already be registered — either as one of
    ///     the selected service types (e.g. via a separate <c>AsSelf()</c> selection) or separately by the caller —
    ///     otherwise resolution fails at runtime.
    /// </summary>
    Dependent,

    /// <summary>
    ///     Every service type is registered as its own independent descriptor. With a Singleton or Scoped lifetime
    ///     this produces one instance per service type rather than a shared instance. Default for
    ///     <see cref="Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient"/>.
    /// </summary>
    Independent,
}
