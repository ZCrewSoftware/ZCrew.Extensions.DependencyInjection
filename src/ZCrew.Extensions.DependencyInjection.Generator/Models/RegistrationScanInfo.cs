using ZCrew.Extensions.CodeAnalysis.CSharp.Collections;

namespace ZCrew.Extensions.DependencyInjection.Generator.Models;

/// <summary>
///     One decorated type and its rendered attribute constructions. The scan attributes allow multiple, so a single
///     type can yield several constructions. Value-equatable so an unchanged type is cached across incremental runs.
/// </summary>
/// <param name="ImplementationTypeof">The fully-qualified, open-generic type name for a <c>typeof</c> expression.</param>
/// <param name="AttributeConstructions">The rendered <c>new XxxAttribute(...)</c> expressions, one per attribute.</param>
internal sealed record RegistrationScanInfo(string ImplementationTypeof, EquatableArray<string> AttributeConstructions);
