using NetArchTest.Rules;
using Xunit;

namespace PsiFlow.Agenda.Tests.Architecture;

public sealed class CleanArchitectureTests
{
    [Fact]
    public void Domain_Should_Not_Depend_On_Infrastructure_Or_Api()
    {
        var asm = typeof(global::Agenda.Domain.Entities.Appointment).Assembly;
        var result = Types.InAssembly(asm)
            .ShouldNot().HaveDependencyOnAny("Agenda.Infrastructure", "Agenda.API", "Microsoft.EntityFrameworkCore", "Microsoft.AspNetCore")
            .GetResult();
        Assert.True(result.IsSuccessful, string.Join("; ", result.FailingTypeNames ?? new System.Collections.Generic.List<string>()));
    }
    [Fact]
    public void Application_Should_Not_Depend_On_Infrastructure_Or_Api()
    {
        var asm = typeof(global::Agenda.Application.Contracts.IAppointmentService).Assembly;
        var result = Types.InAssembly(asm)
            .ShouldNot().HaveDependencyOnAny("Agenda.Infrastructure", "Agenda.API")
            .GetResult();
        Assert.True(result.IsSuccessful, string.Join("; ", result.FailingTypeNames ?? new System.Collections.Generic.List<string>()));
    }
    [Fact]
    public void Infrastructure_Should_Not_Depend_On_Api()
    {
        var asm = typeof(Infrastructure.Persistence.Data.AgendaDbContext).Assembly;
        var result = Types.InAssembly(asm)
            .ShouldNot().HaveDependencyOn("Agenda.API").GetResult();
        Assert.True(result.IsSuccessful, string.Join("; ", result.FailingTypeNames ?? new System.Collections.Generic.List<string>()));
    }
}
