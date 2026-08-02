using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using ZCrew.Extensions.DependencyInjection.Generator.Emitters;
using ZCrew.Extensions.DependencyInjection.Generator.Models;

namespace ZCrew.Extensions.DependencyInjection.Generator.Registration;

/// <summary>
///     Shared pipeline for the attributed-registration scanners. Each subclass names one attribute; this base scans the
///     compilation for non-abstract classes and structs carrying it, re-emits every usage as a
///     <c>Service.From(typeof(impl), attribute)</c> element, and emits a single per-assembly
///     <c>Xxx.FromThisAssembly()</c> entry point holding the hard-coded list. The entry point is emitted only when at
///     least one type carries the attribute; the service type, key, and lifetime are left on the attribute for the
///     runtime to read, this only scans.
/// </summary>
internal abstract class RegistrationScanSourceGenerator : IIncrementalGenerator
{
    /// <summary>
    ///     The fully-qualified metadata name of the scan attribute, for example
    ///     <c>ZCrew.Extensions.DependencyInjection.Registration.ServiceAttribute</c>.
    /// </summary>
    protected abstract string MetadataName { get; }

    /// <summary>The incremental tracking name for the scan stage.</summary>
    protected abstract string TrackingName { get; }

    /// <summary>The emission settings: namespace, entry-point name, and the runtime type each entry is built through.</summary>
    protected abstract RegistrationEmitConfig EmitConfig { get; }

    /// <summary>
    ///     Emits the embedded attribute definitions this scanner recognizes into the consuming compilation via
    ///     post-initialization: <c>AddEmbeddedAttributeDefinition()</c> (the <c>[Embedded]</c> marker) plus each
    ///     attribute's generated <c>Add{Name}Definition()</c> helper. The scanner reads them back by metadata name, so
    ///     they exist only where the generator runs. Registered directly as the post-initialization callback, so this
    ///     is the single place attribute sources are emitted.
    /// </summary>
    /// <param name="context">The post-initialization context.</param>
    protected abstract void RegisterAttributeDefinitions(IncrementalGeneratorPostInitializationContext context);

    /// <summary>
    ///     Renders the arguments a scanned <paramref name="type"/> contributes to its
    ///     <c>Service.From(typeof(impl), ...)</c> call.
    /// </summary>
    /// <param name="type">The scanned type carrying the registration attributes.</param>
    /// <returns>The rendered argument list, excluding the leading <c>typeof(impl)</c>.</returns>
    protected abstract string RenderConstruction(INamedTypeSymbol type);

    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(RegisterAttributeDefinitions);

        var registrations = context
            .SyntaxProvider.ForAttributeWithMetadataName(
                MetadataName,
                static (node, _) => node is TypeDeclarationSyntax,
                (syntaxContext, _) => Transform(syntaxContext)
            )
            .Where(static info => info is not null)
            .Select(static (info, _) => info!)
            .WithTrackingName(TrackingName);

        // Collect into the single per-assembly entry point.
        var config = EmitConfig;
        context.RegisterSourceOutput(
            registrations.Collect(),
            (sourceContext, items) => Emit(sourceContext, config, items)
        );
    }

    private RegistrationScanInfo? Transform(GeneratorAttributeSyntaxContext context)
    {
        if (context.TargetSymbol is not INamedTypeSymbol type)
        {
            return null;
        }

        // Mirror the concrete-non-abstract Classes filter, widened to the struct target the attributes also allow.
        if (type.TypeKind is not (TypeKind.Class or TypeKind.Struct) || type.IsAbstract || type.IsStatic)
        {
            return null;
        }

        // Generic definitions register as open generics, so emit Foo<> rather than Foo<T>.
        var implementationType = type.IsGenericType ? type.ConstructUnboundGenericType() : type;

        return new RegistrationScanInfo(
            implementationType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            RenderConstruction(type)
        );
    }

    private static void Emit(
        SourceProductionContext context,
        RegistrationEmitConfig config,
        ImmutableArray<RegistrationScanInfo> items
    )
    {
        // Nothing carried the attribute, so there is no entry point to emit for this assembly.
        if (items.IsDefaultOrEmpty)
        {
            return;
        }

        var hintName = $"{config.Namespace}.{config.EntryPointClassName}.g.cs";
        context.AddSource(hintName, SourceText.From(RegistrationEmitter.Emit(config, items), Encoding.UTF8));
    }
}
