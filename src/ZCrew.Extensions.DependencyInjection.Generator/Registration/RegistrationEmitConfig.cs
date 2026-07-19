namespace ZCrew.Extensions.DependencyInjection.Generator.Registration;

/// <summary>
///     The per-attribute emission settings for one registration generator: the namespace the entry point lives in, the
///     entry-point class name, and the fully-qualified runtime type each entry is built through.
/// </summary>
/// <param name="Namespace">The namespace of the emitted entry-point type.</param>
/// <param name="EntryPointClassName">The static class exposing <c>FromThisAssembly()</c> (for example <c>Services</c>).</param>
/// <param name="ServiceTypeFullName">The <c>global::</c>-qualified runtime type each entry is built through (<c>Service</c>).</param>
/// <param name="ServiceFilterTypeFullName">
///     The <c>global::</c>-qualified filter wrapper the entry array is passed to and that <c>FromThisAssembly()</c>
///     returns (<c>ServiceFilter</c>).
/// </param>
internal sealed record RegistrationEmitConfig(
    string Namespace,
    string EntryPointClassName,
    string ServiceTypeFullName,
    string ServiceFilterTypeFullName
);
