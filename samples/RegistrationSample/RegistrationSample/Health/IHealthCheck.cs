namespace RegistrationSample.Health;

/// <summary>
///     A probe over one subsystem. Every implementation in this namespace shares this shape, which is what makes
///     them a good fit for a single convention-based registration.
/// </summary>
public interface IHealthCheck
{
    /// <summary>A short name for the subsystem being checked.</summary>
    string Name { get; }

    /// <summary>Runs the probe and reports whether the subsystem is healthy.</summary>
    bool IsHealthy();
}
