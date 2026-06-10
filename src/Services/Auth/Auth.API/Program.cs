using Auth.API.Endpoints;
using Auth.Application;
using Auth.Application.Settings;
using Auth.Infrastructure;
using Auth.Infrastructure.Persistence.Seeds;
using Microsoft.AspNetCore.Mvc;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration).Enrich.FromLogContext().WriteTo.Console());

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>() ?? new JwtSettings();
if (string.IsNullOrWhiteSpace(jwtSettings.Key))
    jwtSettings.Key = "PsiFlow-Dev-Key-CHANGE-IN-PRODUCTION-please-32-bytes!!";

builder.Services.AddSingleton(jwtSettings);
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();
builder.Services.AddHostedService<AuthDataSeeder>();
builder.Services.AddProblemDetails();
builder.Services.AddCors();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseAuthorization();
app.MapHealthChecks("/health/live");
app.MapHealthChecks("/health/ready");
app.MapHealthChecks("/health/startup");
app.MapAuthEndpoints();
app.MapJwksEndpoint();
app.Run();
