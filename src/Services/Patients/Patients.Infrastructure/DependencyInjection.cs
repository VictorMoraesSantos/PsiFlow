using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Patients.Domain.Repositories;
using Patients.Infrastructure.Persistence.Repositories;
using PsiFlow.Patients.Infrastructure.Persistence.Data;

namespace Patients.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPatientsInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<global::PsiFlow.Patients.Infrastructure.Persistence.Data.PatientsDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("Database")));
            services.AddScoped<IPatientRepository, PatientRepository>();
            services.AddScoped<IPatientStatusHistoryRepository, PatientStatusHistoryRepository>();
            services.AddScoped<IPatientInviteRepository, PatientInviteRepository>();
            services.AddHostedService<PatientsDatabaseInitializer>();
            return services;
        }
    }
}
