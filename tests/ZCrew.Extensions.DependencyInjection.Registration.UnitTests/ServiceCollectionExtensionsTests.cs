using Fixtures.SmallProject.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace ZCrew.Extensions.DependencyInjection.Registration.UnitTests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void Add_WhenCalledWithServiceCollection_ShouldAddDescriptors()
    {
        // Arrange
        var descriptors = new ServiceCollection();
        descriptors.AddTransient<ICustomerService, CustomerService>();

        var services = new ServiceCollection();

        // Act
        services.Add(descriptors);

        // Assert
        var single = Assert.Single(services);
        Assert.Equal(ServiceLifetime.Transient, single.Lifetime);
    }

    [Fact]
    public void AddServices_WhenCalledWithServiceCollection_ShouldAddDescriptors()
    {
        // Arrange
        var descriptors = new ServiceCollection();
        descriptors.AddTransient<ICustomerService, CustomerService>();

        var services = new ServiceCollection();

        // Act
        services.AddServices(descriptors);

        // Assert
        var single = Assert.Single(services);
        Assert.Equal(ServiceLifetime.Transient, single.Lifetime);
    }

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
        var selector = CreateMock<IServiceKeySelector>(descriptor);
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
        var source = CreateMock<IServiceSource>(ServiceDescriptor.Transient<ICustomerService, CustomerService>());
        var services = new ServiceCollection();

        // Act
        var result = services.AddSingleton(source);

        // Assert
        Assert.Same(services, result);
    }

    [Fact]
    public void AddScoped_WhenCalledWithServiceSource_ShouldAddDescriptorsWithScopedLifetime()
    {
        // Arrange
        var descriptor = ServiceDescriptor.Transient<ICustomerService, CustomerService>();
        var source = CreateMock<IServiceSource>(descriptor);
        var services = new ServiceCollection();

        // Act
        services.AddScoped(source);

        // Assert
        var single = Assert.Single(services);
        Assert.Equal(ServiceLifetime.Scoped, single.Lifetime);
    }

    [Fact]
    public void AddScoped_WhenCalledWithKeyedServiceSelector_ShouldAddDescriptorsWithScopedLifetime()
    {
        // Arrange
        var descriptor = ServiceDescriptor.Transient<ICustomerService, CustomerService>();
        var selector = CreateMock<IServiceKeySelector>(descriptor);
        var services = new ServiceCollection();

        // Act
        services.AddScoped(selector);

        // Assert
        var single = Assert.Single(services);
        Assert.Equal(ServiceLifetime.Scoped, single.Lifetime);
    }

    [Fact]
    public void AddScoped_WhenCalledWithServiceSelector_ShouldAddDescriptorsWithScopedLifetime()
    {
        // Arrange
        var descriptor = ServiceDescriptor.Transient<ICustomerService, CustomerService>();
        var selector = CreateMock<IServiceSelector>(descriptor);
        var services = new ServiceCollection();

        // Act
        services.AddScoped(selector);

        // Assert
        var single = Assert.Single(services);
        Assert.Equal(ServiceLifetime.Scoped, single.Lifetime);
    }

    [Fact]
    public void AddScoped_WhenCalledWithTypeFilter_ShouldAddDescriptorsWithScopedLifetime()
    {
        // Arrange
        var descriptor = ServiceDescriptor.Transient<ICustomerService, CustomerService>();
        var filter = CreateMock<ITypeFilter>(descriptor);
        var services = new ServiceCollection();

        // Act
        services.AddScoped(filter);

        // Assert
        var single = Assert.Single(services);
        Assert.Equal(ServiceLifetime.Scoped, single.Lifetime);
    }

    [Fact]
    public void AddScoped_WhenCalledWithTypeSelector_ShouldAddDescriptorsWithScopedLifetime()
    {
        // Arrange
        var descriptor = ServiceDescriptor.Transient<ICustomerService, CustomerService>();
        var selector = CreateMock<ITypeSelector>(descriptor);
        var services = new ServiceCollection();

        // Act
        services.AddScoped(selector);

        // Assert
        var single = Assert.Single(services);
        Assert.Equal(ServiceLifetime.Scoped, single.Lifetime);
    }

    [Fact]
    public void AddScoped_WhenCalledWithAssemblyTypeSelector_ShouldAddDescriptorsWithScopedLifetime()
    {
        // Arrange
        var descriptor = ServiceDescriptor.Transient<ICustomerService, CustomerService>();
        var selector = CreateMock<IAssemblyTypeSelector>(descriptor);
        var services = new ServiceCollection();

        // Act
        services.AddScoped(selector);

        // Assert
        var single = Assert.Single(services);
        Assert.Equal(ServiceLifetime.Scoped, single.Lifetime);
    }

    [Fact]
    public void AddScoped_WhenCalledWithSingletonAndTransientDescriptors_ShouldChangeAllLifetimesToScoped()
    {
        // Arrange
        var source = CreateMock<IServiceSource>(
            ServiceDescriptor.Singleton<ICustomerService, CustomerService>(),
            ServiceDescriptor.Transient<ICustomerService, CustomerService>()
        );
        var services = new ServiceCollection();

        // Act
        services.AddScoped(source);

        // Assert
        Assert.Equal(2, services.Count);
        Assert.All(services, d => Assert.Equal(ServiceLifetime.Scoped, d.Lifetime));
    }

    [Fact]
    public void AddScoped_WhenCalled_ShouldReturnSameServiceCollection()
    {
        // Arrange
        var source = CreateMock<IServiceSource>(ServiceDescriptor.Transient<ICustomerService, CustomerService>());
        var services = new ServiceCollection();

        // Act
        var result = services.AddScoped(source);

        // Assert
        Assert.Same(services, result);
    }

    [Fact]
    public void AddTransient_WhenCalledWithServiceSource_ShouldAddDescriptorsWithTransientLifetime()
    {
        // Arrange
        var descriptor = ServiceDescriptor.Singleton<ICustomerService, CustomerService>();
        var source = CreateMock<IServiceSource>(descriptor);
        var services = new ServiceCollection();

        // Act
        services.AddTransient(source);

        // Assert
        var single = Assert.Single(services);
        Assert.Equal(ServiceLifetime.Transient, single.Lifetime);
    }

    [Fact]
    public void AddTransient_WhenCalledWithKeyedServiceSelector_ShouldAddDescriptorsWithTransientLifetime()
    {
        // Arrange
        var descriptor = ServiceDescriptor.Singleton<ICustomerService, CustomerService>();
        var selector = CreateMock<IServiceKeySelector>(descriptor);
        var services = new ServiceCollection();

        // Act
        services.AddTransient(selector);

        // Assert
        var single = Assert.Single(services);
        Assert.Equal(ServiceLifetime.Transient, single.Lifetime);
    }

    [Fact]
    public void AddTransient_WhenCalledWithServiceSelector_ShouldAddDescriptorsWithTransientLifetime()
    {
        // Arrange
        var descriptor = ServiceDescriptor.Singleton<ICustomerService, CustomerService>();
        var selector = CreateMock<IServiceSelector>(descriptor);
        var services = new ServiceCollection();

        // Act
        services.AddTransient(selector);

        // Assert
        var single = Assert.Single(services);
        Assert.Equal(ServiceLifetime.Transient, single.Lifetime);
    }

    [Fact]
    public void AddTransient_WhenCalledWithTypeFilter_ShouldAddDescriptorsWithTransientLifetime()
    {
        // Arrange
        var descriptor = ServiceDescriptor.Singleton<ICustomerService, CustomerService>();
        var filter = CreateMock<ITypeFilter>(descriptor);
        var services = new ServiceCollection();

        // Act
        services.AddTransient(filter);

        // Assert
        var single = Assert.Single(services);
        Assert.Equal(ServiceLifetime.Transient, single.Lifetime);
    }

    [Fact]
    public void AddTransient_WhenCalledWithTypeSelector_ShouldAddDescriptorsWithTransientLifetime()
    {
        // Arrange
        var descriptor = ServiceDescriptor.Singleton<ICustomerService, CustomerService>();
        var selector = CreateMock<ITypeSelector>(descriptor);
        var services = new ServiceCollection();

        // Act
        services.AddTransient(selector);

        // Assert
        var single = Assert.Single(services);
        Assert.Equal(ServiceLifetime.Transient, single.Lifetime);
    }

    [Fact]
    public void AddTransient_WhenCalledWithAssemblyTypeSelector_ShouldAddDescriptorsWithTransientLifetime()
    {
        // Arrange
        var descriptor = ServiceDescriptor.Singleton<ICustomerService, CustomerService>();
        var selector = CreateMock<IAssemblyTypeSelector>(descriptor);
        var services = new ServiceCollection();

        // Act
        services.AddTransient(selector);

        // Assert
        var single = Assert.Single(services);
        Assert.Equal(ServiceLifetime.Transient, single.Lifetime);
    }

    [Fact]
    public void AddTransient_WhenCalledWithSingletonAndScopedDescriptors_ShouldChangeAllLifetimesToTransient()
    {
        // Arrange
        var source = CreateMock<IServiceSource>(
            ServiceDescriptor.Singleton<ICustomerService, CustomerService>(),
            ServiceDescriptor.Scoped<ICustomerService, CustomerService>()
        );
        var services = new ServiceCollection();

        // Act
        services.AddTransient(source);

        // Assert
        Assert.Equal(2, services.Count);
        Assert.All(services, d => Assert.Equal(ServiceLifetime.Transient, d.Lifetime));
    }

    [Fact]
    public void AddTransient_WhenCalled_ShouldReturnSameServiceCollection()
    {
        // Arrange
        var source = CreateMock<IServiceSource>(ServiceDescriptor.Transient<ICustomerService, CustomerService>());
        var services = new ServiceCollection();

        // Act
        var result = services.AddTransient(source);

        // Assert
        Assert.Same(services, result);
    }

    private static T CreateMock<T>(params ServiceDescriptor[] descriptors)
        where T : class, IServiceSource
    {
        var mock = Substitute.For<T>();
        mock.ToServiceCollection(Arg.Any<IServiceCollection>())
            .Returns(callInfo =>
            {
                var collection = callInfo.Arg<IServiceCollection>();
                foreach (var descriptor in descriptors)
                {
                    collection.Add(descriptor);
                }
                return collection;
            });
        return mock;
    }
}
