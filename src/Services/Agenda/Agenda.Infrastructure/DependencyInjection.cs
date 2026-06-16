using Agenda.Application.Contracts;
using Agenda.Domain.Repositories;
using Agenda.Infrastructure.Notifications;
using Agenda.Infrastructure.Persistence.Repositories;
using Agenda.Infrastructure.Sessions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PsiFlow.Agenda.Infrastructure.Persistence.Data;

namespace Agenda.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAgendaInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AgendaDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("Database")));
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            services.AddScoped<IAvailabilityRepository, AvailabilityRepository>();
            services.AddScoped<IScheduleBlockRepository, ScheduleBlockRepository>();
            services.AddHttpClient<IAppointmentNotificationProvider, HttpAppointmentNotificationProvider>();
            services.AddHttpClient<IAppointmentSessionProvider, HttpAppointmentSessionProvider>();
            services.AddSingleton<IPatientRelationshipProvider, AllowTenantPatientRelationshipProvider>();
            services.AddHostedService<AgendaDatabaseInitializer>();
            return services;
        }
    }
}
