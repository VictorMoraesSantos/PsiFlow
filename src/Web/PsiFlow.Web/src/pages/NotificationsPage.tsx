import { ResourcePage, statusBadge } from '../components/ResourcePage';
import { Button } from '../components/Button';
import { Section } from '../components/Section';
import { useToast } from '../components/Toast';
import { api, isLocalFallbackStatus } from '../services/api';
import type { FormField, NotificationTemplate } from '../types';
import { useState } from 'react';

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
  const { notify } = useToast();
  const [logs, setLogs] = useState<Array<{ id: number; recipientEmail?: string; status?: string; templateKey?: string }>>([]);

  async function loadLogs() {
    try {
      const loaded = await api.get<Array<{ id: number; recipientEmail?: string; status?: string; templateKey?: string }>>('notifications', '/v1/notification-logs');
      setLogs(loaded);
      notify('Logs de notificacao carregados.');
    } catch (error) {
      if (isLocalFallbackStatus(error)) {
        setLogs([]);
        notify('Logs indisponiveis, mantendo modo local.', 'info');
        return;
      }
      notify(error instanceof Error ? error.message : 'Nao foi possivel carregar logs.', 'danger');
    }
  }

  async function retryFirstFailed() {
    const failed = logs.find((log) => log.status === 'failed') ?? logs[0];
    if (!failed) {
      notify('Carregue os logs antes de tentar retry.', 'info');
      return;
    }
    try {
      await api.post('notifications', `/v1/notifications/retry/${failed.id}`);
      notify('Retry solicitado.');
    } catch (error) {
      if (isLocalFallbackStatus(error)) {
        notify('Retry registrado localmente.', 'info');
        return;
      }
      notify(error instanceof Error ? error.message : 'Nao foi possivel solicitar retry.', 'danger');
    }
  }

  return (
    <div className="resource-layout">
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
      <Section title="Logs e reprocessamento" description="Acompanhe entregas transacionais e reenvie falhas sem abrir outro sistema.">
        <div className="workflow-actions">
          <Button type="button" variant="secondary" onClick={loadLogs}>Carregar logs</Button>
          <Button type="button" onClick={retryFirstFailed}>Reprocessar falha</Button>
        </div>
        <div className="workflow-panel workflow-panel--wide">
          {logs.length ? <ul>{logs.slice(0, 8).map((log) => <li key={log.id}>#{log.id} {log.templateKey || 'template'} para {log.recipientEmail || 'destinatario'}, {log.status || 'sem status'}</li>)}</ul> : <p>Nenhum log carregado.</p>}
        </div>
      </Section>
    </div>
  );
}
