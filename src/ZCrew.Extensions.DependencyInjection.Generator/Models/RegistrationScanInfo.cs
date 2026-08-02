namespace ZCrew.Extensions.DependencyInjection.Generator.Models;

/// <summary>
///     One decorated type and its rendered <c>Service.From</c> arguments. A <c>[Service]</c> type maps to exactly one
///     registration (the modifier attributes decide its lifetime, key, and service types). Value-equatable so an
///     unchanged type is cached across incremental runs.
/// </summary>
/// <param name="ImplementationTypeof">The fully-qualified, open-generic type name for a <c>typeof</c> expression.</param>
/// <param name="Construction">
///     The rendered arguments passed to <c>Service.From(typeof(impl), ...)</c>: the lifetime, the implementation key,
///     and each service type paired with its key.
/// </param>
internal sealed record RegistrationScanInfo(string ImplementationTypeof, string Construction);
