using Fixtures.SmallProject.Application.Caching;
using Fixtures.SmallProject.Application.Services;
using Fixtures.SmallProject.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration.IntegrationTests.ClassesTests;

public class ClassesAttributeFilterTests
{
    [Fact]
    public void HasAttribute_WithAttributeType_ShouldFilterToDecoratedTypes()
    {
        // Act
        var result = Classes
            .From(typeof(RegionalCustomerStore), typeof(UnmarkedStore))
            .HasAttribute(typeof(RegionCacheAttribute))
            .AsSelf()
            .ToServiceCollection();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(RegionalCustomerStore), registeredTypes);
        Assert.DoesNotContain(typeof(UnmarkedStore), registeredTypes);
    }

    [Fact]
    public void HasAttribute_WithGenericAttribute_ShouldFilterToDecoratedTypes()
    {
        // Act
        var result = Classes
            .From(typeof(RegionalCustomerStore), typeof(UnmarkedStore))
            .HasAttribute<RegionCacheAttribute>()
            .AsSelf()
            .ToServiceCollection();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(RegionalCustomerStore), registeredTypes);
        Assert.DoesNotContain(typeof(UnmarkedStore), registeredTypes);
    }

    [Fact]
    public void HasAttribute_WhenNoTypeHasAttribute_ShouldReturnEmpty()
    {
        // Act
        var result = Classes
            .From(typeof(UnmarkedStore), typeof(RegionalCustomerStore))
            .HasAttribute<TracedAttribute>()
            .AsSelf()
            .ToServiceCollection();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void HasAttribute_WithInheritedTrue_ShouldMatchDerivedType()
    {
        // Act
        var result = Classes
            .From(typeof(TracedBase), typeof(TracedDerived))
            .HasAttribute(typeof(TracedAttribute), inherited: true)
            .AsSelf()
            .ToServiceCollection();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(TracedBase), registeredTypes);
        Assert.Contains(typeof(TracedDerived), registeredTypes);
    }

    [Fact]
    public void HasAttribute_WithInheritedFalse_ShouldMatchOnlyDeclaringType()
    {
        // Act
        var result = Classes
            .From(typeof(TracedBase), typeof(TracedDerived))
            .HasAttribute(typeof(TracedAttribute), inherited: false)
            .AsSelf()
            .ToServiceCollection();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(TracedBase), registeredTypes);
        Assert.DoesNotContain(typeof(TracedDerived), registeredTypes);
    }

    [Fact]
    public void HasAttribute_WithGenericInheritedTrue_ShouldMatchDerivedType()
    {
        // Act
        var result = Classes
            .From(typeof(TracedBase), typeof(TracedDerived))
            .HasAttribute<TracedAttribute>(inherited: true)
            .AsSelf()
            .ToServiceCollection();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(TracedBase), registeredTypes);
        Assert.Contains(typeof(TracedDerived), registeredTypes);
    }

    [Fact]
    public void HasAttribute_WithNonInheritedAttributeAndInheritedTrue_ShouldNotMatchDerivedType()
    {
        // Act
        var result = Classes
            .From(typeof(LocalOnlyBase), typeof(LocalOnlyDerived))
            .HasAttribute<LocalOnlyAttribute>(inherited: true)
            .AsSelf()
            .ToServiceCollection();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(LocalOnlyBase), registeredTypes);
        Assert.DoesNotContain(typeof(LocalOnlyDerived), registeredTypes);
    }

    [Fact]
    public void HasAttribute_WithAttributeTypeAndCondition_ShouldFilterByAttributeValue()
    {
        // Act
        var result = Classes
            .From(typeof(RegionalCustomerStore), typeof(RegionalOrderStore))
            .HasAttribute(typeof(RegionCacheAttribute), a => ((RegionCacheAttribute)a).Region == "customers")
            .AsSelf()
            .ToServiceCollection();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(RegionalCustomerStore), registeredTypes);
        Assert.DoesNotContain(typeof(RegionalOrderStore), registeredTypes);
    }

    [Fact]
    public void HasAttribute_WithGenericCondition_ShouldFilterByAttributeValue()
    {
        // Act
        var result = Classes
            .From(typeof(RegionalCustomerStore), typeof(RegionalOrderStore))
            .HasAttribute<RegionCacheAttribute>(a => a.Region == "customers")
            .AsSelf()
            .ToServiceCollection();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(RegionalCustomerStore), registeredTypes);
        Assert.DoesNotContain(typeof(RegionalOrderStore), registeredTypes);
    }

    [Fact]
    public void HasAttribute_WithConditionAndTypeMissingAttribute_ShouldExcludeType()
    {
        // Act
        var result = Classes
            .From(typeof(RegionalCustomerStore), typeof(UnmarkedStore))
            .HasAttribute<RegionCacheAttribute>(a => a.Region.Length > 0)
            .AsSelf()
            .ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(RegionalCustomerStore), descriptor.ImplementationType);
    }

    [Fact]
    public void HasAttribute_WithConditionAndInheritedTrue_ShouldMatchDerivedType()
    {
        // Act
        var result = Classes
            .From(typeof(TracedBase), typeof(TracedDerived))
            .HasAttribute(typeof(TracedAttribute), inherited: true, a => ((TracedAttribute)a).Channel == "audit")
            .AsSelf()
            .ToServiceCollection();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(TracedBase), registeredTypes);
        Assert.Contains(typeof(TracedDerived), registeredTypes);
    }

    [Fact]
    public void HasAttribute_WithGenericConditionAndInheritedFalse_ShouldNotMatchDerivedType()
    {
        // Act
        var result = Classes
            .From(typeof(TracedBase), typeof(TracedDerived))
            .HasAttribute<TracedAttribute>(inherited: false, a => a.Channel == "audit")
            .AsSelf()
            .ToServiceCollection();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(TracedBase), registeredTypes);
        Assert.DoesNotContain(typeof(TracedDerived), registeredTypes);
    }

    [Fact]
    public void HasAttributes_WithCondition_ShouldFilterByAttributeCount()
    {
        // Act
        var result = Classes
            .From(typeof(MultiTagged), typeof(SingleTagged))
            .HasAttributes(typeof(TagAttribute), attributes => attributes.Count() >= 2)
            .AsSelf()
            .ToServiceCollection();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(MultiTagged), registeredTypes);
        Assert.DoesNotContain(typeof(SingleTagged), registeredTypes);
    }

    [Fact]
    public void HasAttributes_WithGenericCondition_ShouldFilterByAttributeCount()
    {
        // Act
        var result = Classes
            .From(typeof(MultiTagged), typeof(SingleTagged))
            .HasAttributes<TagAttribute>(attributes => attributes.Count() >= 2)
            .AsSelf()
            .ToServiceCollection();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(MultiTagged), registeredTypes);
        Assert.DoesNotContain(typeof(SingleTagged), registeredTypes);
    }

    [Fact]
    public void HasAttributes_WithConditionRequiringAll_ShouldFilterByEveryAttribute()
    {
        // Act
        var result = Classes
            .From(typeof(MultiTagged), typeof(SingleTagged))
            .HasAttributes<TagAttribute>(attributes => attributes.All(t => t.Name.Length == 1))
            .AsSelf()
            .ToServiceCollection();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(MultiTagged), registeredTypes);
        Assert.DoesNotContain(typeof(SingleTagged), registeredTypes);
    }

    [Fact]
    public void HasAttributes_WithConditionAndInheritedTrue_ShouldMatchDerivedType()
    {
        // Act
        var result = Classes
            .From(typeof(TaggedDerived))
            .HasAttributes(typeof(TagAttribute), inherited: true, attributes => attributes.Count() >= 2)
            .AsSelf()
            .ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(TaggedDerived), descriptor.ImplementationType);
    }

    [Fact]
    public void HasAttributes_WithGenericConditionAndInheritedFalse_ShouldNotMatchDerivedType()
    {
        // Act
        var result = Classes
            .From(typeof(TaggedDerived))
            .HasAttributes<TagAttribute>(inherited: false, attributes => attributes.Count() >= 2)
            .AsSelf()
            .ToServiceCollection();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void HasAttribute_WithMarkerInterfaceType_ShouldMatchAllImplementingAttributes()
    {
        // Act
        var result = Classes
            .From(
                typeof(RegionalCustomerStore),
                typeof(RegionalOrderStore),
                typeof(PartitionedPaymentStore),
                typeof(UnmarkedStore)
            )
            .HasAttribute(typeof(IRegionAware))
            .AsSelf()
            .ToServiceCollection();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Equal(3, registeredTypes.Length);
        Assert.Contains(typeof(RegionalCustomerStore), registeredTypes);
        Assert.Contains(typeof(RegionalOrderStore), registeredTypes);
        Assert.Contains(typeof(PartitionedPaymentStore), registeredTypes);
    }

    [Fact]
    public void HasAttribute_WithMarkerInterfaceGeneric_ShouldMatchAllImplementingAttributes()
    {
        // Act
        var result = Classes
            .From(typeof(RegionalCustomerStore), typeof(PartitionedPaymentStore), typeof(UnmarkedStore))
            .HasAttribute<IRegionAware>()
            .AsSelf()
            .ToServiceCollection();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(RegionalCustomerStore), registeredTypes);
        Assert.Contains(typeof(PartitionedPaymentStore), registeredTypes);
        Assert.DoesNotContain(typeof(UnmarkedStore), registeredTypes);
    }

    [Fact]
    public void HasAttribute_WithMarkerInterfaceCondition_ShouldFilterByInterfaceProperty()
    {
        // Act
        var result = Classes
            .From(typeof(RegionalCustomerStore), typeof(RegionalOrderStore), typeof(PartitionedPaymentStore))
            .HasAttribute<IRegionAware>(a => a.Region == "customers")
            .AsSelf()
            .ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(RegionalCustomerStore), descriptor.ImplementationType);
    }

    [Fact]
    public void HasAttribute_WithNonAttributeType_ShouldThrowArgumentException()
    {
        // Arrange
        var filter = Classes.From(typeof(RegionalCustomerStore));

        // Act
        var act = () => filter.HasAttribute(typeof(string));

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void HasAttribute_WithNonAttributeGenericType_ShouldThrowArgumentException()
    {
        // Arrange
        var filter = Classes.From(typeof(RegionalCustomerStore));

        // Act
        var act = () => filter.HasAttribute<object>();

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void HasAttribute_WithNullAttributeType_ShouldThrowArgumentNullException()
    {
        // Arrange
        var filter = Classes.From(typeof(RegionalCustomerStore));

        // Act
        var act = () => filter.HasAttribute(null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void HasAttribute_WithNullCondition_ShouldThrowArgumentNullException()
    {
        // Arrange
        var filter = Classes.From(typeof(RegionalCustomerStore));

        // Act
        var act = () => filter.HasAttribute<RegionCacheAttribute>(null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void HasAttributes_WithNullCondition_ShouldThrowArgumentNullException()
    {
        // Arrange
        var filter = Classes.From(typeof(MultiTagged));

        // Act
        var act = () => filter.HasAttributes<TagAttribute>(null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void HasAttribute_WhenCombinedWithWhere_ShouldApplyBoth()
    {
        // Act
        var result = Classes
            .From(typeof(RegionalCustomerStore), typeof(RegionalOrderStore), typeof(PartitionedPaymentStore))
            .Where(t => t.Name.Contains("Customer"))
            .HasAttribute<IRegionAware>()
            .AsSelf()
            .ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(RegionalCustomerStore), descriptor.ImplementationType);
    }

    [Fact]
    public void HasAttribute_WhenCombinedWithBasedOn_ShouldApplyBoth()
    {
        // Act
        var result = Classes
            .From(typeof(CustomerService), typeof(CachingCustomerService))
            .BasedOn<ICustomerService>()
            .HasAttribute<CacheableAttribute>()
            .AsBase()
            .ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(CachingCustomerService), descriptor.ImplementationType);
        Assert.Equal(typeof(ICustomerService), descriptor.ServiceType);
    }

    [Fact]
    public void HasAttribute_WhenEnumeratedWithoutTerminalMethod_ShouldDefaultToSelfRegistration()
    {
        // Arrange
        var filter = Classes.From(typeof(RegionalCustomerStore), typeof(UnmarkedStore)).HasAttribute<RegionCacheAttribute>();

        // Act
        var result = filter.ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(RegionalCustomerStore), descriptor.ServiceType);
        Assert.Equal(typeof(RegionalCustomerStore), descriptor.ImplementationType);
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
    }
}
