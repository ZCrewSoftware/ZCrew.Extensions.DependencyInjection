using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using ZCrew.Extensions.DependencyInjection.Generator.Analyzers;
using ZCrew.Extensions.DependencyInjection.Registration;

namespace ZCrew.Extensions.DependencyInjection.Generator.Tests.TestHelpers;

/// <summary>
///     One driver pass over an input compilation: the driver (carrying incremental state for reruns), the run result
///     with tracked steps, and the output compilation including the generated trees.
/// </summary>
internal sealed record HarnessRun(
    GeneratorDriver Driver,
    CSharpCompilation InputCompilation,
    GeneratorDriverRunResult Result,
    Compilation OutputCompilation
);

/// <summary>
///     Drives <c>ServiceRegistrationSourceGenerator</c> over snippet compilations referencing the Registration and
///     MSDI assemblies, with incremental step tracking enabled so tests can assert on cached vs. re-run outputs.
/// </summary>
internal static class GeneratorHarness
{
    private static readonly CSharpParseOptions ParseOptions = new(LanguageVersion.CSharp14);

    private static readonly ImmutableArray<MetadataReference> References = BuildReferences();

    public static HarnessRun Run(params string[] sources)
    {
        var compilation = CSharpCompilation.Create(
            "GeneratorTests",
            sources.Select(source => CSharpSyntaxTree.ParseText(source, ParseOptions)),
            References,
            new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary,
                nullableContextOptions: NullableContextOptions.Enable
            )
        );

        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            [new ServiceRegistrationSourceGenerator().AsSourceGenerator()],
            parseOptions: ParseOptions,
            driverOptions: new GeneratorDriverOptions(
                IncrementalGeneratorOutputKind.None,
                trackIncrementalGeneratorSteps: true
            )
        );

        return RunDriver(driver, compilation);
    }

    /// <summary>Reruns the driver with one input source replaced, preserving incremental state.</summary>
    public static HarnessRun Update(this HarnessRun run, int sourceIndex, string updatedSource)
    {
        var trees = run.InputCompilation.SyntaxTrees;
        var compilation = run.InputCompilation.ReplaceSyntaxTree(
            trees[sourceIndex],
            CSharpSyntaxTree.ParseText(updatedSource, ParseOptions)
        );
        return RunDriver(run.Driver, compilation);
    }

    /// <summary>The run result of one generator in the driver.</summary>
    public static GeneratorRunResult ResultOf<TGenerator>(this HarnessRun run)
        where TGenerator : IIncrementalGenerator
    {
        return run.Result.Results.Single(result => result.Generator.GetGeneratorType() == typeof(TGenerator));
    }

    /// <summary>The text of the generated source with the given hint name, searched across all generators.</summary>
    public static string GeneratedSource(this HarnessRun run, string hintName)
    {
        return run
            .Result.Results.SelectMany(result => result.GeneratedSources)
            .Single(source => source.HintName == hintName)
            .SourceText.ToString();
    }

    /// <summary>Asserts the output compilation (all generators' parts combined) has no errors.</summary>
    public static void AssertNoErrors(this HarnessRun run)
    {
        var errors = run
            .OutputCompilation.GetDiagnostics()
            .Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
            .ToList();
        Assert.Empty(errors);
    }

    /// <summary>
    ///     Runs the companion analyzer over the output compilation (post-generation, so the embedded attribute
    ///     definitions resolve) and returns its diagnostics.
    /// </summary>
    public static async Task<ImmutableArray<Diagnostic>> AnalyzerDiagnosticsAsync(this HarnessRun run)
    {
        var compilation = run.OutputCompilation.WithAnalyzers([new ServiceRegistrationAnalyzer()]);
        return await compilation.GetAnalyzerDiagnosticsAsync(TestContext.Current.CancellationToken);
    }

    private static HarnessRun RunDriver(GeneratorDriver driver, CSharpCompilation compilation)
    {
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        return new HarnessRun(driver, compilation, driver.GetRunResult(), outputCompilation);
    }

    private static ImmutableArray<MetadataReference> BuildReferences()
    {
        var trusted = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!).Split(Path.PathSeparator);
        var paths = new HashSet<string>(trusted, StringComparer.OrdinalIgnoreCase)
        {
            typeof(Service).Assembly.Location,
            typeof(ServiceLifetime).Assembly.Location,
        };
        return [.. paths.Select(path => (MetadataReference)MetadataReference.CreateFromFile(path))];
    }
}
