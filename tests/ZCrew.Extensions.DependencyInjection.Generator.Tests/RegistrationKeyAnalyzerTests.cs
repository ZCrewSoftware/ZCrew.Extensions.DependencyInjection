using ZCrew.Extensions.DependencyInjection.Generator.Tests.TestHelpers;

namespace ZCrew.Extensions.DependencyInjection.Generator.Tests;

public sealed class RegistrationKeyAnalyzerTests
{
    [Fact]
    public async Task Analyze_WhenKeyIsArray_ShouldReportKeyCannotBeArray()
    {
        // Arrange
        const string source = """
            using ZCrew.Extensions.DependencyInjection.Registration;

            namespace Sample;

            public interface IService;

            [Service(typeof(IService), Key = new int[] { 1, 2 })]
            public class ArrayKeyedService : IService;
            """;
        var run = GeneratorHarness.Run(source);

        // Act
        var diagnostics = await run.AnalyzerDiagnosticsAsync();

        // Assert
        var diagnostic = Assert.Single(diagnostics);
        Assert.Equal("ZCDI001", diagnostic.Id);
        Assert.Contains("Service", diagnostic.GetMessage());
    }

    [Fact]
    public async Task Analyze_WhenMultipleServiceTypes_ShouldReportNothing()
    {
        // Arrange
        const string source = """
            using ZCrew.Extensions.DependencyInjection.Registration;

            namespace Sample;

            public interface IFoo;

            public interface IBar;

            [Service(typeof(IFoo), typeof(IBar))]
            public class MultiService : IFoo, IBar;
            """;
        var run = GeneratorHarness.Run(source);

        // Act
        var diagnostics = await run.AnalyzerDiagnosticsAsync();

        // Assert
        Assert.Empty(diagnostics);
    }

    [Fact]
    public async Task Analyze_WhenKeyIsString_ShouldReportNothing()
    {
        // Arrange
        const string source = """
            using ZCrew.Extensions.DependencyInjection.Registration;

            namespace Sample;

            public interface IService;

            [Service(typeof(IService), Key = "primary")]
            public class KeyedService : IService;
            """;
        var run = GeneratorHarness.Run(source);

        // Act
        var diagnostics = await run.AnalyzerDiagnosticsAsync();

        // Assert
        Assert.Empty(diagnostics);
    }

    [Fact]
    public async Task Analyze_WhenServiceHasNoKey_ShouldReportNothing()
    {
        // Arrange
        const string source = """
            using ZCrew.Extensions.DependencyInjection.Registration;

            namespace Sample;

            public interface IService;

            [Service(typeof(IService))]
            public class UnkeyedService : IService;
            """;
        var run = GeneratorHarness.Run(source);

        // Act
        var diagnostics = await run.AnalyzerDiagnosticsAsync();

        // Assert
        Assert.Empty(diagnostics);
    }
}
