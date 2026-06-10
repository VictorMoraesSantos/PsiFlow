using System.Reflection;
using NetArchTest.Rules;
using Xunit;

namespace PsiFlow.Patients.Tests.Architecture;

public sealed class CleanArchitectureTests
{
    [Fact]
    public void Domain_Should_Not_Depend_On_Infrastructure_Or_Api()
    {
        var asm = typeof(global::Patients.Domain.Aggregates.Patient).Assembly;
        var result = Types.InAssembly(asm)
            .ShouldNot().HaveDependencyOnAny("Patients.Infrastructure", "Patients.API", "Microsoft.EntityFrameworkCore", "Microsoft.AspNetCore")
            .GetResult();
        Assert.True(result.IsSuccessful, string.Join("; ", result.FailingTypeNames ?? new System.Collections.Generic.List<string>()));
    }

    [Fact]
    public void Application_Should_Not_Depend_On_Infrastructure_Or_Api()
    {
        var asm = typeof(global::Patients.Application.Contracts.IPatientService).Assembly;
        var result = Types.InAssembly(asm)
            .ShouldNot().HaveDependencyOnAny("Patients.Infrastructure", "Patients.API")
            .GetResult();
        Assert.True(result.IsSuccessful, string.Join("; ", result.FailingTypeNames ?? new System.Collections.Generic.List<string>()));
    }

    [Fact]
    public void Infrastructure_Should_Not_Depend_On_Api()
    {
        var asm = typeof(Infrastructure.Persistence.PatientsDbContext).Assembly;
        var result = Types.InAssembly(asm)
            .ShouldNot().HaveDependencyOn("Patients.API").GetResult();
        Assert.True(result.IsSuccessful, string.Join("; ", result.FailingTypeNames ?? new System.Collections.Generic.List<string>()));
    }
}
