import { api } from './http';
import type {
  NotificationLog,
  NotificationScheduleReminders,
  NotificationTemplate,
  NotificationTemplateVersion,
  NotificationTestEmail,
} from '../types';

export const notificationTemplatesApi = {
  list: () => api.get<NotificationTemplate[]>('/api/notifications/v1/notification-templates'),
  create: (payload: NotificationTemplate) =>
    api.post<NotificationTemplate>('/api/notifications/v1/notification-templates', payload),
  update: (id: number, payload: NotificationTemplate) =>
    api.put<NotificationTemplate>(`/api/notifications/v1/notification-templates/${id}`, payload),
  createVersion: (id: number, payload: NotificationTemplateVersion) =>
    api.post<unknown>(`/api/notifications/v1/notification-templates/${id}/versions`, payload),
};

export const notificationLogsApi = {
  list: () => api.get<NotificationLog[]>('/api/notifications/v1/notification-logs'),
  retry: (id: number) => api.post<unknown>(`/api/notifications/v1/notifications/retry/${id}`),
};

export const notificationsApi = {
  sendTestEmail: (payload: NotificationTestEmail) =>
    api.post<unknown>('/api/notifications/v1/notifications/test-email', payload),
  scheduleReminders: (payload: NotificationScheduleReminders) =>
    api.post<unknown>('/api/notifications/v1/notifications/schedule-reminders', payload),
};
