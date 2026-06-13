'use client';

import { ResourcePage, statusBadge, type ResourceCrud } from '../components/ResourcePage';
import { sessionsApi } from '../services/sessions';
import { useApp } from '../state/AppContext';
import type { DashboardData, FormField, LookupMap, Session } from '../types';

const fields: Array<FormField<Session>> = [
  { name: 'name', label: 'Nome da sessao', type: 'text', required: true, placeholder: 'Ex: Retorno clinico Marina' },
  { name: 'appointmentId', label: 'Consulta', type: 'lookup', lookupKey: 'appointments', required: true, helperText: 'Consulta que originou esta sessao.' },
  { name: 'patientId', label: 'Paciente', type: 'lookup', lookupKey: 'patients', required: true },
  { name: 'psychologistId', label: 'Psicologo', type: 'lookup', lookupKey: 'psychologists', required: true },
  { name: 'startsAt', label: 'Inicio', type: 'datetime-local', required: true },
  { name: 'endsAt', label: 'Fim', type: 'datetime-local', required: true },
  { name: 'status', label: 'Status', type: 'select', required: true, options: [{ label: 'Preparar', value: 'Preparar' }, { label: 'Em andamento', value: 'Em andamento' }, { label: 'Finalizada', value: 'Finalizada' }] },
  { name: 'modality', label: 'Modalidade', type: 'select', options: [{ label: 'Online', value: 'online' }, { label: 'Presencial', value: 'in_person' }] },
];

const emptySession: Session = {
  id: 0,
  tenantId: 1,
  name: '',
  appointmentId: 0,
  patientId: 0,
  psychologistId: 0,
  startsAt: '',
  endsAt: '',
  patientName: '',
  status: 'Preparar',
  modality: 'online',
  payment: 'Pendente',
  room: 'Sala online',
};

function buildLookups(data: DashboardData): LookupMap {
  return {
    patients: data.patients.map((patient) => ({ label: patient.fullName ?? patient.name, value: patient.id })),
    appointments: data.appointments.map((appointment) => ({
      label: `${appointment.time || '--:--'} · ${appointment.patientName || appointment.name || `Consulta ${appointment.id}`}`,
      value: appointment.id,
    })),
    psychologists: data.sessions
      .map((session) => session.psychologistId)
      .filter((value): value is number => typeof value === 'number')
      .filter((value, index, list) => list.indexOf(value) === index)
      .map((id) => ({ label: `Psicologo #${id}`, value: id })),
  };
}

const sessionsCrud: ResourceCrud<Session> = {
  create: (session) => sessionsApi.create(toPayload(session)),
  update: (id, session) => sessionsApi.update(Number(id), toPayload(session)),
  remove: (id) => sessionsApi.cancel(Number(id), { reason: 'Removida pelo workspace web' }).then(() => undefined),
};

function toPayload(session: Session): Session {
  return {
    ...session,
    name: session.name || session.patientName,
    modality: session.modality || 'online',
    status: session.status,
  };
}

