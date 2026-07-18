using Fixtures.SmallProject.Application.Ports;
using Fixtures.SmallProject.Application.Services;
using Fixtures.SmallProject.Domain.Entities;
using Fixtures.SmallProject.Domain.Repositories;
using Fixtures.SmallProject.Infrastructure.External;
using Fixtures.SmallProject.Infrastructure.Persistence;

namespace ZCrew.Extensions.DependencyInjection.Registration.UnitTests;

public class ServiceExtensionsTests
{
    [Fact]
    public void As_T_WhenCalled_ShouldAddServiceAfterImplementation()
    {
        // Arrange
        var service = Service.From<CustomerService>();

        // Act
        var result = service.As<ICustomerService>();

        // Assert
        Assert.Equal([typeof(CustomerService), typeof(ICustomerService)], result.ServiceTypes);
    }

    [Fact]
    public void As_T2_WhenCalled_ShouldAddBothServicesInOrder()
    {
        // Arrange
        var service = Service.From<SqlCustomerRepository>();

        // Act
        var result = service.As<ICustomerRepository, IRepository<Customer>>();

        // Assert
        Assert.Equal(
            [typeof(SqlCustomerRepository), typeof(ICustomerRepository), typeof(IRepository<Customer>)],
            result.ServiceTypes
        );
    }

    [Fact]
    public void As_T3_WhenCalled_ShouldAddAllServicesInOrder()
    {
        // Arrange
        var service = Service.From<SqlCustomerRepository>();

        // Act
        var result = service.As<ICustomerRepository, IRepository<Customer>, IReadOnlyRepository<Customer>>();

        // Assert
        Assert.Equal(
            [
                typeof(SqlCustomerRepository),
                typeof(ICustomerRepository),
                typeof(IRepository<Customer>),
                typeof(IReadOnlyRepository<Customer>),
            ],
            result.ServiceTypes
        );
    }

    [Fact]
    public void As_T4_WhenCalled_ShouldAddAllServicesInOrder()
    {
        // Arrange
        var service = Service.From<SqlCustomerRepository>();

        // Act
        var result = service.As<
            ICustomerRepository,
            IRepository<Customer>,
            IReadOnlyRepository<Customer>,
            IDisposable
        >();

        // Assert
        Assert.Equal(
            [
                typeof(SqlCustomerRepository),
                typeof(ICustomerRepository),
                typeof(IRepository<Customer>),
                typeof(IReadOnlyRepository<Customer>),
                typeof(IDisposable),
            ],
            result.ServiceTypes
        );
    }

    [Fact]
    public void As_T5_WhenCalled_ShouldAddAllServicesInOrder()
    {
        // Arrange
        var service = Service.From<SqlCustomerRepository>();

        // Act
        var result = service.As<
            ICustomerRepository,
            IRepository<Customer>,
            IReadOnlyRepository<Customer>,
            IDisposable,
            IAsyncDisposable
        >();

        // Assert
        Assert.Equal(
            [
                typeof(SqlCustomerRepository),
                typeof(ICustomerRepository),
                typeof(IRepository<Customer>),
                typeof(IReadOnlyRepository<Customer>),
                typeof(IDisposable),
                typeof(IAsyncDisposable),
            ],
            result.ServiceTypes
        );
    }

    [Fact]
    public void As_T6_WhenCalled_ShouldAddAllServicesInOrder()
    {
        // Arrange
        var service = Service.From<SqlCustomerRepository>();

        // Act
        var result = service.As<
            ICustomerRepository,
            IRepository<Customer>,
            IReadOnlyRepository<Customer>,
            IDisposable,
            IAsyncDisposable,
            RepositoryBase<Customer>
        >();

        // Assert
        Assert.Equal(
            [
                typeof(SqlCustomerRepository),
                typeof(ICustomerRepository),
                typeof(IRepository<Customer>),
                typeof(IReadOnlyRepository<Customer>),
                typeof(IDisposable),
                typeof(IAsyncDisposable),
                typeof(RepositoryBase<Customer>),
            ],
            result.ServiceTypes
        );
    }

