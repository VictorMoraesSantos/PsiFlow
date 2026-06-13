using Core.Domain.Filters;
using Core.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Notifications.Domain.Entities;
using Notifications.Domain.Filters;
using Notifications.Domain.Filters.Specifications;
using Notifications.Domain.Repositories;
using PsiFlow.Notifications.Infrastructure.Persistence.Data;

namespace Notifications.Infrastructure.Persistence.Repositories;

public sealed class NotificationTemplateRepository(NotificationsDbContext dbContext) : Repository<NotificationTemplate, int, NotificationTemplateQueryFilter>(dbContext), INotificationTemplateRepository
{
    protected override Specification<NotificationTemplate, int> CreateSpecification(NotificationTemplateQueryFilter filter) => new NotificationTemplateSpecification(filter);

    public Task<bool> AnyByIdAsync(int id, CancellationToken cancellationToken = default) =>
        dbContext.NotificationTemplates.AsNoTracking().AnyAsync(x => x.Id == id, cancellationToken);
}
