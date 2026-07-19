using Microsoft.CodeAnalysis;
using ZCrew.Extensions.DependencyInjection.Generator.Registration;

namespace ZCrew.Extensions.DependencyInjection.Generator;

/// <summary>
///     Scans for <c>[Service]</c> types and emits
///     <c>ZCrew.Extensions.DependencyInjection.Registration.Services.FromThisAssembly()</c>: the compile-time list of
///     <c>Service</c> registrations that replaces reflection-based assembly scanning.
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
}
