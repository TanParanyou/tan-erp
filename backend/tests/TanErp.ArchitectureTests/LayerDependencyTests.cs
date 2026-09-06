using ArchUnitNET.Domain;
using ArchUnitNET.Loader;
using ArchUnitNET.xUnit;
using Xunit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;
using Assembly = System.Reflection.Assembly;

namespace TanErp.ArchitectureTests;

public class LayerDependencyTests
{
    private static readonly Assembly DomainAssembly = typeof(TanErp.Domain.DomainMarker).Assembly;
    private static readonly Assembly ApplicationAssembly = typeof(TanErp.Application.ApplicationMarker).Assembly;
    private static readonly Assembly InfrastructureAssembly = typeof(TanErp.Infrastructure.InfrastructureMarker).Assembly;
    private static readonly Assembly ApiAssembly = typeof(TanErp.Api.ApiMarker).Assembly;

    private static readonly Architecture Architecture = new ArchLoader()
        .LoadAssemblies(DomainAssembly, ApplicationAssembly, InfrastructureAssembly, ApiAssembly)
        .Build();

    [Fact]
    public void Domain_ShouldNotDependOn_Application_Infrastructure_Or_Api()
    {
        var rule = Types().That().ResideInAssembly(DomainAssembly)
            .Should().NotDependOnAny(Types().That().ResideInAssembly(ApplicationAssembly))
            .AndShould().NotDependOnAny(Types().That().ResideInAssembly(InfrastructureAssembly))
            .AndShould().NotDependOnAny(Types().That().ResideInAssembly(ApiAssembly));

        rule.Check(Architecture);
    }

    [Fact]
    public void Application_ShouldNotDependOn_Infrastructure_Or_Api()
    {
        var rule = Types().That().ResideInAssembly(ApplicationAssembly)
            .Should().NotDependOnAny(Types().That().ResideInAssembly(InfrastructureAssembly))
            .AndShould().NotDependOnAny(Types().That().ResideInAssembly(ApiAssembly));

        rule.Check(Architecture);
    }

    [Fact]
    public void Infrastructure_ShouldNotDependOn_Api()
    {
        var rule = Types().That().ResideInAssembly(InfrastructureAssembly)
            .Should().NotDependOnAny(Types().That().ResideInAssembly(ApiAssembly));

        rule.Check(Architecture);
    }
}
