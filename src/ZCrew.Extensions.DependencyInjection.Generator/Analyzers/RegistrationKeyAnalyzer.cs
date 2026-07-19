using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ZCrew.Extensions.DependencyInjection.Generator.Analyzers;

/// <summary>
///     Reports an array used as the <c>Key</c> on a registration attribute (<c>[Service]</c>). Keys resolve by their
///     type's default equality, and arrays compare by reference, so a fresh array instance never matches a lookup. The
///     generator emits the key faithfully regardless (generators never diagnose); this analyzer owns the report.
///     Registration rules are <c>ZCDI0##</c>.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal sealed class RegistrationKeyAnalyzer : DiagnosticAnalyzer
{
    private const string Category = "ZCrew.Extensions.DependencyInjection.Registration";

    public static readonly DiagnosticDescriptor KeyCannotBeArray = new(
        "ZCDI001",
        "Registration key cannot be an array",
        "'{0}' uses an array as its [{1}] key; arrays compare by reference, so keyed resolution never matches. Use a value-equatable key such as a ValueTuple, string, enum, or primitive.",
        Category,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    // Each registration attribute's metadata name paired with the short name shown in the message.
    private static readonly (string MetadataName, string ShortName)[] RegistrationAttributes =
    [
        ("ZCrew.Extensions.DependencyInjection.Registration.ServiceAttribute", "Service"),
    ];

    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [KeyCannotBeArray];

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSymbolAction(static symbolContext => Analyze(symbolContext), SymbolKind.NamedType);
    }

    private static void Analyze(SymbolAnalysisContext context)
    {
        var type = (INamedTypeSymbol)context.Symbol;
        foreach (var attribute in type.GetAttributes())
        {
            var name = attribute.AttributeClass?.ToDisplayString();
            var match = RegistrationAttributes.FirstOrDefault(candidate => candidate.MetadataName == name);
            if (match.MetadataName is null || !HasArrayKey(attribute))
            {
                continue;
            }

            var location =
                attribute.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken).GetLocation()
                ?? type.Locations[0];
            context.ReportDiagnostic(Diagnostic.Create(KeyCannotBeArray, location, type.Name, match.ShortName));
        }
    }

    // The key is the `Key` init property, so it only ever appears as a named argument. The constructor's
    // `params Type[] serviceTypes` list is a legitimate positional array and must not be mistaken for the key.
    private static bool HasArrayKey(AttributeData attribute)
    {
        return attribute.NamedArguments.Any(argument =>
            argument.Key == "Key" && argument.Value.Kind == TypedConstantKind.Array
        );
    }
}
