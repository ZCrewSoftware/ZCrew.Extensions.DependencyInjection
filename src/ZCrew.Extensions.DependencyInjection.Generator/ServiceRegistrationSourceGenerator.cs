using Microsoft.CodeAnalysis;
using ZCrew.Extensions.DependencyInjection.Generator.Registration;
using ZCrew.Extensions.DependencyInjection.Registration;

namespace ZCrew.Extensions.DependencyInjection.Generator;

/// <summary>
///     Scans for <c>[Service]</c> types and emits
///     <c>ZCrew.Extensions.DependencyInjection.Registration.Services.FromThisAssembly()</c>: the compile-time list of
///     <c>Service</c> registrations that replaces reflection-based assembly scanning. The <c>[Service]</c> family of
///     attributes (<c>[Service]</c>, <c>[As]</c>, <c>[Singleton]</c>, <c>[Scoped]</c>, <c>[Transient]</c>,
///     <c>[Keyed]</c>) is embedded into the consuming compilation, so it exists only where this generator runs.
/// </summary>
[Generator(LanguageNames.CSharp)]
internal sealed class ServiceRegistrationSourceGenerator : RegistrationScanSourceGenerator
{
    /// <inheritdoc/>
    protected override string MetadataName => "ZCrew.Extensions.DependencyInjection.Registration.ServiceAttribute";

    /// <inheritdoc/>
    protected override string TrackingName => "ZCrewDI_ServiceRegistrations";

    /// <inheritdoc/>
    protected override RegistrationEmitConfig EmitConfig { get; } =
        new(
            "ZCrew.Extensions.DependencyInjection.Registration",
            "Services",
            "global::ZCrew.Extensions.DependencyInjection.Registration.Service",
            "global::ZCrew.Extensions.DependencyInjection.Registration.ServiceFilter"
        );

    /// <inheritdoc/>
    protected override void RegisterAttributeDefinitions(IncrementalGeneratorPostInitializationContext context)
    {
        context.AddEmbeddedAttributeDefinition();
        context.AddServiceAttributeDefinition();
        context.AddAsAttributeDefinition();
        context.AddSingletonAttributeDefinition();
        context.AddScopedAttributeDefinition();
        context.AddTransientAttributeDefinition();
        context.AddKeyedAttributeDefinition();
    }

    /// <inheritdoc/>
    protected override string RenderConstruction(INamedTypeSymbol type)
    {
        return ServiceConstructionRenderer.Render(type);
    }
}
