export type NotificationTemplate = {
  id: number;
  tenantId?: number | null;
  key?: string;
  name: string;
  channel: 'Email' | 'WhatsApp' | 'Sistema';
  status: 'Ativo' | 'Pausado';
  isActive?: boolean;
};

export type NotificationLog = {
  id: number;
  recipientEmail?: string;
  status?: string;
  templateKey?: string;
};

export type NotificationTestEmail = {
  recipientEmail: string;
  templateKey: string;
};

export type NotificationScheduleReminders = {
  notificationType: string;
  scheduledFor: string;
  recipientEmail: string;
  recipientUserId: number | null;
  payloadJson: string;
};

export type NotificationTemplateVersion = {
  subject: string;
  bodyHtml: string;
  bodyText: string;
};
