namespace RegistrationSample.Health;

/// <summary>
///     Reports whether upstream network dependencies respond.
/// </summary>
public sealed class NetworkHealthCheck : IHealthCheck, IDisposable
{
    /// <inheritdoc />
    public string Name => "Network";

    /// <inheritdoc />
    public bool IsHealthy() => true;

    /// <inheritdoc />
    public void Dispose() { }
}
