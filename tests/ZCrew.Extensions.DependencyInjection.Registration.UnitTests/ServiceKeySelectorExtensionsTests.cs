using System.Reflection;
using Fixtures.SmallProject.Attributes;

namespace ZCrew.Extensions.DependencyInjection.Registration.UnitTests;

public class ServiceKeySelectorExtensionsTests
{
    [Fact]
    public void KeyedByAttribute_T_WhenAttributeProjected_ShouldKeyDescriptor()
    {
        // Arrange
        var source = Classes.From(typeof(RegionalCustomerStore)).AsSelf();

        // Act
        var result = source.KeyedByAttribute<RegionCacheAttribute>(a => a.Region).ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.True(descriptor.IsKeyedService);
        Assert.Equal("customers", descriptor.ServiceKey);
        Assert.Equal(typeof(RegionalCustomerStore), descriptor.KeyedImplementationType);
    }

    [Fact]
    public void KeyedByAttribute_T_WhenMarkerInterface_ShouldKeyByProjection()
    {
        // Arrange
        var source = Classes.From(typeof(RegionalCustomerStore)).AsSelf();

        // Act
        var result = source.KeyedByAttribute<IRegionAware>(a => a.Region).ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.True(descriptor.IsKeyedService);
        Assert.Equal("customers", descriptor.ServiceKey);
    }

    [Fact]
    public void KeyedByAttribute_T_WhenAttributeMissing_ShouldLeaveUnkeyed()
    {
        // Arrange
        var source = Classes.From(typeof(UnmarkedStore)).AsSelf();

        // Act
        var result = source.KeyedByAttribute<RegionCacheAttribute>(a => a.Region).ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.False(descriptor.IsKeyedService);
        Assert.Equal(typeof(UnmarkedStore), descriptor.ImplementationType);
    }

    [Fact]
    public void KeyedByAttribute_T_WhenSelectorReturnsNull_ShouldLeaveUnkeyed()
    {
        // Arrange
        var source = Classes.From(typeof(RegionalCustomerStore)).AsSelf();

        // Act
        var result = source.KeyedByAttribute<RegionCacheAttribute>(_ => null).ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.False(descriptor.IsKeyedService);
        Assert.Equal(typeof(RegionalCustomerStore), descriptor.ImplementationType);
    }

    [Fact]
    public void KeyedByAttribute_T_WhenInheritedDefault_ShouldKeyDerivedFromBase()
    {
        // Arrange
        var source = Classes.From(typeof(TracedDerived)).AsSelf();

        // Act
        var result = source.KeyedByAttribute<TracedAttribute>(a => a.Channel).ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.True(descriptor.IsKeyedService);
        Assert.Equal("audit", descriptor.ServiceKey);
    }

    [Fact]
    public void KeyedByAttribute_T_WhenInheritedFalse_ShouldLeaveDerivedUnkeyed()
    {
        // Arrange
        var source = Classes.From(typeof(TracedDerived)).AsSelf();

        // Act
        var result = source.KeyedByAttribute<TracedAttribute>(false, a => a.Channel).ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.False(descriptor.IsKeyedService);
        Assert.Equal(typeof(TracedDerived), descriptor.ImplementationType);
    }

    [Fact]
    public void KeyedByAttribute_T_WhenMultipleMatchingAttributes_ShouldThrowAmbiguousMatch()
    {
        // Arrange
        var source = Classes.From(typeof(MultiTagged)).AsSelf().KeyedByAttribute<TagAttribute>(a => a.Name);

        // Act
        var act = () => source.ToServiceCollection();

        // Assert
        Assert.Throws<AmbiguousMatchException>(act);
    }

    [Fact]
    public void KeyedByAttribute_T_WhenSelectorNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var selector = Classes.From(typeof(RegionalCustomerStore)).AsSelf();

        // Act
        var act = () => selector.KeyedByAttribute<RegionCacheAttribute>(null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void KeyedByAttribute_WhenAttributeTypeProjected_ShouldKeyDescriptor()
    {
        // Arrange
        var source = Classes.From(typeof(RegionalCustomerStore)).AsSelf();

        // Act
        var result = source
            .KeyedByAttribute(typeof(RegionCacheAttribute), a => ((RegionCacheAttribute)a).Region)
            .ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.True(descriptor.IsKeyedService);
        Assert.Equal("customers", descriptor.ServiceKey);
        Assert.Equal(typeof(RegionalCustomerStore), descriptor.KeyedImplementationType);
    }

    [Fact]
    public void KeyedByAttribute_WhenAttributeTypeMissing_ShouldLeaveUnkeyed()
    {
        // Arrange
        var source = Classes.From(typeof(UnmarkedStore)).AsSelf();

        // Act
        var result = source
            .KeyedByAttribute(typeof(RegionCacheAttribute), a => ((RegionCacheAttribute)a).Region)
            .ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.False(descriptor.IsKeyedService);
        Assert.Equal(typeof(UnmarkedStore), descriptor.ImplementationType);
    }

    [Fact]
    public void KeyedByAttribute_WhenAttributeTypeInheritedDefault_ShouldKeyDerivedFromBase()
    {
        // Arrange
        var source = Classes.From(typeof(TracedDerived)).AsSelf();

        // Act
        var result = source
            .KeyedByAttribute(typeof(TracedAttribute), a => ((TracedAttribute)a).Channel)
            .ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.True(descriptor.IsKeyedService);
        Assert.Equal("audit", descriptor.ServiceKey);
    }

    [Fact]
    public void KeyedByAttribute_WhenAttributeTypeInheritedFalse_ShouldLeaveDerivedUnkeyed()
    {
        // Arrange
        var source = Classes.From(typeof(TracedDerived)).AsSelf();

        // Act
        var result = source
            .KeyedByAttribute(typeof(TracedAttribute), false, a => ((TracedAttribute)a).Channel)
            .ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.False(descriptor.IsKeyedService);
        Assert.Equal(typeof(TracedDerived), descriptor.ImplementationType);
    }

    [Fact]
    public void KeyedByAttribute_WhenNonAttributeType_ShouldThrowArgumentException()
    {
        // Arrange
        var source = Classes.From(typeof(RegionalCustomerStore)).AsSelf().KeyedByAttribute(typeof(string), a => a.ToString());

        // Act
        var act = () => source.ToServiceCollection();

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void KeyedByAttribute_WhenAttributeTypeSelectorNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var selector = Classes.From(typeof(RegionalCustomerStore)).AsSelf();

        // Act
        var act = () => selector.KeyedByAttribute(typeof(RegionCacheAttribute), null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }
}
