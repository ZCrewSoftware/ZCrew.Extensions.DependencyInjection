namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Convenience filters for <see cref="ServiceFilter"/> that mirror the <see cref="TypeFilter"/> filters, each
///     applied to a service's <see cref="Service.ImplementationType"/>. Every filter returns a new
///     <see cref="ServiceFilter"/>, so they can be chained. To filter on the declared service types instead of the
///     implementation, use <see cref="ServiceFilter.Where"/> directly (for example
///     <c>filter.Where(service =&gt; service.ServiceTypes.Any(...))</c>).
/// </summary>
public static partial class ServiceFilterExtensions;
