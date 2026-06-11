import { ResourcePage, statusBadge } from '../components/ResourcePage';
import { api } from '../services/api';
import type { DashboardData, FormField, LookupMap, VideoRoom } from '../types';

const fields: Array<FormField<VideoRoom>> = [
  { name: 'sessionId', label: 'Sessao', type: 'lookup', lookupKey: 'sessions', required: true, helperText: 'Sessao que abrira a sala online.' },
  { name: 'name', label: 'Nome da sala', type: 'text', required: true, placeholder: 'Ex: Sala Marina 14:30' },
  { name: 'provider', label: 'Provedor', type: 'select', required: true, options: [{ label: 'Externo', value: 'external' }, { label: 'Google Meet', value: 'google_meet' }, { label: 'Zoom', value: 'zoom' }] },
  { name: 'urlEncrypted', label: 'Link HTTPS da sala', type: 'text', helperText: 'Cole apenas links HTTPS. O hash nao substitui o link de acesso.' },
  { name: 'urlHash', label: 'Hash registrado', type: 'text', helperText: 'Usado para conferencia tecnica. Nao envie hash como link da sessao.' },
  { name: 'instructions', label: 'Instrucoes de acesso', type: 'textarea' },
  { name: 'createdBy', label: 'Criado por', type: 'lookup', lookupKey: 'psychologists' },
  { name: 'status', label: 'Status', type: 'select', required: true, options: [{ label: 'Ativo', value: 'Ativo' }, { label: 'Pausado', value: 'Pausado' }, { label: 'Inativo', value: 'Inativo' }] },
];

const emptyRoom: VideoRoom = { id: 0, tenantId: 1, sessionId: 0, name: '', provider: 'external', urlEncrypted: '', urlHash: '', instructions: '', createdBy: 1, status: 'Ativo' };

function buildLookups(data: DashboardData): LookupMap {
  return {
    sessions: data.sessions.map((session) => ({ label: `${session.startsAt?.slice(11, 16) || '--:--'} · ${session.patientName || session.name || `Sessao ${session.id}`}`, value: session.id })),
    psychologists: data.sessions
      .map((session) => session.psychologistId)
      .filter((value): value is number => typeof value === 'number')
      .filter((value, index, list) => list.indexOf(value) === index)
      .map((id) => ({ label: `Psicologo #${id}`, value: id })),
  };
}

type OnlineSessionPageProps = { data: DashboardData; onVideoRoomsChange: (rooms: VideoRoom[]) => void };

