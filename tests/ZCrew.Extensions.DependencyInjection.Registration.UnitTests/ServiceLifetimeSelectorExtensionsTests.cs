using System.Reflection;
using Fixtures.SmallProject.Application.Services;
using Fixtures.SmallProject.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration.UnitTests;

public class ServiceLifetimeSelectorExtensionsTests
{
    [Fact]
    public void AsSingleton_WhenCalled_ShouldSetAllDescriptorsToSingletonLifetime()
    {
        // Arrange
        var source = Classes.From(typeof(CustomerService)).AsInterface<ICustomerService>();

        // Act
        var result = source.AsSingleton().ToServiceCollection();

        // Assert
        Assert.NotEmpty(result);
        Assert.All(result, d => Assert.Equal(ServiceLifetime.Singleton, d.Lifetime));
    }

    [Fact]
    public void AsScoped_WhenCalled_ShouldSetAllDescriptorsToScopedLifetime()
    {
        // Arrange
        var source = Classes.From(typeof(CustomerService)).AsInterface<ICustomerService>();

        // Act
        var result = source.AsScoped().ToServiceCollection();

        // Assert
        Assert.NotEmpty(result);
        Assert.All(result, d => Assert.Equal(ServiceLifetime.Scoped, d.Lifetime));
    }

    [Fact]
    public void AsTransient_WhenCalled_ShouldSetAllDescriptorsToTransientLifetime()
    {
        // Arrange
        var source = Classes.From(typeof(CustomerService)).AsInterface<ICustomerService>();

        // Act
        var result = source.AsTransient().ToServiceCollection();

        // Assert
        Assert.NotEmpty(result);
        Assert.All(result, d => Assert.Equal(ServiceLifetime.Transient, d.Lifetime));
    }

    [Fact]
    public void AsLifetime_WhenCalled_ShouldIncludeRequestedServiceType()
    {
        // Arrange
        var source = Classes.From(typeof(CustomerService)).AsInterface<ICustomerService>();

        // Act
        var result = source.AsLifetime(ServiceLifetime.Scoped).ToServiceCollection();

        // Assert
        Assert.Contains(result, d => d.ServiceType == typeof(ICustomerService));
    }

    [Fact]
    public void AsLifetime_WhenSelectorProvided_ShouldApplyLifetimePerType()
    {
        // Arrange
        var source = Classes.From(typeof(ScopedLifetimeStore), typeof(TransientLifetimeStore)).AsSelf();

        // Act
        var result = source
            .AsLifetime(type =>
                type == typeof(TransientLifetimeStore) ? ServiceLifetime.Transient : ServiceLifetime.Scoped
            )
            .ToServiceCollection();

        // Assert
        Assert.Contains(
            result,
            d => d.ImplementationType == typeof(ScopedLifetimeStore) && d.Lifetime == ServiceLifetime.Scoped
        );
        Assert.Contains(
            result,
            d => d.ImplementationType == typeof(TransientLifetimeStore) && d.Lifetime == ServiceLifetime.Transient
        );
    }

    [Fact]
    public void AsLifetime_WhenSelectorNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var selector = Classes.From(typeof(ScopedLifetimeStore)).AsSelf();

