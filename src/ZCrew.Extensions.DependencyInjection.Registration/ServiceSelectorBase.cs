namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Base class for service selectors. Provides bridge implementations of <see cref="IServiceKeySelector"/>
///     methods that route through <see cref="ServiceSelectorExtensions.AsSelf(IServiceSelector)"/>, so subclasses
///     only need to implement the <see cref="IServiceSelector"/> methods themselves.
/// </summary>
internal abstract class ServiceSelectorBase : ServiceKeySelectorBase, IServiceSelector
{
    /// <inheritdoc />
    public abstract IServiceKeySelector As(Func<Type, IEnumerable<Type>> serviceSelector);

    /// <inheritdoc />
    public abstract IServiceKeySelector As(Func<Type, IReadOnlyList<Type>, IEnumerable<Type>> serviceSelector);

    /// <inheritdoc />
    public override IServiceSource Unkeyed()
    {
        return this.AsSelf().Unkeyed();
    }

    /// <inheritdoc />
    public override IServiceSource Keyed()
    {
        return this.AsSelf().Keyed();
    }

    /// <inheritdoc />
    public override IServiceSource Keyed(object? serviceKey)
    {
        return this.AsSelf().Keyed(serviceKey);
    }

    /// <inheritdoc />
    public override IServiceSource Keyed(Func<Type, object?> serviceKeySelector)
    {
        return this.AsSelf().Keyed(serviceKeySelector);
    }

    /// <inheritdoc />
    public override IServiceSource Keyed(Func<Type, Type, object?> serviceKeySelector)
    {
        return this.AsSelf().Keyed(serviceKeySelector);
    }
}
