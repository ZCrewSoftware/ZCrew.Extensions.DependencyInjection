using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Internal helpers for the <see cref="ServiceLifetime"/> enum.
/// </summary>
internal static class ServiceLifetimeExtensions
{
    extension(ServiceLifetime lifetime)
    {
        /// <summary>
        ///     The <see cref="SharingMode"/> applied by default for this lifetime.
        ///     <see cref="ServiceLifetime.Transient"/> maps to <see cref="SharingMode.Independent"/> — a transient
        ///     service constructs a new instance on every resolution, so sharing an instance is never meaningful.
        ///     All other lifetimes map to <see cref="SharingMode.SharedComponent"/>.
        /// </summary>
        internal SharingMode DefaultSharingMode()
        {
            return lifetime == ServiceLifetime.Transient ? SharingMode.Independent : SharingMode.SharedComponent;
        }
    }
}
