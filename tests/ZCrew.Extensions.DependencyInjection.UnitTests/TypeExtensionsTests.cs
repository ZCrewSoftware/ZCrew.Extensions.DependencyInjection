using Fixtures.SmallProject.Application.Caching;
using Fixtures.SmallProject.Application.Ports;
using Fixtures.SmallProject.Application.Services;
using Fixtures.SmallProject.Domain.Auditing;
using Fixtures.SmallProject.Domain.Entities;
using Fixtures.SmallProject.Domain.Repositories;
using Fixtures.SmallProject.Domain.Services;
using Fixtures.SmallProject.Infrastructure.Notifications;
using Fixtures.SmallProject.Infrastructure.Persistence;

namespace ZCrew.Extensions.DependencyInjection.UnitTests;

public class TypeExtensionsTests
{
    [Fact]
    public void HasAttribute_WhenTypeHasAttribute_ShouldReturnTrue()
    {
        // Act
        var result = typeof(CachingCustomerService).HasAttribute<CacheableAttribute>();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasAttribute_WhenTypeDoesNotHaveAttribute_ShouldReturnFalse()
    {
        // Act
        var result = typeof(Customer).HasAttribute<CacheableAttribute>();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HasAttribute_WithFilter_WhenAttributeMatchesFilter_ShouldReturnTrue()
    {
        // Act
        var result = typeof(CachingCustomerService).HasAttribute<CacheableAttribute>(a => a.Region == "customers");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasAttribute_WithFilter_WhenAttributeDoesNotMatchFilter_ShouldReturnFalse()
    {
        // Act
        var result = typeof(CachingCustomerService).HasAttribute<CacheableAttribute>(a => a.Region == "products");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HasAttribute_WithFilter_WhenTypeDoesNotHaveAttribute_ShouldReturnFalse()
    {
        // Act
        var result = typeof(Customer).HasAttribute<CacheableAttribute>(a => a.Region == "customers");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsAbstractClass_WhenAbstractClass_ShouldReturnTrue()
    {
        // Act
        var result = typeof(RepositoryBase<Customer>).IsAbstractClass;

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsAbstractClass_WhenInterface_ShouldReturnFalse()
    {
        // Act
        var result = typeof(ICustomerRepository).IsAbstractClass;

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsAbstractClass_WhenConcreteClass_ShouldReturnFalse()
    {
        // Act
        var result = typeof(SqlCustomerRepository).IsAbstractClass;

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsAbstractClass_WhenStaticClass_ShouldReturnTrue()
    {
        // Act
        var result = typeof(PricingDefaults).IsAbstractClass;

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsInNamespace_WhenExactMatch_ShouldReturnTrue()
    {
        // Act
        var result = typeof(string).IsInNamespace("System");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsInNamespace_WhenNotMatch_ShouldReturnFalse()
    {
        // Act
        var result = typeof(string).IsInNamespace("System.IO");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsInNamespace_WhenSubNamespaceWithoutFlag_ShouldReturnFalse()
    {
        // Act
        var result = typeof(List<>).IsInNamespace("System");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsInNamespace_WhenSubNamespaceWithFlag_ShouldReturnTrue()
    {
        // Act
        var result = typeof(List<>).IsInNamespace("System", includeSubnamespaces: true);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsInNamespace_WhenExactMatchWithSubNamespaceFlag_ShouldReturnTrue()
    {
        // Act
        var result = typeof(string).IsInNamespace("System", includeSubnamespaces: true);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsInNamespace_WhenNull_ShouldMatchTypeWithNullNamespace()
    {
        // Act
        var result = typeof(string).IsInNamespace(null);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsInSameNamespaceAs_WhenSameNamespace_ShouldReturnTrue()
    {
        // Act
        var result = typeof(string).IsInSameNamespaceAs(typeof(int));

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsInSameNamespaceAs_WhenDifferentNamespace_ShouldReturnFalse()
    {
        // Act
        var result = typeof(string).IsInSameNamespaceAs(typeof(List<>));

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsInSameNamespaceAs_WithSubNamespaceFlag_WhenInSubNamespace_ShouldReturnTrue()
    {
        // Act
        var result = typeof(List<>).IsInSameNamespaceAs(typeof(string), includeSubnamespaces: true);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsInSameNamespaceAs_T_WhenSameNamespace_ShouldReturnTrue()
    {
        // Act
        var result = typeof(string).IsInSameNamespaceAs<int>();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsInSameNamespaceAs_T_WhenDifferentNamespace_ShouldReturnFalse()
    {
        // Act
        var result = typeof(string).IsInSameNamespaceAs<List<int>>();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsInSameNamespaceAs_T_WithSubNamespaceFlag_WhenInSubNamespace_ShouldReturnTrue()
    {
        // Act
        var result = typeof(List<>).IsInSameNamespaceAs<string>(includeSubnamespaces: true);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void GetInterfaceName_WhenHasConventionalIPrefix_ShouldStripPrefix()
    {
        // Act
        var result = typeof(ICustomerService).GetInterfaceName();

        // Assert
        Assert.Equal("CustomerService", result);
    }

    [Fact]
    public void GetInterfaceName_WhenNoIPrefix_ShouldReturnUnchanged()
    {
        // Act
        var result = typeof(Customer).GetInterfaceName();

        // Assert
        Assert.Equal("Customer", result);
    }

    [Fact]
    public void GetInterfaceName_WhenSecondCharIsLowercase_ShouldReturnUnchanged()
    {
        // Act
        var result = typeof(Invoice).GetInterfaceName();

        // Assert
        Assert.Equal("Invoice", result);
    }

    [Fact]
    public void GetTopLevelInterfaces_WhenHierarchy_ShouldReturnOnlyMostDerived()
    {
        // Act
        var result = typeof(SqlCustomerRepository).GetTopLevelInterfaces().ToList();

        // Assert
        Assert.Single(result);
        Assert.Contains(typeof(ICustomerRepository), result);
    }

    [Fact]
    public void GetTopLevelInterfaces_WhenMultipleUnrelated_ShouldReturnAll()
    {
        // Act
        var result = typeof(BroadcastNotificationSender).GetTopLevelInterfaces().ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(typeof(INotificationSender), result);
        Assert.Contains(typeof(IEventPublisher), result);
    }

    [Fact]
    public void GetTopLevelInterfaces_WhenNoInterfaces_ShouldReturnEmpty()
    {
        // Act
        var result = typeof(Customer).GetTopLevelInterfaces();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetTopLevelInterfaces_WhenDiamondHierarchy_ShouldReturnOnlyLeaves()
    {
        // Act
        var result = typeof(AuditableSoftDeletableEntity).GetTopLevelInterfaces().ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(typeof(IAuditable), result);
        Assert.Contains(typeof(ISoftDeletable), result);
        Assert.DoesNotContain(typeof(IIdentifiable), result);
    }

    [Fact]
    public void GetTypes_OnConcreteClassWithBaseAndInterface_ShouldReturnTypeItselfBaseClassesAndInterfaces()
    {
        // Act
        var result = typeof(SqlCustomerRepository).GetTypes().ToList();

        // Assert
        Assert.Contains(typeof(SqlCustomerRepository), result);
        Assert.Contains(typeof(RepositoryBase<Customer>), result);
        Assert.Contains(typeof(object), result);
        Assert.Contains(typeof(ICustomerRepository), result);
        Assert.Contains(typeof(IRepository<Customer>), result);
        Assert.Contains(typeof(IReadOnlyRepository<Customer>), result);
        Assert.Contains(typeof(IDisposable), result);
        Assert.Contains(typeof(IAsyncDisposable), result);
    }

    [Fact]
    public void GetTypes_WhenBaseClassIsAbstract_ShouldIncludeAbstractBaseClasses()
    {
        // Act
        var result = typeof(SqlCustomerRepository).GetTypes().ToList();

        // Assert
        Assert.Contains(typeof(RepositoryBase<Customer>), result);
    }

    [Fact]
    public void GetTypes_WhenCalled_ShouldIncludeObjectBaseType()
    {
        // Act
        var result = typeof(SqlCustomerRepository).GetTypes().ToList();

        // Assert
        Assert.Contains(typeof(object), result);
    }

    [Fact]
    public void GetTypes_OnTypeWithNoBaseOrInterfaces_ShouldReturnTypeItselfAndObject()
    {
        // Act
        var result = typeof(Customer).GetTypes().ToList();

        // Assert
        Assert.Equal(new[] { typeof(Customer), typeof(object) }, result);
    }

    [Fact]
    public void GetTypes_OnTypeWithMultipleBaseClasses_ShouldReturnFullChainInOrder()
    {
        // Act
        var result = typeof(SubscriptionProduct).GetTypes().ToList();

        // Assert
        Assert.Equal(
            new[] { typeof(SubscriptionProduct), typeof(DigitalProduct), typeof(Product), typeof(object) },
            result
        );
    }

    [Fact]
    public void GetTypes_OnInterface_ShouldReturnInterfaceItselfAndInheritedInterfaces()
    {
        // Act
        var result = typeof(ICustomerRepository).GetTypes().ToList();

        // Assert
        Assert.Contains(typeof(ICustomerRepository), result);
        Assert.Contains(typeof(IRepository<Customer>), result);
        Assert.Contains(typeof(IReadOnlyRepository<Customer>), result);
        Assert.Contains(typeof(IDisposable), result);
        Assert.Contains(typeof(IAsyncDisposable), result);
        Assert.DoesNotContain(typeof(object), result);
    }

}
