using Fixtures.SmallProject.Attributes;

namespace ZCrew.Extensions.DependencyInjection.Registration.UnitTests;

public class ServiceExtensionsAttributeTests
{
    [Fact]
    public void AsServicesFromAttribute_WhenAttributeNamesUnrelatedService_ShouldThrow()
    {
        // Act — ContractBase declares a contract it does not implement. The chain accepts this unchecked; a
        // service validates eagerly.
        Action act = () =>
            Service.From<ContractBase>().AsServicesFromAttribute<ContractAttribute>(attribute => attribute.Contracts);

        // Assert
        var exception = Assert.Throws<ArgumentException>(act);
        Assert.Contains("is not based on the service type", exception.Message);
    }

    [Fact]
    public void AsServicesFromAttribute_T_WhenAttributeProjectsServices_ShouldAddThem()
    {
        // Arrange
        var service = Service.From<ContractStore>();

        // Act
        var result = service.AsServicesFromAttribute<ContractAttribute>(attribute => attribute.Contracts);

        // Assert
        Assert.Equal([typeof(ContractStore), typeof(IProvidedServiceA)], result.ServiceTypes);
    }

    [Fact]
    public void AsServicesFromAttribute_T_WhenInheritedIsFalse_ShouldIgnoreInheritedAttribute()
    {
        // Arrange
        var service = Service.From<ContractStoreDerived>();

        // Act
        var result = service.AsServicesFromAttribute<ContractAttribute>(false, attribute => attribute.Contracts);

        // Assert
        Assert.Equal([typeof(ContractStoreDerived)], result.ServiceTypes);
    }

    [Fact]
    public void AsServicesFromAttribute_T_WhenInheritedRequested_ShouldProjectInheritedAttribute()
    {
        // Arrange
        var service = Service.From<ContractStoreDerived>();

        // Act
        var result = service.AsServicesFromAttribute<ContractAttribute>(attribute => attribute.Contracts);

        // Assert
        Assert.Equal([typeof(ContractStoreDerived), typeof(IProvidedServiceA)], result.ServiceTypes);
    }

    [Fact]
    public void AsServicesFromAttribute_T_WhenNoAttribute_ShouldRegisterImplementationAlone()
    {
        // Arrange
        var service = Service.From<UnmarkedStore>();

        // Act
        var result = service.AsServicesFromAttribute<ContractAttribute>(attribute => attribute.Contracts);

        // Assert
        Assert.Equal([typeof(UnmarkedStore)], result.ServiceTypes);
    }

    [Fact]
    public void AsServicesFromAttribute_T_WhenServiceSelectorIsNull_ShouldThrow()
    {
        // Act
        Action act = () => Service.From<ContractStore>().AsServicesFromAttribute<ContractAttribute>(null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void AsServicesFromAttribute_WithAttributeType_WhenAttributeProjectsServices_ShouldAddThem()
    {
        // Arrange
        var service = Service.From<ContractStore>();

        // Act
        var result = service.AsServicesFromAttribute(
            typeof(ContractAttribute),
            attribute => ((ContractAttribute)attribute).Contracts
        );

        // Assert
        Assert.Equal([typeof(ContractStore), typeof(IProvidedServiceA)], result.ServiceTypes);
    }

    [Fact]
    public void AsServicesFromAttribute_WithAttributeType_WhenInheritedIsFalse_ShouldIgnoreInheritedAttribute()
    {
        // Arrange
        var service = Service.From<ContractStoreDerived>();

        // Act
        var result = service.AsServicesFromAttribute(
            typeof(ContractAttribute),
            false,
            attribute => ((ContractAttribute)attribute).Contracts
        );

        // Assert
        Assert.Equal([typeof(ContractStoreDerived)], result.ServiceTypes);
    }

    [Fact]
    public void AsServicesFromAttribute_WithAttributeType_WhenServiceSelectorIsNull_ShouldThrow()
    {
        // Act
        Action act = () => Service.From<ContractStore>().AsServicesFromAttribute(typeof(ContractAttribute), null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }
}