export function SessionsPage() {
  const { data, setData } = useApp();
  const onSessionsChange = (sessions: Session[]) => setData((current) => ({ ...current, sessions }));

  return (
    <ResourcePage
      title="Sessoes"
      description="Acompanhe preparo, atendimento, pagamento e encerramento de cada sessao."
      createLabel="Criar sessao"
      items={data.sessions}
      crud={sessionsCrud}
      fields={fields}
      lookups={buildLookups(data)}
      emptyValue={emptySession}
      getId={(session) => session.id}
      getTitle={(session) => session.name || session.patientName || `Sessao ${session.id}`}
      onItemsChange={onSessionsChange}
      toCreatePayload={toPayload}
      toUpdatePayload={toPayload}
      columns={[
        {
          key: 'patientName',
          header: 'Paciente e horario',
          render: (session) => (
            <div className="person-cell">
              <strong>{session.patientName || session.name || `Sessao ${session.id}`}</strong>
              <span>{formatSessionRange(session)}</span>
            </div>
          ),
        },
        { key: 'room', header: 'Local', render: (session) => session.room },
        { key: 'nextStep', header: 'Proximo passo', render: (session) => nextStepLabel(session) },
        { key: 'payment', header: 'Pagamento', render: (session) => statusBadge(session.payment) },
        { key: 'status', header: 'Status', render: (session) => statusBadge(session.status) },
      ]}
      detailFields={[
        { label: 'Consulta', value: (session) => (session.appointmentId ? `Consulta #${session.appointmentId}` : 'Nao vinculada') },
        { label: 'Paciente', value: (session) => session.patientName || `Paciente #${session.patientId}` },
        { label: 'Inicio', value: (session) => formatDateTime(session.startsAt) },
        { label: 'Fim', value: (session) => formatDateTime(session.endsAt) },
        { label: 'Pagamento', value: (session) => statusBadge(session.payment) },
        { label: 'Status', value: (session) => statusBadge(session.status) },
      ]}
      actions={[
        { label: 'Iniciar sessao', successMessage: 'Sessao iniciada.', run: (session) => sessionsApi.start(session.id, { reason: 'Iniciada pelo workspace web' }) },
        { label: 'Concluir sessao', successMessage: 'Sessao concluida.', run: (session) => sessionsApi.complete(session.id, { reason: 'Concluida pelo workspace web' }) },
        { label: 'Marcar falta', successMessage: 'Falta registrada.', run: (session) => sessionsApi.noShow(session.id, { reason: 'Ausencia registrada pelo workspace web' }) },
        { label: 'Marcar pagamento', successMessage: 'Pagamento marcado.', run: (session) => sessionsApi.markPaymentReceived(session.id, { amountCents: 0, currency: 'BRL', notes: 'Pagamento manual registrado pelo workspace web' }) },
        { label: 'Ver pagamento', successMessage: 'Pagamento carregado.', run: (session) => sessionsApi.payment(session.id) },
        { label: 'Enviar recibo', successMessage: 'Recibo solicitado.', run: (session) => sessionsApi.sendReceipt(session.id) },
        { label: 'Cancelar sessao', tone: 'danger', successMessage: 'Sessao cancelada.', run: (session) => sessionsApi.cancel(session.id, { reason: 'Cancelada pelo workspace web' }) },
      ]}
      summaryLabel={(count) => `${count} ${count === 1 ? 'sessao' : 'sessoes'}`}
      summaryDescription={(count) =>
        count === 0 ? 'Nenhuma sessao para preparar agora.' : 'Use a lista para retomar contexto, iniciar atendimento e fechar pendencias.'
      }
      updatePolicyLabel="Atualiza apos salvar"
      emptyTitle="Nenhuma sessao cadastrada"
      emptyDescription="Crie uma sessao a partir de uma consulta para acompanhar preparo, atendimento e fechamento clinico."
      modalDescription="Vincule a sessao a uma consulta e confira paciente, profissional e horario antes de salvar."
      createSubmitLabel="Criar sessao"
      editSubmitLabel="Salvar sessao"
      detailEditLabel="Editar sessao"
      moreActionsLabel="Acoes clinicas e administrativas"
      moreActionsHint="Use estas acoes para encerramento, falta, pagamento, recibo ou cancelamento."
    />
  );
}

function formatSessionRange(session: Session) {
  const start = formatTime(session.startsAt);
  const end = formatTime(session.endsAt);
  if (start === 'Sem horario' && end === 'Sem horario') return 'Horario nao informado';
  if (end === 'Sem horario') return start;
  return `${start} ate ${end}`;
}

function formatDateTime(value?: string) {
  if (!value) return 'Nao informado';
  return new Date(value).toLocaleString('pt-BR', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  });
}

function formatTime(value?: string) {
  if (!value) return 'Sem horario';
  return new Date(value).toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' });
}

function nextStepLabel(session: Session) {
  const status = session.status.toLowerCase();
  if (status.includes('final')) return 'Revisar fechamento';
  if (status.includes('andamento')) return 'Concluir sessao';
  if (status.includes('cancel')) return 'Revisar cancelamento';
  return 'Iniciar sessao';
}
