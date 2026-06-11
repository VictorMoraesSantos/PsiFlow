import { ResourcePage, statusBadge } from '../components/ResourcePage';
import { api } from '../services/api';
import type { DashboardData, FormField, LookupMap, VideoRoom } from '../types';

const fields: Array<FormField<VideoRoom>> = [
  { name: 'sessionId', label: 'Sessao', type: 'lookup', lookupKey: 'sessions', required: true, helperText: 'Sessao que abrira a sala online.' },
  { name: 'name', label: 'Nome da sala', type: 'text', required: true, placeholder: 'Ex: Sala Marina 14:30' },
  { name: 'provider', label: 'Provedor', type: 'select', required: true, options: [{ label: 'Externo', value: 'external' }, { label: 'Google Meet', value: 'google_meet' }, { label: 'Zoom', value: 'zoom' }] },
  { name: 'urlEncrypted', label: 'URL externa HTTPS', type: 'text', helperText: 'Cole o link que sera enviado ao endpoint seguro da sessao.' },
  { name: 'urlHash', label: 'Hash da URL', type: 'text' },
  { name: 'instructions', label: 'Instrucoes', type: 'textarea' },
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
  return (
    <ResourcePage
      title="Atendimento online"
      description="Salas por sessao, instrucoes de acesso e rastreio de clique."
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
        { key: 'name', header: 'Sala', render: (room) => <div className="person-cell"><strong>{room.name}</strong><span>Sessao #{room.sessionId}</span></div> },
        { key: 'provider', header: 'Provedor', render: (room) => room.provider },
        { key: 'instructions', header: 'Instrucoes', render: (room) => room.instructions || 'Sem instrucoes' },
        { key: 'status', header: 'Status', render: (room) => statusBadge(room.status) },
      ]}
      detailFields={[
        { label: 'Sessao', value: (room) => room.sessionId ? `Sessao #${room.sessionId}` : 'Nao vinculada' },
        { label: 'Provedor', value: (room) => room.provider },
        { label: 'Hash', value: (room) => room.urlHash || 'Nao informado' },
        { label: 'Instrucoes', value: (room) => room.instructions || 'Nao informado' },
        { label: 'Status', value: (room) => statusBadge(room.status) },
      ]}
      actions={[
        { label: 'Salvar link da sessao', successMessage: 'Link da sessao salvo.', run: (room) => api.put('onlineSession', `/v1/sessions/${room.sessionId}/video-room`, { name: room.name, provider: room.provider || 'external', url: room.urlEncrypted || room.urlHash || 'https://meet.google.com/psiflow-demo', instructions: room.instructions || null }) },
        { label: 'Consultar link', successMessage: 'Link da sessao carregado.', run: (room) => api.get('onlineSession', `/v1/sessions/${room.sessionId}/video-room`) },
        { label: 'Registrar clique', successMessage: 'Clique registrado ou simulado.', run: (room) => api.post('onlineSession', `/v1/sessions/${room.sessionId}/video-room/click`) },
        { label: 'Ver cliques', successMessage: 'Historico carregado ou simulado.', run: (room) => api.get('onlineSession', `/v1/sessions/${room.sessionId}/video-room/clicks`) },
        { label: 'Definir link padrao', successMessage: 'Link padrao salvo.', run: (room) => api.put('onlineSession', '/v1/video-settings/default-link', { provider: room.provider, url: room.urlEncrypted || room.urlHash || 'https://meet.google.com/psiflow-demo' }) },
        { label: 'Ver link padrao', successMessage: 'Link padrao carregado.', run: () => api.get('onlineSession', '/v1/video-settings/default-link') },
      ]}
    />
  );
}
