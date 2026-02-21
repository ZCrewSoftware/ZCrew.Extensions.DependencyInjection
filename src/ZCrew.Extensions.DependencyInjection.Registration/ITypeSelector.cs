namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Defines the type selection stage of the registration fluent API. Provides access to the concrete set of types
///     that will be filtered and registered.
/// </summary>
public partial interface ITypeSelector : ITypeFilter
{
    /// <summary>
    ///     Materializes the selected types as an enumerable sequence.
    /// </summary>
    internal IEnumerable<Type> SelectTypes();
}
