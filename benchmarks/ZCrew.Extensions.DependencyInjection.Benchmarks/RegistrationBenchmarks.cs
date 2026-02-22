using System.Reflection;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.InProcess.Emit;
using Castle.Windsor;
using Fixtures.LargeProject.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using CastleClasses = Castle.MicroKernel.Registration.Classes;
using ZCrewClasses = ZCrew.Extensions.DependencyInjection.Registration.Classes;

namespace ZCrew.Extensions.DependencyInjection.Benchmarks;

[MemoryDiagnoser]
[Config(typeof(InProcessConfig))]
public class RegistrationBenchmarks
{
    private class InProcessConfig : ManualConfig
    {
        public InProcessConfig()
        {
            AddJob(Job.Default.WithToolchain(InProcessEmitToolchain.Instance));
        }
    }

    public enum ProjectSize
    {
        Small,
        Large,
    }

    [Params(ProjectSize.Small, ProjectSize.Large)]
    public ProjectSize Size { get; set; }

    private Assembly assembly = null!;

    [GlobalSetup]
    public void Setup()
    {
        this.assembly = Size switch
        {
            ProjectSize.Small => typeof(
                Fixtures.SmallProject.Application.Services.ICustomerService
            ).Assembly,
            ProjectSize.Large => typeof(IService1).Assembly,
            _ => throw new ArgumentOutOfRangeException(),
        };
    }

    // ── Scenario 1: All public classes → all interfaces ──

    [Benchmark]
    public int ZCrew_AllInterfaces()
    {
        var services = new ServiceCollection();
        services.AddSingleton(ZCrewClasses.FromAssembly(this.assembly).AsAllInterfaces());
        return services.Count;
    }

    [Benchmark]
    public int Scrutor_AllInterfaces()
    {
        var services = new ServiceCollection();
        services.Scan(scan =>
            scan.FromAssemblies(this.assembly)
                .AddClasses()
                .AsImplementedInterfaces()
                .WithSingletonLifetime()
        );
        return services.Count;
    }

    [Benchmark]
    public IWindsorContainer Windsor_AllInterfaces()
    {
        var container = new WindsorContainer();
        container.Register(
            CastleClasses.FromAssembly(this.assembly).Pick().WithService.AllInterfaces()
        );
        return container;
    }

    // ── Scenario 2: All public classes → default/matching interface ──

    [Benchmark]
    public int ZCrew_DefaultInterfaces()
    {
        var services = new ServiceCollection();
        services.AddSingleton(ZCrewClasses.FromAssembly(this.assembly).AsDefaultInterfaces());
        return services.Count;
    }

    [Benchmark]
    public int Scrutor_DefaultInterfaces()
    {
        var services = new ServiceCollection();
        services.Scan(scan =>
            scan.FromAssemblies(this.assembly)
                .AddClasses()
                // Can't use AsMatchingInterface() here since that is not available from Scrutor
                .As(type => type.GetInterfaces().Where(service => type.Name.Contains(service.GetInterfaceName())))
                .WithSingletonLifetime()
        );
        return services.Count;
    }

    [Benchmark]
    public IWindsorContainer Windsor_DefaultInterfaces()
    {
        var container = new WindsorContainer();
        container.Register(
            CastleClasses.FromAssembly(this.assembly).Pick().WithService.DefaultInterfaces()
        );
        return container;
    }

    // ── Scenario 3: All public classes → self ──

    [Benchmark]
    public int ZCrew_AsSelf()
    {
        var services = new ServiceCollection();
        services.AddSingleton(ZCrewClasses.FromAssembly(this.assembly).AsSelf());
        return services.Count;
    }

    [Benchmark]
    public int Scrutor_AsSelf()
    {
        var services = new ServiceCollection();
        services.Scan(scan =>
            scan.FromAssemblies(this.assembly).AddClasses().AsSelf().WithSingletonLifetime()
        );
        return services.Count;
    }

    [Benchmark]
    public IWindsorContainer Windsor_AsSelf()
    {
        var container = new WindsorContainer();
        container.Register(
            CastleClasses.FromAssembly(this.assembly).Pick().WithService.Self()
        );
        return container;
    }

    // ── Scenario 4: All types (incl. internal) → all interfaces ──

    [Benchmark]
    public int ZCrew_InternalTypes_AllInterfaces()
    {
        var services = new ServiceCollection();
        services.AddSingleton(ZCrewClasses
            .FromAssembly(this.assembly)
            .IncludeInternalTypes()
            .AsAllInterfaces());
        return services.Count;
    }

    [Benchmark]
    public int Scrutor_InternalTypes_AllInterfaces()
    {
        var services = new ServiceCollection();
        services.Scan(scan =>
            scan.FromAssemblies(this.assembly)
                .AddClasses(publicOnly: false)
                .AsImplementedInterfaces()
                .WithSingletonLifetime()
        );
        return services.Count;
    }

    [Benchmark]
    public IWindsorContainer Windsor_InternalTypes_AllInterfaces()
    {
        var container = new WindsorContainer();
        container.Register(
            CastleClasses
                .FromAssembly(this.assembly)
                .IncludeNonPublicTypes()
                .Pick()
                .WithService.AllInterfaces()
        );
        return container;
    }

    // ── Scenario 5: Filter by base type → interface ──

    [Benchmark]
    public int ZCrew_BasedOn_AsInterface()
    {
        var services = new ServiceCollection();
        services.AddSingleton(ZCrewClasses
            .FromAssembly(this.assembly)
            .BasedOn<IService1>()
            .AsInterface());
        return services.Count;
    }

    [Benchmark]
    public int Scrutor_BasedOn_AsInterface()
    {
        var services = new ServiceCollection();
        services.Scan(scan =>
            scan.FromAssemblies(this.assembly)
                .AddClasses(f => f.AssignableTo<IService1>())
                // TODO: this isn't the same as what the other systems are doing... so of course it performs better
                .AsImplementedInterfaces()
                .WithSingletonLifetime()
        );
        return services.Count;
    }

    [Benchmark]
    public IWindsorContainer Windsor_BasedOn_AllInterfaces()
    {
        var container = new WindsorContainer();
        container.Register(
            CastleClasses
                .FromAssembly(this.assembly)
                .BasedOn<IService1>()
                .WithService.AllInterfaces()
        );
        return container;
    }

    // ── Scenario 6: First interface only ──

    [Benchmark]
    public int ZCrew_FirstInterface()
    {
        var services = new ServiceCollection();
        services.AddSingleton(ZCrewClasses.FromAssembly(this.assembly).AsFirstInterface());
        return services.Count;
    }

    [Benchmark]
    public int Scrutor_FirstInterface()
    {
        var services = new ServiceCollection();
        services.Scan(scan =>
            scan.FromAssemblies(this.assembly)
                .AddClasses()
                .As(t => t.GetInterfaces().Take(1))
                .WithSingletonLifetime()
        );
        return services.Count;
    }

    [Benchmark]
    public IWindsorContainer Windsor_FirstInterface()
    {
        var container = new WindsorContainer();
        container.Register(
            CastleClasses.FromAssembly(this.assembly).Pick().WithService.FirstInterface()
        );
        return container;
    }
}
