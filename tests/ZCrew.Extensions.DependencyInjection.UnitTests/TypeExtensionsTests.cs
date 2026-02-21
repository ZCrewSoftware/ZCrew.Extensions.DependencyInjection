namespace ZCrew.Extensions.DependencyInjection.UnitTests;

public class TypeExtensionsTests
{
    [Fact]
    public void HasAttribute_WhenTypeHasAttribute_ShouldReturnTrue()
    {
        // Act
        var result = typeof(DecoratedType).HasAttribute<TestAttribute>();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasAttribute_WhenTypeDoesNotHaveAttribute_ShouldReturnFalse()
    {
        // Act
        var result = typeof(PlainType).HasAttribute<TestAttribute>();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HasAttribute_WithFilter_WhenAttributeMatchesFilter_ShouldReturnTrue()
    {
        // Act
        var result = typeof(DecoratedType).HasAttribute<TestAttribute>(a => a.Value == "hello");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasAttribute_WithFilter_WhenAttributeDoesNotMatchFilter_ShouldReturnFalse()
    {
        // Act
        var result = typeof(DecoratedType).HasAttribute<TestAttribute>(a => a.Value == "world");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HasAttribute_WithFilter_WhenTypeDoesNotHaveAttribute_ShouldReturnFalse()
    {
        // Act
        var result = typeof(PlainType).HasAttribute<TestAttribute>(a => a.Value == "hello");

        // Assert
        Assert.False(result);
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
        var result = typeof(IMyService).GetInterfaceName();

        // Assert
        Assert.Equal("MyService", result);
    }

    [Fact]
    public void GetInterfaceName_WhenNoIPrefix_ShouldReturnUnchanged()
    {
        // Act
        var result = typeof(PlainType).GetInterfaceName();

        // Assert
        Assert.Equal("PlainType", result);
    }

    [Fact]
    public void GetInterfaceName_WhenSecondCharIsLowercase_ShouldReturnUnchanged()
    {
        // Act
        var result = typeof(Items).GetInterfaceName();

        // Assert
        Assert.Equal("Items", result);
    }

    [Fact]
    public void GetTopLevelInterfaces_WhenHierarchy_ShouldReturnOnlyMostDerived()
    {
        // Act
        var result = typeof(DerivedImpl).GetTopLevelInterfaces().ToList();

        // Assert
        Assert.Single(result);
        Assert.Contains(typeof(IDerived), result);
    }

    [Fact]
    public void GetTopLevelInterfaces_WhenMultipleUnrelated_ShouldReturnAll()
    {
        // Act
        var result = typeof(MultiImpl).GetTopLevelInterfaces().ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(typeof(IFirst), result);
        Assert.Contains(typeof(ISecond), result);
    }

    [Fact]
    public void GetTopLevelInterfaces_WhenNoInterfaces_ShouldReturnEmpty()
    {
        // Act
        var result = typeof(PlainType).GetTopLevelInterfaces();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetTopLevelInterfaces_WhenDiamondHierarchy_ShouldReturnOnlyLeaves()
    {
        // Act
        var result = typeof(DiamondImpl).GetTopLevelInterfaces().ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(typeof(IDiamondLeft), result);
        Assert.Contains(typeof(IDiamondRight), result);
        Assert.DoesNotContain(typeof(IDiamondBase), result);
    }

    [AttributeUsage(AttributeTargets.Class)]
    private class TestAttribute(string value) : Attribute
    {
        public string Value => value;
    }

    [Test("hello")]
    private class DecoratedType;

    private class PlainType;

    private class Items;

    private interface IMyService;

    private interface IBase;

    private interface IDerived : IBase;

    private class DerivedImpl : IDerived;

    private interface IFirst;

    private interface ISecond;

    private class MultiImpl : IFirst, ISecond;

    private interface IDiamondBase;

    private interface IDiamondLeft : IDiamondBase;

    private interface IDiamondRight : IDiamondBase;

    private class DiamondImpl : IDiamondLeft, IDiamondRight;
}
