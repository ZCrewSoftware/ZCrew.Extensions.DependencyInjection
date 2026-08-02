using ZCrew.Extensions.DependencyInjection.Generator.Tests.TestHelpers;

namespace ZCrew.Extensions.DependencyInjection.Generator.Tests;

public sealed class ServiceRegistrationAnalyzerTests
{
    [Fact]
    public async Task Analyze_WhenKeyedKeyIsArray_ShouldReportKeyCannotBeArray()
    {
        // Arrange
        const string source = """
            using ZCrew.Extensions.DependencyInjection.Registration;

            namespace Sample;

            [Service, Keyed(new int[] { 1, 2 })]
            public class ArrayKeyedService;
            """;
        var run = GeneratorHarness.Run(source);

        // Act
        var diagnostics = await run.AnalyzerDiagnosticsAsync();

        // Assert
        var diagnostic = Assert.Single(diagnostics);
        Assert.Equal("ZCDI001", diagnostic.Id);
        Assert.Contains("Keyed", diagnostic.GetMessage());
    }

    [Fact]
    public async Task Analyze_WhenAsKeyIsArray_ShouldReportKeyCannotBeArray()
    {
        // Arrange
        const string source = """
            using ZCrew.Extensions.DependencyInjection.Registration;

            namespace Sample;

            public interface IService;

            [Service, As<IService>(new int[] { 1, 2 })]
            public class ArrayKeyedService : IService;
            """;
        var run = GeneratorHarness.Run(source);

        // Act
        var diagnostics = await run.AnalyzerDiagnosticsAsync();

        // Assert
        var diagnostic = Assert.Single(diagnostics);
        Assert.Equal("ZCDI001", diagnostic.Id);
        Assert.Contains("As", diagnostic.GetMessage());
    }

    [Fact]
    public async Task Analyze_WhenNonGenericAsKeyIsArray_ShouldReportKeyCannotBeArray()
    {
        // Arrange
        const string source = """
            using ZCrew.Extensions.DependencyInjection.Registration;

            namespace Sample;

            public interface IService;

            [Service, As(typeof(IService), new int[] { 1, 2 })]
            public class ArrayKeyedService : IService;
            """;
        var run = GeneratorHarness.Run(source);

        // Act
        var diagnostics = await run.AnalyzerDiagnosticsAsync();

        // Assert
        var diagnostic = Assert.Single(diagnostics);
        Assert.Equal("ZCDI001", diagnostic.Id);
        Assert.Contains("As", diagnostic.GetMessage());
    }

    [Fact]
    public async Task Analyze_WhenKeyIsString_ShouldReportNothing()
    {
        // Arrange
        const string source = """
            using ZCrew.Extensions.DependencyInjection.Registration;

            namespace Sample;

            public interface IService;

            [Service, Keyed("primary")]
            [As<IService>("secondary")]
            public class KeyedService : IService;
            """;
        var run = GeneratorHarness.Run(source);

        // Act
        var diagnostics = await run.AnalyzerDiagnosticsAsync();

        // Assert
        Assert.Empty(diagnostics);
    }

    [Fact]
    public async Task Analyze_WhenModifierHasNoService_ShouldReportModifierRequiresService()
    {
        // Arrange
        const string source = """
            using ZCrew.Extensions.DependencyInjection.Registration;

            namespace Sample;

            [Scoped]
            public class OrphanModifier;
            """;
        var run = GeneratorHarness.Run(source);

        // Act
        var diagnostics = await run.AnalyzerDiagnosticsAsync();

        // Assert
        var diagnostic = Assert.Single(diagnostics);
        Assert.Equal("ZCDI002", diagnostic.Id);
        Assert.Contains("Scoped", diagnostic.GetMessage());
    }

