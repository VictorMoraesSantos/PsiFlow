using System.Reflection;
using NetArchTest.Rules;
using Xunit;

namespace PsiFlow.Notifications.Tests.Architecture;

public sealed class CleanArchitectureTests
{
    [Fact]
    public void Domain_Should_Not_Depend_On_Infrastructure_Or_Api()
    {
        var asm = typeof(global::Notifications.Domain.Aggregates.NotificationTemplate).Assembly;
        var result = Types.InAssembly(asm)
            .ShouldNot().HaveDependencyOnAny("Notifications.Infrastructure", "Notifications.API", "Microsoft.EntityFrameworkCore", "Microsoft.AspNetCore")
            .GetResult();
        Assert.True(result.IsSuccessful, string.Join("; ", result.FailingTypeNames ?? new System.Collections.Generic.List<string>()));
    }
    [Fact]
    public void Application_Should_Not_Depend_On_Infrastructure_Or_Api()
    {
        var asm = typeof(global::Notifications.Application.Contracts.INotificationTemplateService).Assembly;
        var result = Types.InAssembly(asm)
            .ShouldNot().HaveDependencyOnAny("Notifications.Infrastructure", "Notifications.API")
            .GetResult();
        Assert.True(result.IsSuccessful, string.Join("; ", result.FailingTypeNames ?? new System.Collections.Generic.List<string>()));
    }
    [Fact]
    public void Infrastructure_Should_Not_Depend_On_Api()
    {
        var asm = typeof(Infrastructure.Persistence.NotificationsDbContext).Assembly;
        var result = Types.InAssembly(asm)
            .ShouldNot().HaveDependencyOn("Notifications.API").GetResult();
        Assert.True(result.IsSuccessful, string.Join("; ", result.FailingTypeNames ?? new System.Collections.Generic.List<string>()));
    }
}
