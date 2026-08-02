using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using ZCrew.Extensions.CodeAnalysis.CSharp.Testing;

namespace ZCrew.Extensions.DependencyInjection.Generator.Tests.TestHelpers;

internal static class GeneratorTest
{
    /// <summary>
    ///     The baseline for the registration generator's snapshot tests: net10 reference assemblies plus the runtime
    ///     assemblies the emitted code binds against (<c>Service</c>, <c>ServiceFilter</c>, and <c>ServiceLifetime</c>).
    ///     The <c>[Service]</c> attributes themselves are embedded by the generator. Compiler diagnostics are off; the
    ///     generated text is what is asserted. Locally a
    ///     mismatched or missing expected file is rewritten in place (the run still fails); review the diff, re-run to
    ///     go green. CI never writes.
    /// </summary>
    public static RoslynTestBuilder<DefaultVerifier> CreateBaseline<TGenerator>()
        where TGenerator : IIncrementalGenerator, new()
    {
        return IncrementalGeneratorTestBuilder
            .CreateDefaultBuilder<TGenerator>()
            .WithReferenceAssemblies(ReferenceAssemblies.Net.Net100)
            .WithAdditionalReferences(
                "ZCrew.Extensions.DependencyInjection.Registration.dll",
                "ZCrew.Extensions.DependencyInjection.dll",
                "Microsoft.Extensions.DependencyInjection.Abstractions.dll"
            )
            .WithCompilerDiagnostics(CompilerDiagnostics.None)
            .WithExpectedSourceUpdates(enabled: Environment.GetEnvironmentVariable("CI") is null);
    }
}