    [Fact]
    public async Task Analyze_WhenAsHasNoService_ShouldReportModifierRequiresService()
    {
        // Arrange — the type implements IService, so only the orphan-modifier rule can fire.
        const string source = """
            using ZCrew.Extensions.DependencyInjection.Registration;

            namespace Sample;

            public interface IService;

            [As<IService>]
            public class OrphanAs : IService;
            """;
        var run = GeneratorHarness.Run(source);

        // Act
        var diagnostics = await run.AnalyzerDiagnosticsAsync();

        // Assert
        var diagnostic = Assert.Single(diagnostics);
        Assert.Equal("ZCDI002", diagnostic.Id);
        Assert.Contains("As", diagnostic.GetMessage());
    }

    [Fact]
    public async Task Analyze_WhenKeyedHasNoService_ShouldReportModifierRequiresService()
    {
        // Arrange
        const string source = """
            using ZCrew.Extensions.DependencyInjection.Registration;

            namespace Sample;

            [Keyed("primary")]
            public class OrphanKeyed;
            """;
        var run = GeneratorHarness.Run(source);

        // Act
        var diagnostics = await run.AnalyzerDiagnosticsAsync();

        // Assert
        var diagnostic = Assert.Single(diagnostics);
        Assert.Equal("ZCDI002", diagnostic.Id);
        Assert.Contains("Keyed", diagnostic.GetMessage());
    }

    [Fact]
    public async Task Analyze_WhenServicePresent_ShouldNotReportModifierRequiresService()
    {
        // Arrange
        const string source = """
            using ZCrew.Extensions.DependencyInjection.Registration;

            namespace Sample;

            public interface IService;

            [Service, Scoped]
            [As<IService>]
            public class ProperService : IService;
            """;
        var run = GeneratorHarness.Run(source);

        // Act
        var diagnostics = await run.AnalyzerDiagnosticsAsync();

        // Assert
        Assert.Empty(diagnostics);
    }

    [Fact]
    public async Task Analyze_WhenImplementationNotAssignableToServiceType_ShouldReportNotAssignable()
    {
        // Arrange
        const string source = """
            using ZCrew.Extensions.DependencyInjection.Registration;

            namespace Sample;

            public interface IUnrelated;

            [Service, As<IUnrelated>]
            public class NotAnIUnrelated;
            """;
        var run = GeneratorHarness.Run(source);

        // Act
        var diagnostics = await run.AnalyzerDiagnosticsAsync();

        // Assert
        var diagnostic = Assert.Single(diagnostics);
        Assert.Equal("ZCDI003", diagnostic.Id);
        Assert.Contains("IUnrelated", diagnostic.GetMessage());
    }

    [Fact]
    public async Task Analyze_WhenImplementationAssignableToServiceType_ShouldReportNothing()
    {
        // Arrange
        const string source = """
            using ZCrew.Extensions.DependencyInjection.Registration;

            namespace Sample;

            public interface IService;

            [Service, As<IService>]
            public class ProperService : IService;
            """;
        var run = GeneratorHarness.Run(source);

        // Act
        var diagnostics = await run.AnalyzerDiagnosticsAsync();

        // Assert
        Assert.Empty(diagnostics);
    }

    [Fact]
    public async Task Analyze_WhenImplicitOperatorToServiceType_ShouldReportNotAssignable()
    {
        // Arrange — a user-defined implicit conversion is not one the container can perform; it casts at resolution.
        const string source = """
            using ZCrew.Extensions.DependencyInjection.Registration;

            namespace Sample;

            public class Money;

            [Service, As<Money>]
            public class Cash
            {
                public static implicit operator Money(Cash cash) => new Money();
            }
            """;
        var run = GeneratorHarness.Run(source);

        // Act
        var diagnostics = await run.AnalyzerDiagnosticsAsync();

        // Assert
        var diagnostic = Assert.Single(diagnostics);
        Assert.Equal("ZCDI003", diagnostic.Id);
        Assert.Contains("Money", diagnostic.GetMessage());
    }

