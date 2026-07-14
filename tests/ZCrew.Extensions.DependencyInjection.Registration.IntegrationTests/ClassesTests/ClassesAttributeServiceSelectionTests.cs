using Fixtures.SmallProject.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration.IntegrationTests.ClassesTests;

public class ClassesAttributeServiceSelectionTests
{
    [Fact]
    public void AsServicesFromAttribute_WhenResolved_ShouldRegisterAndShareInstanceAcrossServiceTypes()
    {
        // Arrange
        var services = Classes.From(typeof(MultiServiceStore)).AsServicesFromAttribute().ToServiceCollection();
        var provider = services.BuildServiceProvider();

        // Act
        var a = provider.GetRequiredService<IProvidedServiceA>();
        var b = provider.GetRequiredService<IProvidedServiceB>();

        // Assert
        Assert.IsType<MultiServiceStore>(a);
        Assert.Same(a, b);
    }

    [Fact]
    public void AsServicesFromAttribute_WhenNoAttribute_ShouldNotRegisterService()
    {
        // Arrange
        var services = Classes.From(typeof(UnmarkedStore)).AsServicesFromAttribute().ToServiceCollection();
        var provider = services.BuildServiceProvider();

        // Act
        var resolved = provider.GetService<UnmarkedStore>();

        // Assert
        Assert.Null(resolved);
    }

    [Fact]
    public void AsServicesFromAttributeOrSelf_WhenNoAttribute_ShouldResolveAsSelf()
    {
        // Arrange
        var services = Classes.From(typeof(UnmarkedStore)).AsServicesFromAttributeOrSelf().ToServiceCollection();
        var provider = services.BuildServiceProvider();

        // Act
        var resolved = provider.GetRequiredService<UnmarkedStore>();

        // Assert
        Assert.IsType<UnmarkedStore>(resolved);
    }
}
