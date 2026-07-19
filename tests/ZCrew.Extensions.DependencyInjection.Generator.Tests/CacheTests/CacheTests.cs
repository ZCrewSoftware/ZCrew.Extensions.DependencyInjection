using Microsoft.CodeAnalysis;
using ZCrew.Extensions.DependencyInjection.Generator.Tests.TestHelpers;

namespace ZCrew.Extensions.DependencyInjection.Generator.Tests.CacheTests;

public class CacheTests
{
    private const string Widget = """
        using ZCrew.Extensions.DependencyInjection.Registration;

        namespace Widgets;

        public interface IWidget;

        [Service(typeof(IWidget))]
        public class Widget : IWidget;
        """;

    private const string Gadget = """
        using ZCrew.Extensions.DependencyInjection.Registration;

        namespace Gadgets;

        public interface IGadget;

        [Service(typeof(IGadget))]
        public class Gadget : IGadget;
        """;

    private const string RenamedGadget = """
        using ZCrew.Extensions.DependencyInjection.Registration;

        namespace Gadgets;

        public interface IGadget;

        [Service(typeof(IGadget))]
        public class RenamedGadget : IGadget;
        """;

    [Fact]
    public void Generate_WhenUnrelatedTypeEdited_ShouldReuseCachedRegistration()
    {
        // Arrange
        var run = GeneratorHarness.Run(Widget, Gadget);

        // Act
        var rerun = run.Update(1, RenamedGadget);

        // Assert
        var registrations = rerun
            .ResultOf<ServiceRegistrationSourceGenerator>()
            .TrackedSteps["ZCrewDI_ServiceRegistrations"]
            .SelectMany(step => step.Outputs)
            .ToList();
        Assert.Equal(2, registrations.Count);
        Assert.Single(registrations, output => output.Reason == IncrementalStepRunReason.Modified);
        Assert.Single(
            registrations,
            output => output.Reason is IncrementalStepRunReason.Cached or IncrementalStepRunReason.Unchanged
        );
    }
}