    [Fact]
    public async Task Analyze_WhenServiceTypeIsBaseClass_ShouldReportNothing()
    {
        // Arrange
        const string source = """
            using ZCrew.Extensions.DependencyInjection.Registration;

            namespace Sample;

            public class BaseThing;

            [Service, As<BaseThing>]
            public class Thing : BaseThing;
            """;
        var run = GeneratorHarness.Run(source);

        // Act
        var diagnostics = await run.AnalyzerDiagnosticsAsync();

        // Assert
        Assert.Empty(diagnostics);
    }

    [Fact]
    public async Task Analyze_WhenStructImplementsServiceInterface_ShouldReportNothing()
    {
        // Arrange — a struct reaches its interface by boxing, which the container performs.
        const string source = """
            using ZCrew.Extensions.DependencyInjection.Registration;

            namespace Sample;

            public interface IService;

            [Service, As<IService>]
            public struct StructService : IService;
            """;
        var run = GeneratorHarness.Run(source);

        // Act
        var diagnostics = await run.AnalyzerDiagnosticsAsync();

        // Assert
        Assert.Empty(diagnostics);
    }

    [Fact]
    public async Task Analyze_WhenNonGenericAsTypeNotAssignable_ShouldReportNotAssignable()
    {
        // Arrange
        const string source = """
            using ZCrew.Extensions.DependencyInjection.Registration;

            namespace Sample;

            public interface IUnrelated;

            [Service, As(typeof(IUnrelated))]
            public class NotAnIUnrelated;
            """;
        var run = GeneratorHarness.Run(source);

        // Act
        var diagnostics = await run.AnalyzerDiagnosticsAsync();

        // Assert
        var diagnostic = Assert.Single(diagnostics);
        Assert.Equal("ZCDI003", diagnostic.Id);
        Assert.Contains("IUnrelated", diagnostic.GetMessage());
    }

    [Fact]
    public async Task Analyze_WhenNonGenericAsTypeAssignable_ShouldReportNothing()
    {
        // Arrange
        const string source = """
            using ZCrew.Extensions.DependencyInjection.Registration;

            namespace Sample;

            public interface IService;

            [Service, As(typeof(IService))]
            public class ProperService : IService;
            """;
        var run = GeneratorHarness.Run(source);

        // Act
        var diagnostics = await run.AnalyzerDiagnosticsAsync();

        // Assert
        Assert.Empty(diagnostics);
    }

    [Fact]
    public async Task Analyze_WhenOpenGenericAs_ShouldReportNothing()
    {
        // Arrange — assignability is not checked for generic implementations.
        const string source = """
            using ZCrew.Extensions.DependencyInjection.Registration;

            namespace Sample;

            public interface IRepository<T>;

            [Service, As(typeof(IRepository<>))]
            public class Repository<T> : IRepository<T>;
            """;
        var run = GeneratorHarness.Run(source);

        // Act
        var diagnostics = await run.AnalyzerDiagnosticsAsync();

        // Assert
        Assert.Empty(diagnostics);
    }

    [Fact]
    public async Task Analyze_WhenConflictingLifetimes_ShouldReportConflictingLifetimes()
    {
        // Arrange
        const string source = """
            using ZCrew.Extensions.DependencyInjection.Registration;

            namespace Sample;

            [Service, Singleton, Scoped]
            public class TwoLifetimes;
            """;
        var run = GeneratorHarness.Run(source);

        // Act
        var diagnostics = await run.AnalyzerDiagnosticsAsync();

        // Assert
        var diagnostic = Assert.Single(diagnostics);
        Assert.Equal("ZCDI004", diagnostic.Id);
    }

    [Fact]
    public async Task Analyze_WhenSingleLifetime_ShouldReportNothing()
    {
        // Arrange
        const string source = """
            using ZCrew.Extensions.DependencyInjection.Registration;

            namespace Sample;

            [Service, Transient]
            public class OneLifetime;
            """;
        var run = GeneratorHarness.Run(source);

        // Act
        var diagnostics = await run.AnalyzerDiagnosticsAsync();

        // Assert
        Assert.Empty(diagnostics);
    }
}
