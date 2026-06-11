using NetArchTest.Rules;
using Xunit;

namespace PsiFlow.Sessions.Tests.Architecture;

public sealed class CleanArchitectureTests
{
    [Fact]
    public void Domain_Should_Not_Depend_On_Infrastructure_Or_Api()
    {
        var asm = typeof(global::Sessions.Domain.Entities.Session).Assembly;
        var result = Types.InAssembly(asm)
            .ShouldNot().HaveDependencyOnAny("Sessions.Infrastructure", "Sessions.API", "Microsoft.EntityFrameworkCore", "Microsoft.AspNetCore")
            .GetResult();
        Assert.True(result.IsSuccessful, string.Join("; ", result.FailingTypeNames ?? new System.Collections.Generic.List<string>()));
    }
    [Fact]
    public void Application_Should_Not_Depend_On_Infrastructure_Or_Api()
    {
        var asm = typeof(global::Sessions.Application.Contracts.ISessionService).Assembly;
        var result = Types.InAssembly(asm)
            .ShouldNot().HaveDependencyOnAny("Sessions.Infrastructure", "Sessions.API")
            .GetResult();
        Assert.True(result.IsSuccessful, string.Join("; ", result.FailingTypeNames ?? new System.Collections.Generic.List<string>()));
    }
    [Fact]
    public void Infrastructure_Should_Not_Depend_On_Api()
    {
        var asm = typeof(Infrastructure.Persistence.Data.SessionsDbContext).Assembly;
        var result = Types.InAssembly(asm)
            .ShouldNot().HaveDependencyOn("Sessions.API").GetResult();
        Assert.True(result.IsSuccessful, string.Join("; ", result.FailingTypeNames ?? new System.Collections.Generic.List<string>()));
    }
}
