using Fixtures.SmallProject.Attributes;

namespace ZCrew.Extensions.DependencyInjection.Registration.UnitTests;

public class ServiceSelectorExtensionsAttributeTests
{
    [Fact]
    public void AsServicesFromAttribute_T_WhenAttributeProjected_ShouldRegisterServices()
    {
        // Arrange
        var source = Classes.From(typeof(ContractBase));

        // Act
        var result = source.AsServicesFromAttribute<ContractAttribute>(a => a.Contracts).ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(IProvidedServiceA), descriptor.ServiceType);
        Assert.Equal(typeof(ContractBase), descriptor.ImplementationType);
    }

    [Fact]
    public void AsServicesFromAttribute_T_WhenAttributeMissing_ShouldSkipType()
    {
        // Arrange
        var source = Classes.From(typeof(UnmarkedStore));

        // Act
        var result = source.AsServicesFromAttribute<ContractAttribute>(a => a.Contracts).ToServiceCollection();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void AsServicesFromAttributeOrSelf_T_WhenAttributeMissing_ShouldRegisterAsSelf()
    {
        // Arrange
        var source = Classes.From(typeof(UnmarkedStore));

        // Act
        var result = source.AsServicesFromAttributeOrSelf<ContractAttribute>(a => a.Contracts).ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(UnmarkedStore), descriptor.ServiceType);
        Assert.Equal(typeof(UnmarkedStore), descriptor.ImplementationType);
    }

    [Fact]
    public void AsServicesFromAttribute_T_WhenSelectorReturnsEmpty_ShouldSkipType()
    {
        // Arrange
        var source = Classes.From(typeof(ContractBase));

        // Act
        var result = source.AsServicesFromAttribute<ContractAttribute>(_ => []).ToServiceCollection();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void AsServicesFromAttributeOrSelf_T_WhenSelectorReturnsEmpty_ShouldRegisterAsSelf()
    {
        // Arrange
        var source = Classes.From(typeof(ContractBase));

        // Act
        var result = source.AsServicesFromAttributeOrSelf<ContractAttribute>(_ => []).ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(ContractBase), descriptor.ServiceType);
        Assert.Equal(typeof(ContractBase), descriptor.ImplementationType);
    }

    [Fact]
    public void AsServicesFromAttribute_T_WhenInheritedDefault_ShouldRegisterDerivedFromBase()
    {
        // Arrange
        var source = Classes.From(typeof(ContractDerived));

        // Act
        var result = source.AsServicesFromAttribute<ContractAttribute>(a => a.Contracts).ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(IProvidedServiceA), descriptor.ServiceType);
        Assert.Equal(typeof(ContractDerived), descriptor.ImplementationType);
    }

    [Fact]
    public void AsServicesFromAttribute_T_WhenInheritedFalse_ShouldSkipDerived()
    {
        // Arrange
        var source = Classes.From(typeof(ContractDerived));

        // Act
        var result = source.AsServicesFromAttribute<ContractAttribute>(false, a => a.Contracts).ToServiceCollection();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void AsServicesFromAttribute_T_WhenSelectorNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var selector = Classes.From(typeof(ContractBase));

        // Act
        var act = () => selector.AsServicesFromAttribute<ContractAttribute>(null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void AsServicesFromAttribute_WhenAttributeTypeProjected_ShouldRegisterServices()
    {
        // Arrange
        var source = Classes.From(typeof(ContractBase));

        // Act
        var result = source
            .AsServicesFromAttribute(typeof(ContractAttribute), a => ((ContractAttribute)a).Contracts)
            .ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(IProvidedServiceA), descriptor.ServiceType);
        Assert.Equal(typeof(ContractBase), descriptor.ImplementationType);
    }

    [Fact]
    public void AsServicesFromAttribute_WhenAttributeTypeMissing_ShouldSkipType()
    {
        // Arrange
        var source = Classes.From(typeof(UnmarkedStore));

        // Act
        var result = source
            .AsServicesFromAttribute(typeof(ContractAttribute), a => ((ContractAttribute)a).Contracts)
            .ToServiceCollection();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void AsServicesFromAttribute_WhenNonAttributeType_ShouldThrowArgumentException()
    {
        // Arrange
        var source = Classes.From(typeof(ContractBase)).AsServicesFromAttribute(typeof(string), _ => []);

        // Act
        var act = () => source.ToServiceCollection();

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void AsServicesFromAttribute_WhenAttributeTypeSelectorNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var selector = Classes.From(typeof(ContractBase));

        // Act
        var act = () => selector.AsServicesFromAttribute(typeof(ContractAttribute), null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }
}