        // Act
        var act = () => selector.AsLifetime(null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void AsLifetimeByAttribute_WhenProviderYieldsLifetime_ShouldSetLifetime()
    {
        // Arrange
        var source = Classes.From(typeof(ScopedLifetimeStore)).AsSelf();

        // Act
        var result = source.AsLifetimeByAttribute().ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
        Assert.Equal(typeof(ScopedLifetimeStore), descriptor.ImplementationType);
    }

    [Fact]
    public void AsLifetimeByAttribute_WhenNoProviderAttribute_ShouldFallBackToSingleton()
    {
        // Arrange
        var source = Classes.From(typeof(UnmarkedStore)).AsSelf();

        // Act
        var result = source.AsLifetimeByAttribute().ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
        Assert.Equal(typeof(UnmarkedStore), descriptor.ImplementationType);
    }

    [Fact]
    public void AsLifetimeByAttribute_WhenMixedTypes_ShouldApplyLifetimePerType()
    {
        // Arrange
        var source = Classes.From(typeof(ScopedLifetimeStore), typeof(UnmarkedStore)).AsSelf();

        // Act
        var result = source.AsLifetimeByAttribute().ToServiceCollection();

        // Assert
        Assert.Contains(
            result,
            d => d.ImplementationType == typeof(ScopedLifetimeStore) && d.Lifetime == ServiceLifetime.Scoped
        );
        Assert.Contains(
            result,
            d => d.ImplementationType == typeof(UnmarkedStore) && d.Lifetime == ServiceLifetime.Singleton
        );
    }

    [Fact]
    public void AsLifetimeByAttribute_WhenAttributeIsNonInherited_ShouldApplyOnlyToDeclaringType()
    {
        // Arrange — [Lifetime] is declared Inherited = false, so the lifetime does not flow to derived types
        var source = Classes.From(typeof(LifetimeBase), typeof(LifetimeDerived)).AsSelf();

        // Act
        var result = source.AsLifetimeByAttribute().ToServiceCollection();

        // Assert
        Assert.Contains(
            result,
            d => d.ImplementationType == typeof(LifetimeBase) && d.Lifetime == ServiceLifetime.Scoped
        );
        Assert.Contains(
            result,
            d => d.ImplementationType == typeof(LifetimeDerived) && d.Lifetime == ServiceLifetime.Singleton
        );
    }

    [Fact]
    public void AsLifetimeByAttribute_WhenInheritedDefaultAndAttributeInheritable_ShouldApplyToDerived()
    {
        // Arrange
        var source = Classes.From(typeof(InheritableLifetimeDerived)).AsSelf();

        // Act
        var result = source.AsLifetimeByAttribute().ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    [Fact]
    public void AsLifetimeByAttribute_WhenInheritedFalse_ShouldFallBackToSingleton()
    {
        // Arrange
        var source = Classes.From(typeof(InheritableLifetimeDerived)).AsSelf();

        // Act
        var result = source.AsLifetimeByAttribute(inherited: false).ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
    }

    [Fact]
    public void AsLifetimeByAttribute_WhenMultipleProviderAttributes_ShouldThrowAmbiguousMatch()
    {
        // Arrange
        var source = Classes.From(typeof(MultiLifetimeStore)).AsSelf().AsLifetimeByAttribute();

        // Act
        var act = () => source.ToServiceCollection();

        // Assert
        Assert.Throws<AmbiguousMatchException>(act);
    }

    [Fact]
    public void AsLifetimeByAttribute_T_WhenAttributeProjected_ShouldSetLifetime()
    {
        // Arrange
        var source = Classes.From(typeof(LifestyleBase)).AsSelf();

        // Act
        var result = source.AsLifetimeByAttribute<LifestyleAttribute>(a => a.Lifetime).ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    [Fact]
    public void AsLifetimeByAttribute_T_WhenMarkerInterface_ShouldSetLifetime()
    {
        // Arrange
        var source = Classes.From(typeof(LifestyleBase)).AsSelf();

        // Act
        var result = source.AsLifetimeByAttribute<ILifestyleAware>(a => a.Lifetime).ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    [Fact]
    public void AsLifetimeByAttribute_T_WhenAttributeMissing_ShouldFallBackToSingleton()
    {
        // Arrange
        var source = Classes.From(typeof(UnmarkedStore)).AsSelf();

        // Act
        var result = source.AsLifetimeByAttribute<LifestyleAttribute>(a => a.Lifetime).ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
    }

    [Fact]
    public void AsLifetimeByAttribute_T_WhenInheritedDefault_ShouldApplyToDerived()
    {
        // Arrange
        var source = Classes.From(typeof(LifestyleDerived)).AsSelf();

        // Act
        var result = source.AsLifetimeByAttribute<LifestyleAttribute>(a => a.Lifetime).ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    [Fact]
    public void AsLifetimeByAttribute_T_WhenInheritedFalse_ShouldFallBackToSingleton()
    {
        // Arrange
        var source = Classes.From(typeof(LifestyleDerived)).AsSelf();

        // Act
        var result = source.AsLifetimeByAttribute<LifestyleAttribute>(false, a => a.Lifetime).ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
    }

    [Fact]
    public void AsLifetimeByAttribute_T_WhenMultipleMatchingAttributes_ShouldThrowAmbiguousMatch()
    {
        // Arrange
        var source = Classes
            .From(typeof(MultiTagged))
            .AsSelf()
            .AsLifetimeByAttribute<TagAttribute>(_ => ServiceLifetime.Scoped);

        // Act
        var act = () => source.ToServiceCollection();

        // Assert
        Assert.Throws<AmbiguousMatchException>(act);
    }

    [Fact]
    public void AsLifetimeByAttribute_T_WhenSelectorNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var selector = Classes.From(typeof(LifestyleBase)).AsSelf();

        // Act
        var act = () => selector.AsLifetimeByAttribute<LifestyleAttribute>(null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void AsLifetimeByAttribute_WhenAttributeTypeProjected_ShouldSetLifetime()
    {
        // Arrange
        var source = Classes.From(typeof(LifestyleBase)).AsSelf();

        // Act
        var result = source
            .AsLifetimeByAttribute(typeof(LifestyleAttribute), a => ((LifestyleAttribute)a).Lifetime)
            .ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    [Fact]
    public void AsLifetimeByAttribute_WhenAttributeTypeMissing_ShouldFallBackToSingleton()
    {
        // Arrange
        var source = Classes.From(typeof(UnmarkedStore)).AsSelf();

        // Act
        var result = source
            .AsLifetimeByAttribute(typeof(LifestyleAttribute), a => ((LifestyleAttribute)a).Lifetime)
            .ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
    }

    [Fact]
    public void AsLifetimeByAttribute_WhenAttributeTypeInheritedFalse_ShouldFallBackToSingleton()
    {
        // Arrange
        var source = Classes.From(typeof(LifestyleDerived)).AsSelf();

        // Act
        var result = source
            .AsLifetimeByAttribute(typeof(LifestyleAttribute), false, a => ((LifestyleAttribute)a).Lifetime)
            .ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
    }

    [Fact]
    public void AsLifetimeByAttribute_WhenNonAttributeType_ShouldThrowArgumentException()
    {
        // Arrange
        var source = Classes
            .From(typeof(LifestyleBase))
            .AsSelf()
            .AsLifetimeByAttribute(typeof(string), _ => ServiceLifetime.Scoped);

        // Act
        var act = () => source.ToServiceCollection();

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void AsLifetimeByAttribute_WhenAttributeTypeSelectorNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var selector = Classes.From(typeof(LifestyleBase)).AsSelf();

        // Act
        var act = () => selector.AsLifetimeByAttribute(typeof(LifestyleAttribute), null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void AsLifetimeByAttribute_WhenTransientAndMultipleInterfaces_ShouldRegisterIndependently()
    {
        // Arrange
        var source = Classes.From(typeof(TransientMultiStore)).AsAllInterfaces();

        // Act
        var result = source.AsLifetimeByAttribute().ToServiceCollection();

        // Assert
        Assert.All(result, d => Assert.Null(d.ImplementationFactory));
        Assert.All(result, d => Assert.Equal(typeof(TransientMultiStore), d.ImplementationType));
        Assert.All(result, d => Assert.Equal(ServiceLifetime.Transient, d.Lifetime));
    }

    [Fact]
    public void AsLifetimeByAttribute_WhenSingletonAndMultipleInterfaces_ShouldApplySingletonToEachIndependently()
    {
        // Arrange — the implementation is not one of the selected services, so each interface is registered
        // independently rather than sharing a single instance.
        var services = Classes
            .From(typeof(SingletonMultiStore))
            .AsAllInterfaces()
            .AsLifetimeByAttribute()
            .ToServiceCollection();
        using var provider = services.BuildServiceProvider();

        // Act
        var alpha = provider.GetRequiredService<ILifetimeAlpha>();
        var beta = provider.GetRequiredService<ILifetimeBeta>();

        // Assert
        Assert.NotSame(alpha, beta);
        Assert.All(services, d => Assert.Equal(ServiceLifetime.Singleton, d.Lifetime));
    }
}
