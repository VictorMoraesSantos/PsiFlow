using BuildingBlocks.CQRS.Notification;
using BuildingBlocks.CQRS.Publisher;
using BuildingBlocks.CQRS.Requests.Request;
using BuildingBlocks.CQRS.Sender;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.CQRS.Extensions
{
    public static class MediatorExtensions
    {
        public static MediatorBuilder AddMediatorService(this IServiceCollection services)
        {
            services.AddScoped<ISender, Sender.Sender>();
            services.AddScoped<IPublisher, Publisher.Publisher>();
            var builder = new MediatorBuilder(services);
            return builder;
        }
    }

    public sealed class MediatorBuilder(IServiceCollection services)
    {
        public IServiceCollection Services { get; } = services;

        public MediatorBuilder AddHandler<TRequest, TResponse, THandler>()
            where TRequest : IRequest<TResponse>
            where THandler : class, IRequestHandler<TRequest, TResponse>
        {
            Services.AddScoped<IRequestHandler<TRequest, TResponse>, THandler>();
            return this;
        }

        public MediatorBuilder AddHandler<TRequest, THandler>()
            where TRequest : IRequest
            where THandler : class, IRequestHandler<TRequest>
        {
            Services.AddScoped<IRequestHandler<TRequest>, THandler>();
            return this;
        }

        public MediatorBuilder AddNotificationHandler<TNotification, THandler>()
            where TNotification : INotification
            where THandler : class, INotificationHandler<TNotification>
        {
            Services.AddScoped<INotificationHandler<TNotification>, THandler>();
            return this;
        }
    }
}