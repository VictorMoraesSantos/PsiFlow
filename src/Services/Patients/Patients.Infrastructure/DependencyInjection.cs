using Patients.Application.Contracts;
using Patients.Domain.Repositories;
using Patients.Infrastructure.Persistence.Repositories;
using Patients.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Patients.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPatientsInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<global::PsiFlow.Patients.Infrastructure.Persistence.PatientsDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("Database")));
            services.AddScoped<IPatientRepository, PatientRepository>();
            services.AddScoped<IPatientService, PatientService>();
            services.AddScoped<IPatientInviteService, PatientInviteService>();
            return services;
        }
    }
}
