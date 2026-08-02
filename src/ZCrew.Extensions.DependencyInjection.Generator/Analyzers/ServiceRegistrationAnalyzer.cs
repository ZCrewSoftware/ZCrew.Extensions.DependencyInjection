using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ZCrew.Extensions.DependencyInjection.Generator.Analyzers;

/// <summary>
///     Reports misuse of the <c>[Service]</c> attribute family. Generators never diagnose, so this analyzer owns every
///     report. Registration rules are <c>ZCDI0##</c>:
///     <list type="table">
///         <listheader>
///             <item>
///                 <term>Code</term>
///                 <description>Description</description>
///             </item>
///         </listheader>
///         <item>
///             <term><c>ZCDI001</c></term>
///             <description>An array used as a <c>[Keyed]</c> or <c>[As]</c> key.</description>
///         </item>
///         <item>
///             <term><c>ZCDI002</c></term>
///             <description>A modifier attribute on a type with no <c>[Service]</c>.</description>
///         </item>
///         <item>
///             <term><c>ZCDI003</c></term>
///             <description>An <c>[As]</c> service type the implementation is not assignable to.</description>
///         </item>
///         <item>
///             <term><c>ZCDI004</c></term>
///             <description>More than one lifetime attribute on a type.</description>
///         </item>
///     </list>
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal sealed class ServiceRegistrationAnalyzer : DiagnosticAnalyzer
{
    private const string Category = "ZCrew.Extensions.DependencyInjection.Registration";
    private const string AttributeNamespace = "ZCrew.Extensions.DependencyInjection.Registration";

    public static readonly DiagnosticDescriptor KeyCannotBeArray = new(
        "ZCDI001",
        "Registration key cannot be an array",
        "'{0}' uses an array as its [{1}] key; arrays compare by reference, so keyed resolution never matches. Use a value-equatable key such as a ValueTuple, string, enum, or primitive.",
        Category,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    public static readonly DiagnosticDescriptor ModifierRequiresService = new(
        "ZCDI002",
        "Registration modifier requires [Service]",
        "'{0}' uses [{1}] but has no [Service] attribute, so the modifier is ignored",
        Category,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    public static readonly DiagnosticDescriptor ServiceTypeNotAssignable = new(
        "ZCDI003",
        "Service type is not assignable from the implementation",
        "'{0}' is registered as '{1}' with [As] but is not assignable to '{1}'",
        Category,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    public static readonly DiagnosticDescriptor ConflictingLifetimes = new(
        "ZCDI004",
        "Conflicting lifetime attributes",
        "'{0}' has more than one lifetime attribute; use at most one of [Singleton], [Scoped], or [Transient]",
        Category,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        [KeyCannotBeArray, ModifierRequiresService, ServiceTypeNotAssignable, ConflictingLifetimes];

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
        var attributes = type.GetAttributes();

        var hasService = false;
        var lifetimeCount = 0;
        foreach (var attribute in attributes)
        {
            if (!IsRegistrationAttribute(attribute.AttributeClass))
            {
                continue;
            }

            switch (attribute.AttributeClass!.Name)
            {
                case "ServiceAttribute":
                    hasService = true;
                    break;
                case "SingletonAttribute":
                case "ScopedAttribute":
                case "TransientAttribute":
                    lifetimeCount++;
                    break;
            }
        }

        foreach (var attribute in attributes)
        {
            if (!IsRegistrationAttribute(attribute.AttributeClass))
            {
                continue;
            }

            switch (attribute.AttributeClass!.Name)
            {
                case "SingletonAttribute":
                case "ScopedAttribute":
                case "TransientAttribute":
                    ReportOrphanModifier(context, type, attribute, hasService);
                    break;
                case "KeyedAttribute":
                    ReportOrphanModifier(context, type, attribute, hasService);
                    ReportArrayKey(context, type, attribute, attribute.ConstructorArguments, index: 0, "Keyed");
                    break;
                case "AsAttribute":
                    ReportOrphanModifier(context, type, attribute, hasService);
                    AnalyzeAs(context, type, attribute);
                    break;
            }
        }

        if (lifetimeCount > 1)
        {
            context.ReportDiagnostic(Diagnostic.Create(ConflictingLifetimes, TypeLocation(type), type.Name));
        }
    }

    private static void ReportOrphanModifier(
        SymbolAnalysisContext context,
        INamedTypeSymbol type,
        AttributeData attribute,
        bool hasService
    )
    {
        if (hasService)
        {
            return;
        }

        context.ReportDiagnostic(
            Diagnostic.Create(
                ModifierRequiresService,
                AttributeLocation(context, attribute, type),
                type.Name,
                ShortName(attribute)
            )
        );
    }

    private static void AnalyzeAs(SymbolAnalysisContext context, INamedTypeSymbol type, AttributeData attribute)
    {
        var attributeClass = attribute.AttributeClass!;

        ITypeSymbol? serviceType;
        if (attributeClass.IsGenericType)
        {
            serviceType = attributeClass.TypeArguments[0];
            ReportArrayKey(context, type, attribute, attribute.ConstructorArguments, index: 0, "As");
        }
        else
        {
            serviceType =
                attribute.ConstructorArguments.Length > 0
                    ? attribute.ConstructorArguments[0].Value as ITypeSymbol
                    : null;
            ReportArrayKey(context, type, attribute, attribute.ConstructorArguments, index: 1, "As");
        }

        if (serviceType is null || !ShouldCheckAssignability(type, serviceType))
        {
            return;
        }

        // Only the conversions the container can actually perform: identity, implicit reference (class to base or
        // interface), and boxing (struct to interface). A user-defined implicit operator is implicit but casts at
        // resolution, so it must not pass. IsReference alone also covers explicit downcasts, hence the pairing.
        var conversion = ((CSharpCompilation)context.Compilation).ClassifyConversion(type, serviceType);
        if (conversion is not { IsIdentity: true } and not { IsImplicit: true, IsReference: true } and not { IsBoxing: true })
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    ServiceTypeNotAssignable,
                    AttributeLocation(context, attribute, type),
                    type.Name,
                    serviceType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)
                )
            );
        }
    }

    private static void ReportArrayKey(
        SymbolAnalysisContext context,
        INamedTypeSymbol type,
        AttributeData attribute,
        ImmutableArray<TypedConstant> arguments,
        int index,
        string shortName
    )
    {
        if (index < arguments.Length && arguments[index].Kind == TypedConstantKind.Array)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(KeyCannotBeArray, AttributeLocation(context, attribute, type), type.Name, shortName)
            );
        }
    }

    private static bool ShouldCheckAssignability(INamedTypeSymbol implementation, ITypeSymbol serviceType)
    {
        if (implementation.IsGenericType || implementation.IsUnboundGenericType)
        {
            return false;
        }

        return serviceType.TypeKind != TypeKind.TypeParameter
            && serviceType is not INamedTypeSymbol { IsUnboundGenericType: true };
    }

    private static bool IsRegistrationAttribute(INamedTypeSymbol? attributeClass)
    {
        return attributeClass?.ContainingNamespace?.ToDisplayString() == AttributeNamespace;
    }

    private static string ShortName(AttributeData attribute)
    {
        var name = attribute.AttributeClass!.Name;
        return name.EndsWith("Attribute", StringComparison.Ordinal)
            ? name.Substring(0, name.Length - "Attribute".Length)
            : name;
    }

    private static Location AttributeLocation(SymbolAnalysisContext context, AttributeData attribute, INamedTypeSymbol type)
    {
        return attribute.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken).GetLocation()
            ?? TypeLocation(type);
    }

    private static Location TypeLocation(INamedTypeSymbol type)
    {
        return type.Locations.Length > 0 ? type.Locations[0] : Location.None;
    }
}
