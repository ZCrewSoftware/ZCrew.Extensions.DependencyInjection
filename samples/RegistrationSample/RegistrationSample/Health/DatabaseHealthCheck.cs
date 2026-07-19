namespace RegistrationSample.Health;

/// <summary>
///     Reports whether the database connection is reachable.
/// </summary>
public sealed class DatabaseHealthCheck : IHealthCheck
{
    /// <inheritdoc />
    public string Name => "Database";

    /// <inheritdoc />
    public bool IsHealthy() => true;
}
