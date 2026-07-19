using Microsoft.CodeAnalysis.Testing;
using ZCrew.Extensions.CodeAnalysis.CSharp.Testing;

namespace ZCrew.Extensions.DependencyInjection.Generator.Tests.TestHelpers;

internal static class GeneratorTest
{
    /// <summary>
    ///     The .NET 10 reference assemblies, resolved manually until a pre-fab
    ///     <see cref="ReferenceAssemblies.Net"/> entry exists for net10.0.
    /// </summary>
    public static readonly ReferenceAssemblies Net100 = new(
        "net10.0",
        new PackageIdentity("Microsoft.NETCore.App.Ref", "10.0.0"),
        Path.Combine("ref", "net10.0")
    );

    /// <summary>
    ///     The baseline for the registration generator's snapshot tests: net10 reference assemblies plus the runtime
    ///     assemblies the emitted code and the test sources bind against (<c>Service</c>, <c>ServiceAttribute</c>, and
    ///     <c>ServiceLifetime</c>). Compiler diagnostics are off; the generated text is what is asserted. Locally a
    ///     mismatched or missing expected file is rewritten in place (the run still fails); review the diff, re-run to
    ///     go green. CI never writes.
    /// </summary>
    public static SourceGeneratorTestBuilder<TGenerator, DefaultVerifier> CreateBaseline<TGenerator>()
        where TGenerator : new()
    {
        return SourceGeneratorTestBuilder<TGenerator>
            .CreateDefaultBuilder()
            .WithReferenceAssemblies(Net100)
            .WithAdditionalReferences(
                "ZCrew.Extensions.DependencyInjection.Registration.dll",
                "ZCrew.Extensions.DependencyInjection.dll",
                "Microsoft.Extensions.DependencyInjection.Abstractions.dll"
            )
            .WithCompilerDiagnostics(CompilerDiagnostics.None)
            .WithExpectedSourceUpdates(enabled: Environment.GetEnvironmentVariable("CI") is null);
    }
}
