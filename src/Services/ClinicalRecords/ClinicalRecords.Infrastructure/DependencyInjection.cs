using ClinicalRecords.Domain.Repositories;
using ClinicalRecords.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PsiFlow.ClinicalRecords.Infrastructure.Persistence.Data;

namespace ClinicalRecords.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddClinicalRecordsInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<global::PsiFlow.ClinicalRecords.Infrastructure.Persistence.Data.ClinicalRecordsDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("Database")));
            services.AddScoped<IMedicalRecordRepository, MedicalRecordRepository>();
            services.AddScoped<IAnamnesisRepository, AnamnesisRepository>();
            services.AddScoped<IAnamnesisVersionRepository, AnamnesisVersionRepository>();
            services.AddScoped<IEvolutionRepository, EvolutionRepository>();
            services.AddScoped<IEvolutionVersionRepository, EvolutionVersionRepository>();
            services.AddScoped<IClinicalAuditLogRepository, ClinicalAuditLogRepository>();
            services.AddHostedService<ClinicalRecordsDatabaseInitializer>();
            return services;
        }
    }
}
