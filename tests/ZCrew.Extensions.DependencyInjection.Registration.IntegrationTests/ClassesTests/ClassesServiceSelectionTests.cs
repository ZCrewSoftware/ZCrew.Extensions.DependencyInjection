using Fixtures.SmallProject.Application.Pipelines;
using Fixtures.SmallProject.Application.Ports;
using Fixtures.SmallProject.Application.Services;
using Fixtures.SmallProject.Domain.Entities;
using Fixtures.SmallProject.Domain.Repositories;
using Fixtures.SmallProject.Domain.ValueObjects;
using Fixtures.SmallProject.Infrastructure.External;
using Fixtures.SmallProject.Infrastructure.Notifications;
using Fixtures.SmallProject.Infrastructure.Persistence;

namespace ZCrew.Extensions.DependencyInjection.Registration.IntegrationTests.ClassesTests;

public class ClassesServiceSelectionTests
{
    [Fact]
    public void AsSelf_WhenCalled_ShouldRegisterAsImplementationType()
    {
        // Act
        var result = Classes.From(typeof(CustomerService)).AsSelf().ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(CustomerService), descriptor.ServiceType);
        Assert.Equal(typeof(CustomerService), descriptor.ImplementationType);
    }

    [Fact]
    public void AsAllInterfaces_WhenCalled_ShouldRegisterAllInterfaces()
    {
        // Act
        var result = Classes.From(typeof(PayPalPaymentGateway)).AsAllInterfaces().ToServiceCollection();

        // Assert
        var serviceTypes = result.Select(d => d.ServiceType).ToArray();
        Assert.Contains(typeof(IPaymentGateway), serviceTypes);
        Assert.Contains(typeof(IDisposable), serviceTypes);
    }

    [Fact]
    public void AsAllNonSystemInterfaces_WhenCalled_ShouldExcludeSystemInterfaces()
    {
        // Act
        var result = Classes.From(typeof(PayPalPaymentGateway)).AsAllNonSystemInterfaces().ToServiceCollection();

        // Assert
        var serviceTypes = result.Select(d => d.ServiceType).ToArray();
        Assert.Contains(typeof(IPaymentGateway), serviceTypes);
        Assert.DoesNotContain(typeof(IDisposable), serviceTypes);
    }

    [Fact]
    public void AsDefaultInterfaces_WhenCalled_ShouldMatchByNamingConvention()
    {
        // Act
        var result = Classes
            .From(typeof(CustomerService), typeof(EmailNotificationSender))
            .AsDefaultInterfaces()
            .ToServiceCollection();

        // Assert
        Assert.Contains(
            result,
            d => d.ImplementationType == typeof(CustomerService) && d.ServiceType == typeof(ICustomerService)
        );
        Assert.Contains(
            result,
            d => d.ImplementationType == typeof(EmailNotificationSender) && d.ServiceType == typeof(INotificationSender)
        );
    }

    [Fact]
    public void AsDefaultInterfaces_WhenNoConventionMatch_ShouldNotRegister()
    {
        // Act
        var result = Classes.From(typeof(Customer)).AsDefaultInterfaces().ToServiceCollection();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void AsDefaultNonSystemInterfaces_WhenCalled_ShouldCombineBothFilters()
    {
        // Act
        var result = Classes.From(typeof(PayPalPaymentGateway)).AsDefaultNonSystemInterfaces().ToServiceCollection();

        // Assert
        var serviceTypes = result.Select(d => d.ServiceType).ToArray();
        Assert.Contains(typeof(IPaymentGateway), serviceTypes);
        Assert.DoesNotContain(typeof(IDisposable), serviceTypes);
    }

    [Fact]
    public void AsAllTypes_WhenCalled_ShouldRegisterTypeAndNonAbstractBaseClassesAndInterfaces()
    {
        // Act
        var result = Classes.From(typeof(SqlCustomerRepository)).AsAllTypes().ToServiceCollection();

        // Assert
        var serviceTypes = result.Select(d => d.ServiceType).ToArray();
        Assert.Contains(typeof(SqlCustomerRepository), serviceTypes);
        Assert.Contains(typeof(ICustomerRepository), serviceTypes);
        Assert.Contains(typeof(IDisposable), serviceTypes);
        Assert.DoesNotContain(typeof(RepositoryBase<Customer>), serviceTypes);
    }

    [Fact]
    public void AsAllNonSystemTypes_WhenCalled_ShouldExcludeSystemTypesAndAbstractBaseClasses()
    {
        // Act
        var result = Classes.From(typeof(SqlCustomerRepository)).AsAllNonSystemTypes().ToServiceCollection();

        // Assert
        var serviceTypes = result.Select(d => d.ServiceType).ToArray();
        Assert.Contains(typeof(SqlCustomerRepository), serviceTypes);
        Assert.Contains(typeof(ICustomerRepository), serviceTypes);
        Assert.DoesNotContain(typeof(IDisposable), serviceTypes);
        Assert.DoesNotContain(typeof(IAsyncDisposable), serviceTypes);
        Assert.DoesNotContain(typeof(object), serviceTypes);
        Assert.DoesNotContain(typeof(RepositoryBase<Customer>), serviceTypes);
    }

    [Fact]
    public void AsDefaultTypes_WhenCalled_ShouldMatchByNamingConvention()
    {
        // Act
        var result = Classes
            .From(typeof(CustomerService), typeof(EmailNotificationSender))
            .AsDefaultTypes()
            .ToServiceCollection();

        // Assert
        var serviceTypes = result.Select(d => d.ServiceType).ToArray();
        Assert.Contains(typeof(ICustomerService), serviceTypes);
        Assert.Contains(typeof(INotificationSender), serviceTypes);
    }

    [Fact]
    public void AsDefaultTypes_WhenTypeHasNoBaseOrInterfaceMatchingConvention_ShouldRegisterOnlySelf()
    {
        // Act
        var result = Classes.From(typeof(Customer)).AsDefaultTypes().ToServiceCollection();

        // Assert
        var serviceTypes = result.Select(d => d.ServiceType).ToArray();
        Assert.Contains(typeof(Customer), serviceTypes);
        Assert.DoesNotContain(typeof(object), serviceTypes);
    }

    [Fact]
    public void AsDefaultNonSystemTypes_WhenCalled_ShouldCombineBothFilters()
    {
        // Act
        var result = Classes.From(typeof(SqlCustomerRepository)).AsDefaultNonSystemTypes().ToServiceCollection();

        // Assert
        var serviceTypes = result.Select(d => d.ServiceType).ToArray();
        Assert.Contains(typeof(ICustomerRepository), serviceTypes);
        Assert.DoesNotContain(typeof(IDisposable), serviceTypes);
        Assert.DoesNotContain(typeof(IAsyncDisposable), serviceTypes);
        Assert.DoesNotContain(typeof(RepositoryBase<Customer>), serviceTypes);
    }

    [Fact]
    public void AsAllTypes_WhenTypeExtendsConcreteBase_ShouldRegisterAgainstBaseAndInheritedInterfaces()
    {
        // Act
        var result = Classes.From(typeof(CachingPayPalPaymentGateway)).AsAllTypes().ToServiceCollection();

        // Assert
        var serviceTypes = result.Select(d => d.ServiceType).ToArray();
        Assert.Contains(typeof(CachingPayPalPaymentGateway), serviceTypes);
        Assert.Contains(typeof(PayPalPaymentGateway), serviceTypes);
        Assert.Contains(typeof(IPaymentGateway), serviceTypes);
        Assert.Contains(typeof(IDisposable), serviceTypes);
    }

    [Fact]
    public void AsAllNonSystemTypes_WhenTypeExtendsConcreteBase_ShouldExcludeSystemInheritedInterfaces()
    {
        // Act
        var result = Classes.From(typeof(CachingPayPalPaymentGateway)).AsAllNonSystemTypes().ToServiceCollection();

        // Assert
        var serviceTypes = result.Select(d => d.ServiceType).ToArray();
        Assert.Contains(typeof(CachingPayPalPaymentGateway), serviceTypes);
        Assert.Contains(typeof(PayPalPaymentGateway), serviceTypes);
        Assert.Contains(typeof(IPaymentGateway), serviceTypes);
        Assert.DoesNotContain(typeof(IDisposable), serviceTypes);
        Assert.DoesNotContain(typeof(object), serviceTypes);
    }

    [Fact]
    public void AsDefaultTypes_WhenBaseClassNameMatchesConvention_ShouldRegisterAgainstBase()
    {
        // Act
        var result = Classes.From(typeof(CachingPayPalPaymentGateway)).AsDefaultTypes().ToServiceCollection();

        // Assert
        var serviceTypes = result.Select(d => d.ServiceType).ToArray();
        Assert.Contains(typeof(PayPalPaymentGateway), serviceTypes);
        Assert.Contains(typeof(IPaymentGateway), serviceTypes);
    }

    [Fact]
    public void AsDefaultNonSystemTypes_WhenBaseClassNameMatchesConvention_ShouldRegisterAgainstBase()
    {
        // Act
        var result = Classes.From(typeof(CachingPayPalPaymentGateway)).AsDefaultNonSystemTypes().ToServiceCollection();

        // Assert
        var serviceTypes = result.Select(d => d.ServiceType).ToArray();
        Assert.Contains(typeof(PayPalPaymentGateway), serviceTypes);
        Assert.Contains(typeof(IPaymentGateway), serviceTypes);
        Assert.DoesNotContain(typeof(IDisposable), serviceTypes);
    }

    [Fact]
    public void AsFirstInterface_WhenCalled_ShouldRegisterFirstInterface()
    {
        // Act
        var result = Classes.From(typeof(CustomerService)).AsFirstInterface().ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(CustomerService), descriptor.ImplementationType);
        Assert.Contains(descriptor.ServiceType, typeof(CustomerService).GetInterfaces());
    }

    [Fact]
    public void AsFirstInterface_WhenNoInterfaces_ShouldNotRegister()
    {
        // Act
        var result = Classes.From(typeof(Customer)).AsFirstInterface().ToServiceCollection();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void AsInterface_WithBasedOn_ShouldRegisterTopLevelDerivedInterfaces()
    {
        // Act
        var result = Classes.From(typeof(SqlCustomerRepository)).BasedOn(typeof(IRepository<>)).AsInterface().ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(SqlCustomerRepository), descriptor.ImplementationType);
        Assert.Equal(typeof(ICustomerRepository), descriptor.ServiceType);
    }

    [Fact]
    public void AsInterface_WithGenericTypeArg_ShouldRegisterDerivedInterfaces()
    {
        // Act
        var result = Classes.From(typeof(PayPalPaymentGateway)).AsInterface<IPaymentGateway>().ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(PayPalPaymentGateway), descriptor.ImplementationType);
        Assert.Equal(typeof(IPaymentGateway), descriptor.ServiceType);
    }

    [Fact]
    public void AsInterface_WithExplicitType_ShouldRegisterDerivedInterfaces()
    {
        // Act
        var result = Classes.From(typeof(PayPalPaymentGateway)).AsInterface(typeof(IPaymentGateway)).ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(PayPalPaymentGateway), descriptor.ImplementationType);
        Assert.Equal(typeof(IPaymentGateway), descriptor.ServiceType);
    }

    [Fact]
    public void AsBase_WithBasedOn_ShouldRegisterAsBaseTypes()
    {
        // Act
        var result = Classes.From(typeof(CustomerService)).BasedOn<ICustomerService>().AsBase().ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(CustomerService), descriptor.ImplementationType);
        Assert.Equal(typeof(ICustomerService), descriptor.ServiceType);
    }

    [Fact]
    public void As_WithCustomSelector_ShouldUseProvidedFunction()
    {
        // Act
        var result = Classes.From(typeof(CustomerService)).As(type => type.GetInterfaces()).ToServiceCollection();

        // Assert
        var serviceTypes = result.Select(d => d.ServiceType).ToArray();
        Assert.Contains(typeof(ICustomerService), serviceTypes);
        Assert.All(result, d => Assert.Equal(typeof(CustomerService), d.ImplementationType));
    }

    [Fact]
    public void As_WithBaseTypeContext_ShouldReceiveResolvedBaseTypes()
    {
        // Act
        var result = Classes
            .From(typeof(CustomerService))
            .BasedOn<ICustomerService>()
            .As((_, bases) => bases)
            .ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(CustomerService), descriptor.ImplementationType);
        Assert.Equal(typeof(ICustomerService), descriptor.ServiceType);
    }

    [Fact]
    public void AsInterfaces_WithMultipleBaseTypes_ShouldRegisterDerivedInterfacesFromAll()
    {
        // Act
        var result = Classes
            .From(typeof(CustomerService), typeof(OrderService))
            .AsInterfaces(typeof(ICustomerService), typeof(IOrderService))
            .ToServiceCollection();

        // Assert
        Assert.Contains(
            result,
            d => d.ImplementationType == typeof(CustomerService) && d.ServiceType == typeof(ICustomerService)
        );
        Assert.Contains(
            result,
            d => d.ImplementationType == typeof(OrderService) && d.ServiceType == typeof(IOrderService)
        );
    }

    [Fact]
    public void AsInterfaces_WithOpenGeneric_ShouldRegisterTopLevelDerivedInterfaces()
    {
        // Act
        var result = Classes
            .From(typeof(SqlCustomerRepository), typeof(SqlOrderRepository))
            .AsInterfaces(typeof(IRepository<>))
            .ToServiceCollection();

        // Assert
        Assert.Contains(
            result,
            d => d.ImplementationType == typeof(SqlCustomerRepository) && d.ServiceType == typeof(ICustomerRepository)
        );
        Assert.Contains(
            result,
            d => d.ImplementationType == typeof(SqlOrderRepository) && d.ServiceType == typeof(IOrderRepository)
        );
    }

    [Fact]
    public void AsInterface_WithOpenGenericTypeArg_ShouldRegisterTopLevelDerivedInterfaces()
    {
        // Act
        var result = Classes.From(typeof(SqlCustomerRepository)).AsInterface(typeof(IRepository<>)).ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(SqlCustomerRepository), descriptor.ImplementationType);
        Assert.Equal(typeof(ICustomerRepository), descriptor.ServiceType);
    }

    [Fact]
    public void AsBase_WhenExtendingOpenGenericClassBase_ShouldRegisterConstructedForm()
    {
        // Act
        var result = Classes
            .From(typeof(SqlCustomerRepository))
            .BasedOn(typeof(RepositoryBase<>))
            .AsBase()
            .ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(SqlCustomerRepository), descriptor.ImplementationType);
        Assert.Equal(typeof(RepositoryBase<Customer>), descriptor.ServiceType);
    }

    [Fact]
    public void AsInterface_T_WhenTypeHasNoMatchingInterface_ShouldNotRegister()
    {
        // Act
        var result = Classes.From(typeof(CustomerService)).AsInterface<IPaymentGateway>().ToServiceCollection();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void AsBase_WhenOpenGenericImplementsNestedInterface_ShouldRegisterOpenForm()
    {
        // Act
        var result = Classes
            .From(typeof(LoggingStep<>))
            .BasedOn(typeof(Pipeline<>.IStep))
            .AsBase()
            .ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(LoggingStep<>), descriptor.ImplementationType);
        Assert.Equal(typeof(Pipeline<>.IStep), descriptor.ServiceType);
    }

    [Fact]
    public void AsBase_WhenClosedTypeImplementsNestedInterface_ShouldRegisterClosedForm()
    {
        // Act
        var result = Classes
            .From(typeof(OrderValidationStep))
            .BasedOn(typeof(Pipeline<>.IStep))
            .AsBase()
            .ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(OrderValidationStep), descriptor.ImplementationType);
        Assert.Equal(typeof(Pipeline<Order>.IStep), descriptor.ServiceType);
    }

    [Fact]
    public void AsInterface_WhenOpenGenericImplementsNestedInterface_ShouldRegisterOpenForm()
    {
        // Act
        var result = Classes
            .From(typeof(LoggingStep<>))
            .BasedOn(typeof(Pipeline<>.IStep))
            .AsInterface()
            .ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(LoggingStep<>), descriptor.ImplementationType);
        Assert.Equal(typeof(Pipeline<>.IStep), descriptor.ServiceType);
    }

    [Fact]
    public void AsInterface_WhenClosedTypeImplementsNestedInterface_ShouldRegisterClosedForm()
    {
        // Act
        var result = Classes
            .From(typeof(OrderValidationStep))
            .BasedOn(typeof(Pipeline<>.IStep))
            .AsInterface()
            .ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(OrderValidationStep), descriptor.ImplementationType);
        Assert.Equal(typeof(Pipeline<Order>.IStep), descriptor.ServiceType);
    }

    [Fact]
    public void AsAllInterfacesOrSelf_WhenInterfacesExist_ShouldRegisterAllInterfaces()
    {
        // Act
        var result = Classes.From(typeof(PayPalPaymentGateway)).AsAllInterfacesOrSelf().ToServiceCollection();

        // Assert
        var serviceTypes = result.Select(d => d.ServiceType).ToArray();
        Assert.Contains(typeof(IPaymentGateway), serviceTypes);
        Assert.Contains(typeof(IDisposable), serviceTypes);
    }

    [Fact]
    public void AsAllInterfacesOrSelf_WhenNoInterfaces_ShouldRegisterSelf()
    {
        // Act
        var result = Classes.From(typeof(Customer)).AsAllInterfacesOrSelf().ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(Customer), descriptor.ServiceType);
        Assert.Equal(typeof(Customer), descriptor.ImplementationType);
    }

    [Fact]
    public void AsAllNonSystemInterfacesOrSelf_WhenNonSystemInterfacesExist_ShouldRegisterNonSystemInterfaces()
    {
        // Act
        var result = Classes.From(typeof(PayPalPaymentGateway)).AsAllNonSystemInterfacesOrSelf().ToServiceCollection();

        // Assert
        var serviceTypes = result.Select(d => d.ServiceType).ToArray();
        Assert.Contains(typeof(IPaymentGateway), serviceTypes);
        Assert.DoesNotContain(typeof(IDisposable), serviceTypes);
        Assert.DoesNotContain(typeof(PayPalPaymentGateway), serviceTypes);
    }

    [Fact]
    public void AsAllNonSystemInterfacesOrSelf_WhenOnlySystemInterfaces_ShouldRegisterSelf()
    {
        // Act
        var result = Classes.From(typeof(Address)).AsAllNonSystemInterfacesOrSelf().ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(Address), descriptor.ServiceType);
        Assert.Equal(typeof(Address), descriptor.ImplementationType);
    }

    [Fact]
    public void AsDefaultInterfacesOrSelf_WhenConventionMatches_ShouldRegisterMatchingInterface()
    {
        // Act
        var result = Classes.From(typeof(CustomerService)).AsDefaultInterfacesOrSelf().ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(CustomerService), descriptor.ImplementationType);
        Assert.Equal(typeof(ICustomerService), descriptor.ServiceType);
    }

    [Fact]
    public void AsDefaultInterfacesOrSelf_WhenNoConventionMatch_ShouldRegisterSelf()
    {
        // Act
        var result = Classes.From(typeof(LegacyOrderProcessor)).AsDefaultInterfacesOrSelf().ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(LegacyOrderProcessor), descriptor.ServiceType);
        Assert.Equal(typeof(LegacyOrderProcessor), descriptor.ImplementationType);
    }

    [Fact]
    public void AsDefaultNonSystemInterfacesOrSelf_WhenConventionMatchesNonSystem_ShouldRegisterMatchingInterface()
    {
        // Act
        var result = Classes.From(typeof(CustomerService)).AsDefaultNonSystemInterfacesOrSelf().ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(CustomerService), descriptor.ImplementationType);
        Assert.Equal(typeof(ICustomerService), descriptor.ServiceType);
    }

    [Fact]
    public void AsDefaultNonSystemInterfacesOrSelf_WhenNoMatch_ShouldRegisterSelf()
    {
        // Act
        var result = Classes.From(typeof(Address)).AsDefaultNonSystemInterfacesOrSelf().ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(Address), descriptor.ServiceType);
        Assert.Equal(typeof(Address), descriptor.ImplementationType);
    }

    [Fact]
    public void AsFirstInterfaceOrSelf_WhenInterfacesExist_ShouldRegisterFirstInterface()
    {
        // Act
        var result = Classes.From(typeof(CustomerService)).AsFirstInterfaceOrSelf().ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(CustomerService), descriptor.ImplementationType);
        Assert.Contains(descriptor.ServiceType, typeof(CustomerService).GetInterfaces());
    }

    [Fact]
    public void AsFirstInterfaceOrSelf_WhenNoInterfaces_ShouldRegisterSelf()
    {
        // Act
        var result = Classes.From(typeof(Customer)).AsFirstInterfaceOrSelf().ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(Customer), descriptor.ServiceType);
        Assert.Equal(typeof(Customer), descriptor.ImplementationType);
    }

    [Fact]
    public void AsInterfaceOrSelf_WithBasedOn_ShouldRegisterTopLevelDerivedInterface()
    {
        // Act
        var result = Classes
            .From(typeof(SqlCustomerRepository))
            .BasedOn(typeof(IRepository<>))
            .AsInterfaceOrSelf()
            .ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(SqlCustomerRepository), descriptor.ImplementationType);
        Assert.Equal(typeof(ICustomerRepository), descriptor.ServiceType);
    }

    [Fact]
    public void AsInterfaceOrSelf_WhenNoMatchingInterface_ShouldRegisterSelf()
    {
        // Act
        var result = Classes.From(typeof(Customer)).AsInterfaceOrSelf().ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(Customer), descriptor.ServiceType);
        Assert.Equal(typeof(Customer), descriptor.ImplementationType);
    }

    [Fact]
    public void AsInterfaceOrSelf_T_WhenMatchingInterface_ShouldRegisterInterface()
    {
        // Act
        var result = Classes.From(typeof(CustomerService)).AsInterfaceOrSelf<ICustomerService>().ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(CustomerService), descriptor.ImplementationType);
        Assert.Equal(typeof(ICustomerService), descriptor.ServiceType);
    }

    [Fact]
    public void AsInterfaceOrSelf_T_WhenTypeHasNoMatchingInterface_ShouldRegisterSelf()
    {
        // Act
        var result = Classes.From(typeof(CustomerService)).AsInterfaceOrSelf<IPaymentGateway>().ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(CustomerService), descriptor.ServiceType);
        Assert.Equal(typeof(CustomerService), descriptor.ImplementationType);
    }

    [Fact]
    public void AsInterfaceOrSelf_WithExplicitType_WhenMatching_ShouldRegisterInterface()
    {
        // Act
        var result = Classes.From(typeof(CustomerService)).AsInterfaceOrSelf(typeof(ICustomerService)).ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(CustomerService), descriptor.ImplementationType);
        Assert.Equal(typeof(ICustomerService), descriptor.ServiceType);
    }

    [Fact]
    public void AsInterfaceOrSelf_WithExplicitType_WhenNoMatch_ShouldRegisterSelf()
    {
        // Act
        var result = Classes.From(typeof(CustomerService)).AsInterfaceOrSelf(typeof(IPaymentGateway)).ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(CustomerService), descriptor.ServiceType);
        Assert.Equal(typeof(CustomerService), descriptor.ImplementationType);
    }

    [Fact]
    public void AsInterfaceOrSelf_WithExplicitType_WhenTypeIsNull_ShouldThrow()
    {
        // Act
        var act = () => Classes.From(typeof(Customer)).AsInterfaceOrSelf(null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void AsInterfacesOrSelf_WithMultipleBaseTypes_ShouldRegisterDerivedInterfaces()
    {
        // Act
        var result = Classes
            .From(typeof(CustomerService), typeof(OrderService))
            .AsInterfacesOrSelf(typeof(ICustomerService), typeof(IOrderService))
            .ToServiceCollection();

        // Assert
        Assert.Contains(
            result,
            d => d.ImplementationType == typeof(CustomerService) && d.ServiceType == typeof(ICustomerService)
        );
        Assert.Contains(
            result,
            d => d.ImplementationType == typeof(OrderService) && d.ServiceType == typeof(IOrderService)
        );
    }

    [Fact]
    public void AsInterfacesOrSelf_WhenNoMatchingInterface_ShouldRegisterSelf()
    {
        // Act
        var result = Classes.From(typeof(Customer)).AsInterfacesOrSelf(typeof(ICustomerService)).ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(Customer), descriptor.ServiceType);
        Assert.Equal(typeof(Customer), descriptor.ImplementationType);
    }

    [Fact]
    public void AsInterfacesOrSelf_WhenInterfaceTypesIsNull_ShouldThrow()
    {
        // Act
        var act = () => Classes.From(typeof(Customer)).AsInterfacesOrSelf(null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void AsInterfaceOrSelf_WithMixedTypes_ShouldRegisterInterfacesAndSelfPerType()
    {
        // Act
        var result = Classes.From(typeof(CustomerService), typeof(Customer)).AsInterfaceOrSelf().ToServiceCollection();

        // Assert
        var serviceTypes = result.Select(d => d.ServiceType).ToArray();
        Assert.Contains(typeof(ICustomerService), serviceTypes);
        Assert.Contains(typeof(Customer), serviceTypes);
        Assert.DoesNotContain(typeof(CustomerService), serviceTypes);
    }

    [Fact]
    public void AsAllInterfaces_WhenNoInterfaces_ShouldNotRegister()
    {
        // Act
        var result = Classes.From(typeof(Customer)).AsAllInterfaces().ToServiceCollection();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void AsAllNonSystemInterfaces_WhenNoInterfaces_ShouldNotRegister()
    {
        // Act
        var result = Classes.From(typeof(Customer)).AsAllNonSystemInterfaces().ToServiceCollection();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void AsAllNonSystemInterfaces_WhenOnlySystemInterfaces_ShouldNotRegister()
    {
        // Act
        var result = Classes.From(typeof(Address)).AsAllNonSystemInterfaces().ToServiceCollection();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void AsDefaultNonSystemInterfaces_WhenNoMatch_ShouldNotRegister()
    {
        // Act
        var result = Classes.From(typeof(LegacyOrderProcessor)).AsDefaultNonSystemInterfaces().ToServiceCollection();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void AsInterface_WhenNoMatchingInterface_ShouldNotRegister()
    {
        // Act
        var result = Classes.From(typeof(Customer)).AsInterface().ToServiceCollection();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void AsInterface_WithExplicitType_WhenNoMatch_ShouldNotRegister()
    {
        // Act
        var result = Classes.From(typeof(CustomerService)).AsInterface(typeof(IPaymentGateway)).ToServiceCollection();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void AsInterfaces_WhenNoMatchingInterface_ShouldNotRegister()
    {
        // Act
        var result = Classes.From(typeof(Customer)).AsInterfaces(typeof(ICustomerService)).ToServiceCollection();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void AsInterfaces_WhenInterfaceTypesIsNull_ShouldThrow()
    {
        // Act
        var act = () => Classes.From(typeof(Customer)).AsInterfaces(null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void AsSelf_WhenChainedWithAsAllInterfaces_ShouldRegisterSelfThenDistinctInterfaces()
    {
        // Act
        var result = Classes.From(typeof(SqlCustomerRepository)).AsSelf().AsAllInterfaces().ToServiceCollection();

        // Assert
        var serviceTypes = result.Select(d => d.ServiceType).ToArray();
        Assert.Equal(typeof(SqlCustomerRepository), serviceTypes[0]);
        Assert.Equal(6, serviceTypes.Length);
        Assert.Equal(serviceTypes.Length, serviceTypes.Distinct().Count());
        Assert.Contains(typeof(ICustomerRepository), serviceTypes);
        Assert.Contains(typeof(IRepository<Customer>), serviceTypes);
        Assert.Contains(typeof(IReadOnlyRepository<Customer>), serviceTypes);
        Assert.Contains(typeof(IDisposable), serviceTypes);
        Assert.Contains(typeof(IAsyncDisposable), serviceTypes);
    }

    [Fact]
    public void AsInterface_WhenChainedWithAsAllInterfaces_ShouldDedupeTopLevelInterface()
    {
        // Act
        var result = Classes.From(typeof(SqlCustomerRepository)).AsInterface().AsAllInterfaces().ToServiceCollection();

        // Assert
        var serviceTypes = result.Select(d => d.ServiceType).ToArray();
        Assert.Equal(typeof(ICustomerRepository), serviceTypes[0]);
        Assert.Equal(5, serviceTypes.Length);
        Assert.Equal(serviceTypes.Length, serviceTypes.Distinct().Count());
        Assert.Contains(typeof(IRepository<Customer>), serviceTypes);
        Assert.Contains(typeof(IReadOnlyRepository<Customer>), serviceTypes);
        Assert.Contains(typeof(IDisposable), serviceTypes);
        Assert.Contains(typeof(IAsyncDisposable), serviceTypes);
    }

    [Fact]
    public void As_WhenChainedWithOverlappingServices_ShouldPreserveDistinctFirstOccurrenceOrder()
    {
        // Act
        var result = Classes
            .From(typeof(CustomerService))
            .As(_ => [typeof(IOrderService), typeof(ICustomerService)])
            .As(_ => [typeof(ICustomerService), typeof(IProductService)])
            .ToServiceCollection();

        // Assert
        var serviceTypes = result.Select(d => d.ServiceType).ToArray();
        Assert.Equal(
            [typeof(IOrderService), typeof(ICustomerService), typeof(IProductService)],
            serviceTypes
        );
    }

    [Fact]
    public void AsSelf_WhenChainedTwice_ShouldRegisterImplementationOnce()
    {
        // Act
        var result = Classes.From(typeof(CustomerService)).AsSelf().AsSelf().ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(CustomerService), descriptor.ServiceType);
        Assert.Equal(typeof(CustomerService), descriptor.ImplementationType);
    }

    [Fact]
    public void AsInterface_WhenChainedWithAsSelf_ShouldRegisterInterfaceThenSelf()
    {
        // Act
        var result = Classes.From(typeof(CustomerService)).AsInterface().AsSelf().ToServiceCollection();

        // Assert
        var serviceTypes = result.Select(d => d.ServiceType).ToArray();
        Assert.Equal([typeof(ICustomerService), typeof(CustomerService)], serviceTypes);
    }
}
