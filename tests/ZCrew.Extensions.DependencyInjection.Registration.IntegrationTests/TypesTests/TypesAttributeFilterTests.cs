using Fixtures.SmallProject.Attributes;

namespace ZCrew.Extensions.DependencyInjection.Registration.IntegrationTests.TypesTests;

public class TypesAttributeFilterTests
{
    [Fact]
    public void HasAttribute_WithDecoratedInterface_ShouldMatch()
    {
        // Act
        var result = Types
            .From(typeof(IMarkedContract), typeof(MarkedClass), typeof(UnmarkedStore))
            .HasAttribute<MetaAttribute>()
            .AsSelf()
            .ToServiceCollection();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(IMarkedContract), registeredTypes);
        Assert.Contains(typeof(MarkedClass), registeredTypes);
        Assert.DoesNotContain(typeof(UnmarkedStore), registeredTypes);
    }

    [Fact]
    public void HasAttribute_WithDecoratedStruct_ShouldMatch()
    {
        // Act
        var result = Types
            .From(typeof(MarkedValue), typeof(UnmarkedStore))
            .HasAttribute<MetaAttribute>()
            .AsSelf()
            .ToServiceCollection();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(MarkedValue), registeredTypes);
        Assert.DoesNotContain(typeof(UnmarkedStore), registeredTypes);
    }

    [Fact]
    public void HasAttribute_WithDecoratedEnum_ShouldMatch()
    {
        // Act
        var result = Types
            .From(typeof(MarkedEnum), typeof(UnmarkedStore))
            .HasAttribute<MetaAttribute>()
            .AsSelf()
            .ToServiceCollection();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(MarkedEnum), registeredTypes);
        Assert.DoesNotContain(typeof(UnmarkedStore), registeredTypes);
    }

    [Fact]
    public void HasAttribute_WithClassesEntry_ShouldExcludeNonClassKinds()
    {
        // Act
        var result = Classes
            .From(typeof(IMarkedContract), typeof(MarkedValue), typeof(MarkedEnum), typeof(MarkedClass))
            .HasAttribute<MetaAttribute>()
            .AsSelf()
            .ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(MarkedClass), descriptor.ImplementationType);
    }

    [Fact]
    public void HasAttribute_WithMarkerInterfaceThroughTypes_ShouldMatchImplementingAttributes()
    {
        // Act
        var result = Types
            .From(typeof(RegionalCustomerStore), typeof(PartitionedPaymentStore), typeof(UnmarkedStore))
            .HasAttribute(typeof(IRegionAware))
            .AsSelf()
            .ToServiceCollection();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(RegionalCustomerStore), registeredTypes);
        Assert.Contains(typeof(PartitionedPaymentStore), registeredTypes);
        Assert.DoesNotContain(typeof(UnmarkedStore), registeredTypes);
    }
}
