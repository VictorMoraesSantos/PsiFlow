using BuildingBlocks.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireSaasAdmin", policy => policy.RequireRole("saas_admin"));
    options.AddPolicy("RequirePsychologist", policy => policy.RequireRole("psychologist", "saas_admin"));
    options.AddPolicy("RequirePatient", policy => policy.RequireRole("patient"));
    options.AddPolicy("RequirePsychologistOrPatient", policy => policy.RequireRole("psychologist", "patient", "saas_admin"));
});

builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "PsiFlow.ApiGateway" })).AllowAnonymous();

app.UseAuthentication();
app.UseAuthorization();

app.MapReverseProxy();

app.Run();
