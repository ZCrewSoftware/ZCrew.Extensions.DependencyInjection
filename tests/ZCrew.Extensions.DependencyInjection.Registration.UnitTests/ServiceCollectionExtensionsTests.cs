using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Fixtures.SmallProject.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace ZCrew.Extensions.DependencyInjection.Registration.UnitTests;

[SuppressMessage("ReSharper", "GenericEnumeratorNotDisposed")]
public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddSingleton_WhenCalledWithServiceSource_ShouldAddDescriptorsWithSingletonLifetime()
    {
        // Arrange
        var descriptor = ServiceDescriptor.Transient<ICustomerService, CustomerService>();
        var source = CreateMock<IServiceSource>(descriptor);
        var services = new ServiceCollection();

        // Act
        services.AddSingleton(source);

        // Assert
        var single = Assert.Single(services);
        Assert.Equal(ServiceLifetime.Singleton, single.Lifetime);
    }

    [Fact]
    public void AddSingleton_WhenCalledWithKeyedServiceSelector_ShouldAddDescriptorsWithSingletonLifetime()
    {
        // Arrange
        var descriptor = ServiceDescriptor.Transient<ICustomerService, CustomerService>();
        var selector = CreateMock<IKeyedServiceSelector>(descriptor);
        var services = new ServiceCollection();

        // Act
        services.AddSingleton(selector);

        // Assert
        var single = Assert.Single(services);
        Assert.Equal(ServiceLifetime.Singleton, single.Lifetime);
    }

    [Fact]
    public void AddSingleton_WhenCalledWithServiceSelector_ShouldAddDescriptorsWithSingletonLifetime()
    {
        // Arrange
        var descriptor = ServiceDescriptor.Transient<ICustomerService, CustomerService>();
        var selector = CreateMock<IServiceSelector>(descriptor);
        var services = new ServiceCollection();

        // Act
        services.AddSingleton(selector);

        // Assert
        var single = Assert.Single(services);
        Assert.Equal(ServiceLifetime.Singleton, single.Lifetime);
    }

    [Fact]
    public void AddSingleton_WhenCalledWithTypeFilter_ShouldAddDescriptorsWithSingletonLifetime()
    {
        // Arrange
        var descriptor = ServiceDescriptor.Transient<ICustomerService, CustomerService>();
        var filter = CreateMock<ITypeFilter>(descriptor);
        var services = new ServiceCollection();

        // Act
        services.AddSingleton(filter);

        // Assert
        var single = Assert.Single(services);
        Assert.Equal(ServiceLifetime.Singleton, single.Lifetime);
    }

    [Fact]
    public void AddSingleton_WhenCalledWithTypeSelector_ShouldAddDescriptorsWithSingletonLifetime()
    {
        // Arrange
        var descriptor = ServiceDescriptor.Transient<ICustomerService, CustomerService>();
        var selector = CreateMock<ITypeSelector>(descriptor);
        var services = new ServiceCollection();

        // Act
        services.AddSingleton(selector);

        // Assert
        var single = Assert.Single(services);
        Assert.Equal(ServiceLifetime.Singleton, single.Lifetime);
    }

    [Fact]
    public void AddSingleton_WhenCalledWithAssemblyTypeSelector_ShouldAddDescriptorsWithSingletonLifetime()
    {
        // Arrange
        var descriptor = ServiceDescriptor.Transient<ICustomerService, CustomerService>();
        var selector = CreateMock<IAssemblyTypeSelector>(descriptor);
        var services = new ServiceCollection();

        // Act
        services.AddSingleton(selector);

        // Assert
        var single = Assert.Single(services);
        Assert.Equal(ServiceLifetime.Singleton, single.Lifetime);
    }

    [Fact]
    public void AddSingleton_WhenCalledWithTransientAndScopedDescriptors_ShouldChangeAllLifetimesToSingleton()
    {
        // Arrange
        var source = CreateMock<IServiceSource>(
            ServiceDescriptor.Transient<ICustomerService, CustomerService>(),
            ServiceDescriptor.Scoped<ICustomerService, CustomerService>()
        );
        var services = new ServiceCollection();

        // Act
        services.AddSingleton(source);

        // Assert
        Assert.Equal(2, services.Count);
        Assert.All(services, d => Assert.Equal(ServiceLifetime.Singleton, d.Lifetime));
    }

    [Fact]
    public void AddSingleton_WhenCalled_ShouldReturnSameServiceCollection()
    {
        // Arrange
        var source = CreateMock<IServiceSource>(
            ServiceDescriptor.Transient<ICustomerService, CustomerService>()
        );
        var services = new ServiceCollection();

        // Act
        var result = services.AddSingleton(source);

        // Assert
        Assert.Same(services, result);
    }

    private static T CreateMock<T>(params ServiceDescriptor[] descriptors)
        where T : class, IEnumerable<ServiceDescriptor>
    {
        var mock = Substitute.For<T>();
        IEnumerable<ServiceDescriptor> descriptorList = descriptors;
        mock.GetEnumerator().Returns(_ => descriptorList.GetEnumerator());
        return mock;
    }

}
