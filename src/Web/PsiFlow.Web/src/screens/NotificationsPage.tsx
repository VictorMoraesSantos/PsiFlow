'use client';

import { useState } from 'react';
import { Button } from '../components/Button';
import { ResourcePage, statusBadge, type ResourceCrud } from '../components/ResourcePage';
import { Section } from '../components/Section';
import { useToast } from '../components/Toast';
import { isLocalFallbackStatus } from '../services/http';
import {
  notificationLogsApi,
  notificationTemplatesApi,
  notificationsApi,
} from '../services/notifications';
import { useApp } from '../state/AppContext';
import type { FormField, NotificationTemplate } from '../types';

const fields: Array<FormField<NotificationTemplate>> = [
  { name: 'key', label: 'Chave', type: 'text', required: true },
  { name: 'name', label: 'Nome', type: 'text', required: true },
  { name: 'channel', label: 'Canal', type: 'select', options: [{ label: 'Email', value: 'Email' }, { label: 'WhatsApp', value: 'WhatsApp' }, { label: 'Sistema', value: 'Sistema' }] },
  { name: 'status', label: 'Status', type: 'select', options: [{ label: 'Ativo', value: 'Ativo' }, { label: 'Pausado', value: 'Pausado' }] },
  { name: 'isActive', label: 'Ativo para envio', type: 'checkbox' },
];

const emptyTemplate: NotificationTemplate = { id: 0, tenantId: 1, key: '', name: '', channel: 'Email', status: 'Ativo', isActive: true };

const templatesCrud: ResourceCrud<NotificationTemplate> = {
  create: (template) => notificationTemplatesApi.create(toPayload(template)),
  update: (id, template) => notificationTemplatesApi.update(Number(id), toPayload(template)),
  remove: () => Promise.resolve(),
};

function toPayload(template: NotificationTemplate): NotificationTemplate {
  return {
    ...template,
    channel: template.channel,
    status: template.status,
    isActive: template.isActive,
  };
}

export function NotificationsPage() {
  const { data, setData } = useApp();
  const templates = data.templates;
  const onTemplatesChange = (next: NotificationTemplate[]) => setData((current) => ({ ...current, templates: next }));
  const { notify } = useToast();
  const [logs, setLogs] = useState<Array<{ id: number; recipientEmail?: string; status?: string; templateKey?: string }>>([]);
  const failedLogs = logs.filter((log) => log.status === 'failed');

  async function loadLogs() {
    try {
      const loaded = await notificationLogsApi.list();
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
    const failed = failedLogs[0];
    if (!failed) {
      notify(logs.length ? 'Nenhuma falha encontrada para reprocessar.' : 'Carregue os logs antes de reprocessar falhas.', 'info');
      return;
    }
    try {
      await notificationLogsApi.retry(failed.id);
      notify(`Reprocessamento solicitado para o log #${failed.id}.`);
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
        description="Modelos, canais, versoes, testes e entregas operacionais."
        createLabel="Criar modelo"
        items={templates}
        crud={templatesCrud}
        fields={fields}
        emptyValue={emptyTemplate}
        getId={(template) => template.id}
        getTitle={(template) => template.name}
        onItemsChange={onTemplatesChange}
        toCreatePayload={toPayload}
        toUpdatePayload={toPayload}
        columns={[
          {
            key: 'name',
            header: 'Modelo',
            render: (template) => (
              <div className="person-cell">
                <strong>{template.name}</strong>
                <span>{template.key}</span>
              </div>
            ),
          },
          { key: 'channel', header: 'Canal', render: (template) => template.channel },
          { key: 'delivery', header: 'Uso seguro', render: (template) => (template.isActive === false || template.status === 'Pausado' ? 'Revisar antes de enviar' : 'Pronto para teste') },
          { key: 'status', header: 'Status', render: (template) => statusBadge(template.status) },
        ]}
        detailFields={[
          { label: 'Chave', value: (template) => template.key || 'Nao informado' },
          { label: 'Canal', value: (template) => template.channel },
          { label: 'Ativo', value: (template) => (template.isActive === false ? 'Nao' : 'Sim') },
          { label: 'Status', value: (template) => statusBadge(template.status) },
        ]}
        actions={[
          { label: 'Criar versao do modelo', successMessage: 'Versao do modelo criada.', run: (template) => notificationTemplatesApi.createVersion(template.id, { subject: template.name, bodyHtml: '<p>Mensagem do PsiFlow</p>', bodyText: 'Mensagem do PsiFlow' }) },
          { label: 'Enviar email de teste', successMessage: 'Email de teste enviado para teste@psiflow.local.', run: (template) => notificationsApi.sendTestEmail({ recipientEmail: 'teste@psiflow.local', templateKey: template.key ?? '' }) },
          { label: 'Agendar lembrete de teste', successMessage: 'Lembrete de teste agendado para teste@psiflow.local.', run: () => notificationsApi.scheduleReminders({ notificationType: 'appointment_reminder', scheduledFor: new Date(Date.now() + 3600000).toISOString(), recipientEmail: 'teste@psiflow.local', recipientUserId: null, payloadJson: '{}' }) },
        ]}
        summaryLabel={(count) => `${count} ${count === 1 ? 'modelo' : 'modelos'}`}
        summaryDescription={(count) =>
          count === 0 ? 'Nenhum modelo de notificacao cadastrado.' : 'Revise canais, status e testes antes de depender de envios automaticos.'
        }
        updatePolicyLabel="Envios exigem conferencia"
        emptyTitle="Nenhum modelo de notificacao"
        emptyDescription="Crie modelos para lembretes e mensagens transacionais. Teste cada canal antes de ativar envios automaticos."
        modalDescription="Defina chave, nome, canal e status do modelo. Use chaves estaveis para evitar envio pelo template errado."
        createSubmitLabel="Criar modelo"
        editSubmitLabel="Salvar modelo"
        detailEditLabel="Editar modelo"
        moreActionsLabel="Versao, teste e agendamento"
        moreActionsHint="As acoes de teste usam teste@psiflow.local. Nao substituem o envio real para pacientes."
      />
      <Section
        title="Logs e reprocessamento"
        description="Acompanhe entregas transacionais e reenvie falhas sem abrir outro sistema."
      >
        <div className="workflow-actions">
          <Button type="button" variant="secondary" onClick={loadLogs}>
            Carregar logs
          </Button>
          <Button type="button" disabled={logs.length > 0 && failedLogs.length === 0} onClick={retryFirstFailed}>
            Reprocessar primeira falha
          </Button>
        </div>
        {logs.length ? (
          <p className="workflow-note">
            {failedLogs.length} {failedLogs.length === 1 ? 'falha encontrada' : 'falhas encontradas'} em {logs.length}{' '}
            logs carregados.
          </p>
        ) : null}
        <div className="workflow-panel workflow-panel--wide">
          {logs.length ? (
            <ul>
              {logs.slice(0, 8).map((log) => (
                <li key={log.id}>
                  <strong>
                    #{log.id} {log.status || 'sem status'}
                  </strong>
                  <span>
                    {log.templateKey || 'template sem chave'} para {log.recipientEmail || 'destinatario nao informado'}
                  </span>
                </li>
              ))}
            </ul>
          ) : (
            <p>Carregue os logs para ver entregas, falhas e reprocessamentos disponiveis.</p>
          )}
        </div>
      </Section>
    </div>
  );
}
