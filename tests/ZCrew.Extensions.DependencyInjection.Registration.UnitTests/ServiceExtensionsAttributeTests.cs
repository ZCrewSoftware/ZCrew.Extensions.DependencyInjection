using System.Reflection;
using Fixtures.SmallProject.Attributes;

namespace ZCrew.Extensions.DependencyInjection.Registration.UnitTests;

public class ServiceExtensionsAttributeTests
{
    [Fact]
    public void AsServicesFromAttribute_WhenAttributeProvidesOneService_ShouldAddItAfterImplementation()
    {
        // Arrange
        var component = Service.From<SingleServiceStore>();

        // Act
        var result = component.AsServicesFromAttribute();

        // Assert
        Assert.Equal([typeof(SingleServiceStore), typeof(IProvidedServiceA)], result.ServiceTypes);
    }

    [Fact]
    public void AsServicesFromAttribute_WhenAttributeProvidesSeveralServices_ShouldAddAllInOrder()
    {
        // Arrange
        var component = Service.From<MultiServiceStore>();

        // Act
        var result = component.AsServicesFromAttribute();

        // Assert
        Assert.Equal(
            [typeof(MultiServiceStore), typeof(IProvidedServiceA), typeof(IProvidedServiceB)],
            result.ServiceTypes
        );
    }

    [Fact]
    public void AsServicesFromAttribute_WhenNoAttribute_ShouldRegisterImplementationAlone()
    {
        // Arrange
        var component = Service.From<UnmarkedStore>();

        // Act
        var result = component.AsServicesFromAttribute();

        // Assert — the chain registers nothing here; a component always keeps its implementation.
        Assert.Equal([typeof(UnmarkedStore)], result.ServiceTypes);
    }

    [Fact]
    public void AsServicesFromAttribute_WhenAttributeIsInheritedAndInheritedRequested_ShouldAddProvidedServices()
    {
        // Arrange
        var component = Service.From<InheritableServicesDerived>();

        // Act
        var result = component.AsServicesFromAttribute();

        // Assert
        Assert.Equal([typeof(InheritableServicesDerived), typeof(IProvidedServiceB)], result.ServiceTypes);
    }

    [Fact]
    public void AsServicesFromAttribute_WhenInheritedIsFalse_ShouldIgnoreInheritedAttribute()
    {
        // Arrange
        var component = Service.From<InheritableServicesDerived>();

        // Act
        var result = component.AsServicesFromAttribute(false);

        // Assert
        Assert.Equal([typeof(InheritableServicesDerived)], result.ServiceTypes);
    }

    [Fact]
    public void AsServicesFromAttribute_WhenAttributeIsNotInheritable_ShouldRegisterImplementationAlone()
    {
        // Arrange — [AsServices] is declared Inherited = false, so it does not flow to the derived type.
        var component = Service.From<ServicesDerived>();

        // Act
        var result = component.AsServicesFromAttribute();

        // Assert
        Assert.Equal([typeof(ServicesDerived)], result.ServiceTypes);
    }

    [Fact]
    public void AsServicesFromAttribute_WhenSeveralProviderAttributes_ShouldThrow()
    {
        // Act
        Action act = () => Service.From<MultiServiceProvidedStore>().AsServicesFromAttribute();

        // Assert
        Assert.Throws<AmbiguousMatchException>(act);
    }

    [Fact]
    public void AsServicesFromAttribute_WhenAttributeNamesUnrelatedService_ShouldThrow()
    {
        // Act — ContractBase declares a contract it does not implement. The chain accepts this unchecked; a
        // component validates eagerly.
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
        var component = Service.From<ContractStore>();

        // Act
        var result = component.AsServicesFromAttribute<ContractAttribute>(attribute => attribute.Contracts);

        // Assert
        Assert.Equal([typeof(ContractStore), typeof(IProvidedServiceA)], result.ServiceTypes);
    }

    [Fact]
    public void AsServicesFromAttribute_T_WhenInheritedIsFalse_ShouldIgnoreInheritedAttribute()
    {
        // Arrange
        var component = Service.From<ContractStoreDerived>();

        // Act
        var result = component.AsServicesFromAttribute<ContractAttribute>(false, attribute => attribute.Contracts);

        // Assert
        Assert.Equal([typeof(ContractStoreDerived)], result.ServiceTypes);
    }

    [Fact]
    public void AsServicesFromAttribute_T_WhenInheritedRequested_ShouldProjectInheritedAttribute()
    {
        // Arrange
        var component = Service.From<ContractStoreDerived>();

        // Act
        var result = component.AsServicesFromAttribute<ContractAttribute>(attribute => attribute.Contracts);

        // Assert
        Assert.Equal([typeof(ContractStoreDerived), typeof(IProvidedServiceA)], result.ServiceTypes);
    }

    [Fact]
    public void AsServicesFromAttribute_T_WhenNoAttribute_ShouldRegisterImplementationAlone()
    {
        // Arrange
        var component = Service.From<UnmarkedStore>();

        // Act
        var result = component.AsServicesFromAttribute<ContractAttribute>(attribute => attribute.Contracts);

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
        var component = Service.From<ContractStore>();

        // Act
        var result = component.AsServicesFromAttribute(
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
        var component = Service.From<ContractStoreDerived>();

        // Act
        var result = component.AsServicesFromAttribute(
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

    [Fact]
    public void AsServicesFromAttribute_WhenCalled_ShouldNotChangeImplementationType()
    {
        // Arrange
        var component = Service.From<SingleServiceStore>();

        // Act
        var result = component.AsServicesFromAttribute();

        // Assert
        Assert.Equal(typeof(SingleServiceStore), result.ImplementationType);
    }
}
