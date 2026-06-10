import { ResourcePage, statusBadge } from '../components/ResourcePage';
import { api } from '../services/api';
import type { FormField, NotificationTemplate } from '../types';

const fields: Array<FormField<NotificationTemplate>> = [
  { name: 'key', label: 'Chave', type: 'text', required: true },
  { name: 'name', label: 'Nome', type: 'text', required: true },
  { name: 'channel', label: 'Canal', type: 'select', options: [{ label: 'Email', value: 'Email' }, { label: 'WhatsApp', value: 'WhatsApp' }, { label: 'Sistema', value: 'Sistema' }] },
  { name: 'status', label: 'Status', type: 'select', options: [{ label: 'Ativo', value: 'Ativo' }, { label: 'Pausado', value: 'Pausado' }] },
  { name: 'isActive', label: 'Ativo para envio', type: 'checkbox' },
];

const emptyTemplate: NotificationTemplate = { id: 0, tenantId: 1, key: '', name: '', channel: 'Email', status: 'Ativo', isActive: true };

type NotificationsPageProps = { templates: NotificationTemplate[]; onTemplatesChange: (templates: NotificationTemplate[]) => void };

export function NotificationsPage({ templates, onTemplatesChange }: NotificationsPageProps) {
  return (
    <ResourcePage
      title="Notificacoes"
      description="Modelos, canais, versoes e envios operacionais."
      createLabel="Criar modelo"
      items={templates}
      service="notifications"
      path="/v1/notification-templates"
      fields={fields}
      emptyValue={emptyTemplate}
      getId={(template) => template.id}
      getTitle={(template) => template.name}
      onItemsChange={onTemplatesChange}
      toCreatePayload={(template) => ({ tenantId: template.tenantId || null, key: template.key, channel: template.channel === 'WhatsApp' ? 'whatsapp' : template.channel === 'Sistema' ? 'system' : 'email', name: template.name, status: template.status === 'Pausado' ? 'paused' : 'active', isActive: template.isActive !== false })}
      toUpdatePayload={(template) => ({ id: template.id, tenantId: template.tenantId || null, key: template.key, channel: template.channel === 'WhatsApp' ? 'whatsapp' : template.channel === 'Sistema' ? 'system' : 'email', name: template.name, status: template.status === 'Pausado' ? 'paused' : 'active', isActive: template.isActive !== false })}
      columns={[
        { key: 'name', header: 'Modelo', render: (template) => <div className="person-cell"><strong>{template.name}</strong><span>{template.key}</span></div> },
        { key: 'channel', header: 'Canal', render: (template) => template.channel },
        { key: 'status', header: 'Status', render: (template) => statusBadge(template.status) },
      ]}
      detailFields={[
        { label: 'Chave', value: (template) => template.key || 'Nao informado' },
        { label: 'Canal', value: (template) => template.channel },
        { label: 'Ativo', value: (template) => template.isActive === false ? 'Nao' : 'Sim' },
        { label: 'Status', value: (template) => statusBadge(template.status) },
      ]}
      actions={[{ label: 'Nova versao', successMessage: 'Versao criada ou simulada.', run: (template) => api.post('notifications', `/v1/notification-templates/${template.id}/versions`, { subject: template.name, bodyHtml: '<p>Mensagem do PsiFlow</p>', bodyText: 'Mensagem do PsiFlow' }) }, { label: 'Enviar teste', successMessage: 'Email de teste enviado ou simulado.', run: (template) => api.post('notifications', '/v1/notifications/test-email', { recipientEmail: 'teste@psiflow.local', templateKey: template.key }) }, { label: 'Agendar lembrete', successMessage: 'Lembrete agendado ou simulado.', run: () => api.post('notifications', '/v1/notifications/schedule-reminders', { notificationType: 'appointment_reminder', scheduledFor: new Date(Date.now() + 3600000).toISOString(), recipientEmail: 'teste@psiflow.local', recipientUserId: null, payloadJson: '{}' }) }]}
    />
  );
}