    [Fact]
    public void As_T7_WhenCalled_ShouldAddAllServicesInOrder()
    {
        // Arrange
        var service = Service.From<SqlCustomerRepository>();

        // Act
        var result = service.As<
            ICustomerRepository,
            IRepository<Customer>,
            IReadOnlyRepository<Customer>,
            IDisposable,
            IAsyncDisposable,
            RepositoryBase<Customer>,
            object
        >();

        // Assert
        Assert.Equal(
            [
                typeof(SqlCustomerRepository),
                typeof(ICustomerRepository),
                typeof(IRepository<Customer>),
                typeof(IReadOnlyRepository<Customer>),
                typeof(IDisposable),
                typeof(IAsyncDisposable),
                typeof(RepositoryBase<Customer>),
                typeof(object),
            ],
            result.ServiceTypes
        );
    }

    [Fact]
    public void As_T8_WhenCalled_ShouldAddAllServicesInOrder()
    {
        // Arrange — SqlCustomerRepository has exactly eight base types counting itself, so the widest overload needs
        // the implementation as one of its type arguments. It is already seeded, hence the repeat.
        var service = Service.From<SqlCustomerRepository>();

        // Act
        var result = service.As<
            SqlCustomerRepository,
            ICustomerRepository,
            IRepository<Customer>,
            IReadOnlyRepository<Customer>,
            IDisposable,
            IAsyncDisposable,
            RepositoryBase<Customer>,
            object
        >();

        // Assert
        Assert.Equal(
            [
                typeof(SqlCustomerRepository),
                typeof(SqlCustomerRepository),
                typeof(ICustomerRepository),
                typeof(IRepository<Customer>),
                typeof(IReadOnlyRepository<Customer>),
                typeof(IDisposable),
                typeof(IAsyncDisposable),
                typeof(RepositoryBase<Customer>),
                typeof(object),
            ],
            result.ServiceTypes
        );
    }

    [Fact]
    public void As_T_WhenChained_ShouldAccumulateServices()
    {
        // Arrange
        var service = Service.From<PayPalPaymentGateway>();

        // Act
        var result = service.As<IPaymentGateway>().As<IDisposable>();

        // Assert
        Assert.Equal(
            [typeof(PayPalPaymentGateway), typeof(IPaymentGateway), typeof(IDisposable)],
            result.ServiceTypes
        );
    }

    [Fact]
    public void As_T_WhenMixedWithNonGenericOverload_ShouldAccumulateServices()
    {
        // Arrange
        var service = Service.From<PayPalPaymentGateway>();

        // Act
        var result = service.As<IPaymentGateway>().As(typeof(IDisposable));

        // Assert
        Assert.Equal(
            [typeof(PayPalPaymentGateway), typeof(IPaymentGateway), typeof(IDisposable)],
            result.ServiceTypes
        );
    }

    [Fact]
    public void As_T_WhenServiceIsAlreadySelected_ShouldKeepDuplicate()
    {
        // Arrange
        var service = Service.From<PayPalPaymentGateway>();

        // Act
        var result = service.As<IPaymentGateway>().As<IPaymentGateway>();

        // Assert — duplicates are kept on the service and collapsed when it is registered.
        Assert.Equal(
            [typeof(PayPalPaymentGateway), typeof(IPaymentGateway), typeof(IPaymentGateway)],
            result.ServiceTypes
        );
    }

    [Fact]
    public void As_T_WhenServiceIsNotBaseTypeOfImplementation_ShouldThrow()
    {
        // Act
        Action act = () => Service.From<CustomerService>().As<IPaymentGateway>();

        // Assert
        var exception = Assert.Throws<ArgumentException>(act);
        Assert.Contains("is not based on the service type", exception.Message);
    }

    [Fact]
    public void As_T2_WhenAnyServiceIsNotBaseTypeOfImplementation_ShouldThrow()
    {
        // Act
        Action act = () => Service.From<CustomerService>().As<ICustomerService, IPaymentGateway>();

        // Assert
        var exception = Assert.Throws<ArgumentException>(act);
        Assert.Contains("is not based on the service type", exception.Message);
    }

    [Fact]
    public void As_T_WhenCalled_ShouldNotChangeImplementationType()
    {
        // Arrange
        var service = Service.From<CustomerService>();

        // Act
        var result = service.As<ICustomerService>();

        // Assert
        Assert.Equal(typeof(CustomerService), result.ImplementationType);
    }
}
