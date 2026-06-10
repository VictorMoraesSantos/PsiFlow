using Core.Domain.Filters;
using Core.Infrastructure.Repositories;
using Notifications.Domain.Aggregates;
using Notifications.Domain.Filters;
using Notifications.Domain.Filters.Specifications;
using Notifications.Domain.Repositories;
using PsiFlow.Notifications.Infrastructure.Persistence;

namespace Notifications.Infrastructure.Persistence.Repositories;

public sealed class NotificationTemplateRepository(NotificationsDbContext dbContext) : Repository<NotificationTemplate, int, NotificationTemplateQueryFilter>(dbContext), INotificationTemplateRepository
{
    protected override Specification<NotificationTemplate, int> CreateSpecification(NotificationTemplateQueryFilter filter) => new NotificationTemplateSpecification(filter);
}
