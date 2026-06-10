using Sessions.API.Endpoints;
using Sessions.Application;
using Sessions.Infrastructure;
using BuildingBlocks.Authentication;
using Core.API;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration).Enrich.FromLogContext().WriteTo.Console());

builder.Services.AddSessionsApplication();
builder.Services.AddSessionsInfrastructure(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization(o =>
{
    o.AddPolicy("RequirePsychologist", p => p.RequireAuthenticatedUser().RequireRole("psychologist"));
    o.AddPolicy("RequirePatient", p => p.RequireAuthenticatedUser().RequireRole("patient"));
    o.AddPolicy("RequireSaasAdmin", p => p.RequireAuthenticatedUser().RequireRole("saas_admin"));
    o.AddPolicy("RequirePsychologistOrPatient", p => p.RequireAuthenticatedUser().RequireRole("psychologist", "patient"));
});
builder.Services.AddHealthChecks();
builder.Services.AddCoreApi();

var app = builder.Build();

app.UseCoreApi();

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
app.MapSessionsEndpoints();
app.MapSessionWorkflowEndpoints();
app.Run();
