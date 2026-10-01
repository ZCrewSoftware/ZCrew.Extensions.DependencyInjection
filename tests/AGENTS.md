# Test Conventions

## Running Tests

Tests use **xUnit v3** with the Microsoft Testing Platform runner. This does **not**
support VSTest-style `--filter "FullyQualifiedName~Foo"` syntax. Use the xUnit Then v3
filter flags instead: `--filter-class`, `--filter-method`, `--filter-namespace`.
Wildcards (`*`) are supported at the start and/or end of each value.

Run all tests in a single project:
```bash
dotnet test --project tests/ZCrew.Extensions.DependencyInjection.IntegrationTests/ZCrew.Extensions.DependencyInjection.IntegrationTests.csproj
```

Run all tests in a single class (wildcard matches the namespace prefix):
```bash
dotnet test --project tests/ZCrew.Extensions.DependencyInjection.IntegrationTests/ZCrew.Extensions.DependencyInjection.IntegrationTests.csproj \
  --filter-class "*DecoratorServiceCollectionExtensionsTests"
```

Run a single test method (wildcard-match the method name):
```bash
dotnet test --project tests/ZCrew.Extensions.DependencyInjection.IntegrationTests/ZCrew.Extensions.DependencyInjection.IntegrationTests.csproj \
  --filter-method "*WhenCalledTwice*"
```

Run several tests matching a pattern (e.g. all keyed-service tests across classes):
```bash
dotnet test --project tests/ZCrew.Extensions.DependencyInjection.IntegrationTests/ZCrew.Extensions.DependencyInjection.IntegrationTests.csproj \
  --filter-method "*Keyed*"
```

## Test Naming

Follow `Member_T_When_Should` style. The name reads as: what member is being tested,
what condition triggers it, and what the expected outcome is.

```csharp
Add_T_WhenEntryIsValid_ShouldAddEntry()
InvokeAsync_WhenCalled_ShouldYield()
GetOrder_WhenIdNotFound_ShouldReturnNull()
```

## AAA Structure

Every test must have `// Arrange`, `// Act`, and `// Assert` comments separating the three phases.
The only exception is when the 'Arrange' section is empty. This is very rare though.

```csharp
[Fact]
public void GetOrder_WhenIdIsValid_ShouldReturnOrder()
{
    // Arrange
    var service = new OrderService();

    // Act
    var result = service.GetOrder(42);

    // Assert
    Assert.NotNull(result);
}
```

**Never combine Act and Assert or Arrange & Act.**
If the action itself is the assertion (like testing that something throws), capture the call in a `Func` or `Action` first:

```csharp
// Bad — don't do this:
// Arrange & Act
var act = () => service.GetOrder(-1);

// Good — the arrange isn't necessary:
// Act
var act = () => service.GetOrder(-1);
```

```csharp
// Bad — don't do this:
// Act & Assert
Assert.Throws<ArgumentException>(() => service.GetOrder(-1));

// Good — separate the phases:
// Act
var act = () => service.GetOrder(-1);

// Assert
Assert.Throws<ArgumentException>(act);
```

## No Regions or Decorative Comments

Never use `#region` / `#endregion`. Never use decorative comments to separate groups of
tests (e.g., `// -- Non-keyed source descriptors --`). If a test class is large enough
to need visual separation, split it into partial classes or separate classes instead.

## Test Isolation

Each test must stand alone. Never share mutable state between tests via fields or
static members. If two tests share setup, each should create its own instance.
This prevents cascading failures where one broken test poisons the rest.

## Naming Variables

Never call anything `sut` (system under test). Use a name that describes what the
thing actually is — `service`, `factory`, `generator`, `provider`, etc.

## Test Doubles

**Use NSubstitute** when you need to verify interactions: was a method called, how
many times, with what arguments. That's what mocking libraries are for.

```csharp
var logger = Substitute.For<ILogger>();
service.Process();
logger.Received(1).Log(Arg.Any<string>());
```

**Don't create test doubles when a real type works.** If you need something that
implements `IList<T>`, just use `List<T>`. If you need an `IServiceProvider`, consider
building a real one with `ServiceCollection`. Only mock when you need to control or
observe behavior that a real type can't give you.
