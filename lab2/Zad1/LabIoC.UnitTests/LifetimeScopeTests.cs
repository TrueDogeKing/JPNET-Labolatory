namespace LabIoC.UnitTests;

using Autofac;
using FluentAssertions;
using LabIoC.Core;
using Xunit;

public class LifetimeScopeTests
{
    [Fact]
    public void UnitOfWork_LifetimeScope_ShouldBeSharedWithinScopeAndDifferentAcrossScopes()
    {
        var builder = new ContainerBuilder();
        builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();
        using var container = builder.Build();

        using var scope1 = container.BeginLifetimeScope();
        var uow1a = scope1.Resolve<IUnitOfWork>();
        var uow1b = scope1.Resolve<IUnitOfWork>();

        using var scope2 = container.BeginLifetimeScope();
        var uow2 = scope2.Resolve<IUnitOfWork>();

        uow1a.Id.Should().Be(uow1b.Id);
        uow1a.Id.Should().NotBe(uow2.Id);
    }
}