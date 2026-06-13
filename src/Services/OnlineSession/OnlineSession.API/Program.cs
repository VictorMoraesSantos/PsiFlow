using BuildingBlocks.Authentication;
using BuildingBlocks.Authorization;
using Core.API;
using OnlineSession.API.Endpoints;
using OnlineSession.Application;
using OnlineSession.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration).Enrich.FromLogContext().WriteTo.Console());

builder.Services.AddOnlineSessionApplication();
builder.Services.AddOnlineSessionInfrastructure(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddPsiFlowAuthorization();
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
app.MapVideoRoomsEndpoints();
app.MapOnlineSessionEndpoints();
app.Run();
