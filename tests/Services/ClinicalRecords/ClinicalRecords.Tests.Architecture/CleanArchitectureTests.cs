using NetArchTest.Rules;
using Xunit;

namespace PsiFlow.ClinicalRecords.Tests.Architecture;

public sealed class CleanArchitectureTests
{
    [Fact]
    public void Domain_Should_Not_Depend_On_Infrastructure_Or_Api()
    {
        var asm = typeof(global::ClinicalRecords.Domain.Entities.MedicalRecord).Assembly;
        var result = Types.InAssembly(asm)
            .ShouldNot().HaveDependencyOnAny("ClinicalRecords.Infrastructure", "ClinicalRecords.API", "Microsoft.EntityFrameworkCore", "Microsoft.AspNetCore")
            .GetResult();
        Assert.True(result.IsSuccessful, string.Join("; ", result.FailingTypeNames ?? new System.Collections.Generic.List<string>()));
    }
    [Fact]
    public void Application_Should_Not_Depend_On_Infrastructure_Or_Api()
    {
        var asm = typeof(global::ClinicalRecords.Application.Contracts.IMedicalRecordService).Assembly;
        var result = Types.InAssembly(asm)
            .ShouldNot().HaveDependencyOnAny("ClinicalRecords.Infrastructure", "ClinicalRecords.API")
            .GetResult();
        Assert.True(result.IsSuccessful, string.Join("; ", result.FailingTypeNames ?? new System.Collections.Generic.List<string>()));
    }
    [Fact]
    public void Infrastructure_Should_Not_Depend_On_Api()
    {
        var asm = typeof(Infrastructure.Persistence.Data.ClinicalRecordsDbContext).Assembly;
        var result = Types.InAssembly(asm)
            .ShouldNot().HaveDependencyOn("ClinicalRecords.API").GetResult();
        Assert.True(result.IsSuccessful, string.Join("; ", result.FailingTypeNames ?? new System.Collections.Generic.List<string>()));
    }
}
