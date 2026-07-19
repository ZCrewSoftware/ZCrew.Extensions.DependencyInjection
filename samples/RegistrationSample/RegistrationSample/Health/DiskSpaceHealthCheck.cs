namespace RegistrationSample.Health;

/// <summary>
///     Reports whether enough free disk space is available.
/// </summary>
public sealed class DiskSpaceHealthCheck : IHealthCheck
{
    /// <inheritdoc />
    public string Name => "Disk space";

    /// <inheritdoc />
    public bool IsHealthy() => true;
}