export function OnlineSessionPage({ data, onVideoRoomsChange }: OnlineSessionPageProps) {
  function requireHttpsUrl(room: VideoRoom) {
    const url = room.urlEncrypted?.trim();
    if (!url || !url.startsWith('https://')) {
      throw new Error('Informe um link HTTPS da sala antes de salvar o acesso online.');
    }
    return url;
  }

  return (
    <ResourcePage
      title="Atendimento online"
      description="Links seguros por sessao, instrucoes de acesso e historico de cliques."
      createLabel="Criar sala"
      items={data.videoRooms}
      service="onlineSession"
      path="/v1/video-rooms"
      fields={fields}
      lookups={buildLookups(data)}
      emptyValue={emptyRoom}
      getId={(room) => room.id}
      getTitle={(room) => room.name}
      onItemsChange={onVideoRoomsChange}
      toCreatePayload={(room) => ({ tenantId: room.tenantId, sessionId: room.sessionId, name: room.name, provider: room.provider || 'external', urlEncrypted: room.urlEncrypted || '', urlHash: room.urlHash || '', instructions: room.instructions || null, createdBy: room.createdBy || 1, status: room.status === 'Inativo' ? 'inactive' : room.status === 'Pausado' ? 'paused' : 'active' })}
      toUpdatePayload={(room) => ({ id: room.id, tenantId: room.tenantId, sessionId: room.sessionId, name: room.name, provider: room.provider || 'external', urlEncrypted: room.urlEncrypted || '', urlHash: room.urlHash || '', instructions: room.instructions || null, createdBy: room.createdBy || 1, status: room.status === 'Inativo' ? 'inactive' : room.status === 'Pausado' ? 'paused' : 'active' })}
      columns={[
        { key: 'name', header: 'Sessao e sala', render: (room) => <div className="person-cell"><strong>{sessionLabel(data, room.sessionId)}</strong><span>{room.name || `Sala #${room.id}`}</span></div> },
        { key: 'provider', header: 'Provedor', render: (room) => room.provider },
        { key: 'readiness', header: 'Prontidao', render: (room) => readinessLabel(room) },
        { key: 'status', header: 'Status', render: (room) => statusBadge(room.status) },
      ]}
      detailFields={[
        { label: 'Sessao', value: (room) => sessionLabel(data, room.sessionId) },
        { label: 'Provedor', value: (room) => room.provider },
        { label: 'Link seguro', value: (room) => room.urlEncrypted?.startsWith('https://') ? 'HTTPS informado' : 'Link HTTPS ausente' },
        { label: 'Hash', value: (room) => room.urlHash || 'Nao informado' },
        { label: 'Instrucoes', value: (room) => room.instructions || 'Nao informado' },
        { label: 'Status', value: (room) => statusBadge(room.status) },
      ]}
      actions={[
        { label: 'Salvar link desta sessao', successMessage: 'Link seguro da sessao salvo.', run: (room) => api.put('onlineSession', `/v1/sessions/${room.sessionId}/video-room`, { name: room.name, provider: room.provider || 'external', url: requireHttpsUrl(room), instructions: room.instructions || null }) },
        { label: 'Consultar link', successMessage: 'Link da sessao carregado.', run: (room) => api.get('onlineSession', `/v1/sessions/${room.sessionId}/video-room`) },
        { label: 'Ver historico de cliques', successMessage: 'Historico de cliques carregado.', run: (room) => api.get('onlineSession', `/v1/sessions/${room.sessionId}/video-room/clicks`) },
        { label: 'Definir link padrao do workspace', successMessage: 'Link padrao do workspace salvo.', run: (room) => api.put('onlineSession', '/v1/video-settings/default-link', { provider: room.provider, url: requireHttpsUrl(room) }) },
        { label: 'Ver link padrao', successMessage: 'Link padrao carregado.', run: () => api.get('onlineSession', '/v1/video-settings/default-link') },
      ]}
      summaryLabel={(count) => `${count} ${count === 1 ? 'sala' : 'salas'}`}
      summaryDescription={(count) => count === 0 ? 'Nenhuma sala online cadastrada.' : 'Confira link HTTPS, sessao vinculada e historico antes do atendimento.'}
      updatePolicyLabel="Link seguro por sessao"
      emptyTitle="Nenhuma sala online cadastrada"
      emptyDescription="Crie uma sala para uma sessao online e salve um link HTTPS antes de enviar acesso ao paciente."
      modalDescription="Vincule a sala a uma sessao e informe um link HTTPS. O link padrao do workspace tem escopo maior que o link desta sessao."
      createSubmitLabel="Criar sala"
      editSubmitLabel="Salvar sala"
      detailEditLabel="Editar sala"
      moreActionsLabel="Link, historico e padrao"
      moreActionsHint="Salvar link desta sessao afeta apenas a sessao selecionada. Link padrao vale para o workspace."
    />
  );
}

function sessionLabel(data: DashboardData, sessionId?: number) {
  const session = data.sessions.find((item) => item.id === sessionId);
  if (!session) return sessionId ? `Sessao #${sessionId}` : 'Sessao nao vinculada';
  const time = session.startsAt?.slice(11, 16) || '--:--';
  return `${time} · ${session.patientName || session.name || `Sessao ${session.id}`}`;
}

function readinessLabel(room: VideoRoom) {
  if (room.status === 'Inativo') return 'Inativa';
  if (room.status === 'Pausado') return 'Pausada';
  if (!room.sessionId) return 'Sem sessao';
  if (!room.urlEncrypted?.startsWith('https://')) return 'Link HTTPS ausente';
  return 'Pronta para acesso';
}
