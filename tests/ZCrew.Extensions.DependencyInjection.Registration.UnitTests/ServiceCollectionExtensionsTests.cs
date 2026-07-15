using Fixtures.SmallProject.Application.Services;
using Microsoft.Extensions.DependencyInjection;

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
    public void Add_WhenCalledWithServiceSource_ShouldAddChainDescriptors()
    {
        // Arrange
        ServiceSource source = Classes
            .From(typeof(CustomerService))
            .AsInterface<ICustomerService>()
            .AsSingleton();
        var services = new ServiceCollection();

        // Act
        services.Add(source);

        // Assert
        var single = Assert.Single(services);
        Assert.Equal(typeof(ICustomerService), single.ServiceType);
        Assert.Equal(ServiceLifetime.Singleton, single.Lifetime);
    }

    [Fact]
    public void AddServices_WhenCalledWithServiceSource_ShouldAddChainDescriptors()
    {
        // Arrange
        ServiceSource source = Classes.From(typeof(CustomerService)).AsInterface<ICustomerService>().AsScoped();
        var services = new ServiceCollection();

        // Act
        services.AddServices(source);

        // Assert
        var single = Assert.Single(services);
        Assert.Equal(ServiceLifetime.Scoped, single.Lifetime);
    }

    [Fact]
    public void AddSingleton_WhenCalledWithServiceSource_ShouldAddDescriptorsWithSingletonLifetime()
    {
        // Arrange
        ServiceSource source = Classes.From(typeof(CustomerService)).AsInterface<ICustomerService>().Unkeyed();
        var services = new ServiceCollection();

        // Act
        services.AddSingleton(source);

        // Assert
        var single = Assert.Single(services);
        Assert.Equal(ServiceLifetime.Singleton, single.Lifetime);
    }

    [Fact]
    public void AddSingleton_WhenCalledWithServiceLifetimeSelector_ShouldAddDescriptorsWithSingletonLifetime()
    {
        // Arrange
        ServiceLifetimeSelector selector = Classes
            .From(typeof(CustomerService))
            .AsInterface<ICustomerService>()
            .Unkeyed();
        var services = new ServiceCollection();

        // Act
        services.AddSingleton(selector);

        // Assert
        var single = Assert.Single(services);
        Assert.Equal(ServiceLifetime.Singleton, single.Lifetime);
    }

    [Fact]
    public void AddSingleton_WhenCalledWithServiceKeySelector_ShouldAddDescriptorsWithSingletonLifetime()
    {
        // Arrange
        ServiceKeySelector selector = Classes.From(typeof(CustomerService)).AsInterface<ICustomerService>();
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
        ServiceSelector selector = Classes.From(typeof(CustomerService)).AllTypes();
        var services = new ServiceCollection();

        // Act
        services.AddSingleton(selector);

        // Assert
        Assert.Contains(services, d => d.ServiceType == typeof(CustomerService));
        Assert.All(services, d => Assert.Equal(ServiceLifetime.Singleton, d.Lifetime));
    }

    [Fact]
    public void AddSingleton_WhenCalledWithTypeFilter_ShouldAddDescriptorsWithSingletonLifetime()
    {
        // Arrange
        TypeFilter filter = Classes.From(typeof(CustomerService));
        var services = new ServiceCollection();

        // Act
        services.AddSingleton(filter);

        // Assert
        Assert.Contains(services, d => d.ServiceType == typeof(CustomerService));
        Assert.All(services, d => Assert.Equal(ServiceLifetime.Singleton, d.Lifetime));
    }

    [Fact]
    public void AddSingleton_WhenCalledWithAssemblyTypeSelector_ShouldAddDescriptorsWithSingletonLifetime()
    {
        // Arrange
        AssemblyTypeSelector selector = Classes.FromAssemblyContaining<CustomerService>();
        var services = new ServiceCollection();

        // Act
        services.AddSingleton(selector);

        // Assert
        Assert.NotEmpty(services);
        Assert.All(services, d => Assert.Equal(ServiceLifetime.Singleton, d.Lifetime));
    }

    [Fact]
    public void AddSingleton_WhenCalled_ShouldReturnSameServiceCollection()
    {
        // Arrange
        ServiceSource source = Classes.From(typeof(CustomerService)).AsInterface<ICustomerService>().Unkeyed();
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
        ServiceSource source = Classes.From(typeof(CustomerService)).AsInterface<ICustomerService>().Unkeyed();
        var services = new ServiceCollection();

        // Act
        services.AddScoped(source);

        // Assert
        var single = Assert.Single(services);
        Assert.Equal(ServiceLifetime.Scoped, single.Lifetime);
    }

    [Fact]
    public void AddScoped_WhenCalledWithServiceLifetimeSelector_ShouldAddDescriptorsWithScopedLifetime()
    {
        // Arrange
        ServiceLifetimeSelector selector = Classes
            .From(typeof(CustomerService))
            .AsInterface<ICustomerService>()
            .Unkeyed();
        var services = new ServiceCollection();

        // Act
        services.AddScoped(selector);

        // Assert
        var single = Assert.Single(services);
        Assert.Equal(ServiceLifetime.Scoped, single.Lifetime);
    }

    [Fact]
    public void AddScoped_WhenCalledWithServiceKeySelector_ShouldAddDescriptorsWithScopedLifetime()
    {
        // Arrange
        ServiceKeySelector selector = Classes.From(typeof(CustomerService)).AsInterface<ICustomerService>();
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
        ServiceSelector selector = Classes.From(typeof(CustomerService)).AllTypes();
        var services = new ServiceCollection();

        // Act
        services.AddScoped(selector);

        // Assert
        Assert.Contains(services, d => d.ServiceType == typeof(CustomerService));
        Assert.All(services, d => Assert.Equal(ServiceLifetime.Scoped, d.Lifetime));
    }

    [Fact]
    public void AddScoped_WhenCalledWithTypeFilter_ShouldAddDescriptorsWithScopedLifetime()
    {
        // Arrange
        TypeFilter filter = Classes.From(typeof(CustomerService));
        var services = new ServiceCollection();

        // Act
        services.AddScoped(filter);

        // Assert
        Assert.Contains(services, d => d.ServiceType == typeof(CustomerService));
        Assert.All(services, d => Assert.Equal(ServiceLifetime.Scoped, d.Lifetime));
    }

    [Fact]
    public void AddScoped_WhenCalledWithAssemblyTypeSelector_ShouldAddDescriptorsWithScopedLifetime()
    {
        // Arrange
        AssemblyTypeSelector selector = Classes.FromAssemblyContaining<CustomerService>();
        var services = new ServiceCollection();

        // Act
        services.AddScoped(selector);

        // Assert
        Assert.NotEmpty(services);
        Assert.All(services, d => Assert.Equal(ServiceLifetime.Scoped, d.Lifetime));
    }

    [Fact]
    public void AddScoped_WhenCalled_ShouldReturnSameServiceCollection()
    {
        // Arrange
        ServiceSource source = Classes.From(typeof(CustomerService)).AsInterface<ICustomerService>().Unkeyed();
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
        ServiceSource source = Classes.From(typeof(CustomerService)).AsInterface<ICustomerService>().Unkeyed();
        var services = new ServiceCollection();

        // Act
        services.AddTransient(source);

        // Assert
        var single = Assert.Single(services);
        Assert.Equal(ServiceLifetime.Transient, single.Lifetime);
    }

    [Fact]
    public void AddTransient_WhenCalledWithServiceLifetimeSelector_ShouldAddDescriptorsWithTransientLifetime()
    {
        // Arrange
        ServiceLifetimeSelector selector = Classes
            .From(typeof(CustomerService))
            .AsInterface<ICustomerService>()
            .Unkeyed();
        var services = new ServiceCollection();

        // Act
        services.AddTransient(selector);

        // Assert
        var single = Assert.Single(services);
        Assert.Equal(ServiceLifetime.Transient, single.Lifetime);
    }

    [Fact]
    public void AddTransient_WhenCalledWithServiceKeySelector_ShouldAddDescriptorsWithTransientLifetime()
    {
        // Arrange
        ServiceKeySelector selector = Classes.From(typeof(CustomerService)).AsInterface<ICustomerService>();
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
        ServiceSelector selector = Classes.From(typeof(CustomerService)).AllTypes();
        var services = new ServiceCollection();

        // Act
        services.AddTransient(selector);

        // Assert
        Assert.Contains(services, d => d.ServiceType == typeof(CustomerService));
        Assert.All(services, d => Assert.Equal(ServiceLifetime.Transient, d.Lifetime));
    }

    [Fact]
    public void AddTransient_WhenCalledWithTypeFilter_ShouldAddDescriptorsWithTransientLifetime()
    {
        // Arrange
        TypeFilter filter = Classes.From(typeof(CustomerService));
        var services = new ServiceCollection();

        // Act
        services.AddTransient(filter);

        // Assert
        Assert.Contains(services, d => d.ServiceType == typeof(CustomerService));
        Assert.All(services, d => Assert.Equal(ServiceLifetime.Transient, d.Lifetime));
    }

    [Fact]
    public void AddTransient_WhenCalledWithAssemblyTypeSelector_ShouldAddDescriptorsWithTransientLifetime()
    {
        // Arrange
        AssemblyTypeSelector selector = Classes.FromAssemblyContaining<CustomerService>();
        var services = new ServiceCollection();

        // Act
        services.AddTransient(selector);

        // Assert
        Assert.NotEmpty(services);
        Assert.All(services, d => Assert.Equal(ServiceLifetime.Transient, d.Lifetime));
    }

    [Fact]
    public void AddTransient_WhenCalled_ShouldReturnSameServiceCollection()
    {
        // Arrange
        ServiceSource source = Classes.From(typeof(CustomerService)).AsInterface<ICustomerService>().Unkeyed();
        var services = new ServiceCollection();

        // Act
        var result = services.AddTransient(source);

        // Assert
        Assert.Same(services, result);
    }
}
