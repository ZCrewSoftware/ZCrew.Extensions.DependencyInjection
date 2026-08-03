namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     The shared trimmer-suppression justification for the reflection-based <see cref="Classes"/>/<see cref="Types"/>
///     registration chain.
/// </summary>
internal static class Aot
{
    /// <summary>
    ///     Every stage of the chain has internal constructors, so a chain can only start at a <see cref="Classes"/> or
    ///     <see cref="Types"/> entry point. Those are all annotated with
    ///     <see cref="System.Diagnostics.CodeAnalysis.RequiresUnreferencedCodeAttribute"/>, so the caller is warned
    ///     once for the whole chain instead of once per stage.
    /// </summary>
    internal const string Justification =
        "Every stage of the registration chain has internal constructors, so it is only reachable from the "
        + "RequiresUnreferencedCode entry points on Classes/Types. Those warn the caller once for the whole chain.";
}
