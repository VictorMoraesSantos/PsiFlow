import { ResourcePage, statusBadge } from '../components/ResourcePage';
import { api } from '../services/api';
import type { DashboardData, FormField, LookupMap, Session } from '../types';

const fields: Array<FormField<Session>> = [
  { name: 'name', label: 'Nome da sessao', type: 'text', required: true, placeholder: 'Ex: Sessao clinica Marina' },
  { name: 'appointmentId', label: 'Consulta', type: 'lookup', lookupKey: 'appointments', required: true, helperText: 'Consulta que originou esta sessao.' },
  { name: 'patientId', label: 'Paciente', type: 'lookup', lookupKey: 'patients', required: true },
  { name: 'psychologistId', label: 'Psicologo', type: 'lookup', lookupKey: 'psychologists', required: true },
  { name: 'startsAt', label: 'Inicio', type: 'datetime-local', required: true },
  { name: 'endsAt', label: 'Fim', type: 'datetime-local', required: true },
  { name: 'status', label: 'Status', type: 'select', required: true, options: [{ label: 'Preparar', value: 'Preparar' }, { label: 'Em andamento', value: 'Em andamento' }, { label: 'Finalizada', value: 'Finalizada' }] },
  { name: 'modality', label: 'Modalidade', type: 'select', options: [{ label: 'Online', value: 'online' }, { label: 'Presencial', value: 'in_person' }] },
];

const emptySession: Session = { id: 0, tenantId: 1, name: '', appointmentId: 0, patientId: 0, psychologistId: 0, startsAt: '', endsAt: '', patientName: '', status: 'Preparar', modality: 'online', payment: 'Pendente', room: 'Sala online' };

function buildLookups(data: DashboardData): LookupMap {
  return {
    patients: data.patients.map((patient) => ({ label: patient.fullName ?? patient.name, value: patient.id })),
    appointments: data.appointments.map((appointment) => ({ label: `${appointment.time || '--:--'} · ${appointment.patientName || appointment.name || `Consulta ${appointment.id}`}`, value: appointment.id })),
    psychologists: data.sessions
      .map((session) => session.psychologistId)
      .filter((value): value is number => typeof value === 'number')
      .filter((value, index, list) => list.indexOf(value) === index)
      .map((id) => ({ label: `Psicologo #${id}`, value: id })),
  };
}

type SessionsPageProps = { data: DashboardData; onSessionsChange: (sessions: Session[]) => void };

export function SessionsPage({ data, onSessionsChange }: SessionsPageProps) {
  return (
    <ResourcePage
      title="Sessoes"
      description="Preparacao, status clinico, pagamento e sala online."
      createLabel="Criar sessao"
      items={data.sessions}
      service="sessions"
      path="/v1/sessions"
      fields={fields}
      lookups={buildLookups(data)}
      emptyValue={emptySession}
      getId={(session) => session.id}
      getTitle={(session) => session.name || session.patientName || `Sessao ${session.id}`}
      onItemsChange={onSessionsChange}
      toCreatePayload={(session) => ({ tenantId: session.tenantId, name: session.name || session.patientName, appointmentId: session.appointmentId, patientId: session.patientId, psychologistId: session.psychologistId, startsAt: session.startsAt, endsAt: session.endsAt, status: session.status === 'Finalizada' ? 'completed' : session.status === 'Em andamento' ? 'started' : 'scheduled', modality: session.modality || 'online' })}
      toUpdatePayload={(session) => ({ id: session.id, tenantId: session.tenantId, name: session.name || session.patientName, appointmentId: session.appointmentId, patientId: session.patientId, psychologistId: session.psychologistId, startsAt: session.startsAt, endsAt: session.endsAt, status: session.status === 'Finalizada' ? 'completed' : session.status === 'Em andamento' ? 'started' : 'scheduled', modality: session.modality || 'online' })}
      columns={[
        { key: 'patientName', header: 'Sessao', render: (session) => <strong>{session.name || session.patientName}</strong> },
        { key: 'room', header: 'Local', render: (session) => session.room },
        { key: 'payment', header: 'Pagamento', render: (session) => statusBadge(session.payment) },
        { key: 'status', header: 'Status', render: (session) => statusBadge(session.status) },
      ]}
      detailFields={[
        { label: 'Consulta', value: (session) => session.appointmentId ? `Consulta #${session.appointmentId}` : 'Nao vinculada' },
        { label: 'Paciente', value: (session) => session.patientName || `Paciente #${session.patientId}` },
        { label: 'Inicio', value: (session) => session.startsAt || 'Nao informado' },
        { label: 'Fim', value: (session) => session.endsAt || 'Nao informado' },
        { label: 'Pagamento', value: (session) => statusBadge(session.payment) },
        { label: 'Status', value: (session) => statusBadge(session.status) },
      ]}
      actions={[
        { label: 'Iniciar sessao', successMessage: 'Sessao iniciada ou acao simulada.', run: (session) => api.post('sessions', `/v1/sessions/${session.id}/start`, { reason: 'Iniciada pelo workspace web' }) },
        { label: 'Marcar pagamento', successMessage: 'Pagamento marcado ou acao simulada.', run: (session) => api.post('sessions', `/v1/sessions/${session.id}/payment/mark-received`, { amount: 0, method: 'manual', receivedAt: new Date().toISOString() }) },
        { label: 'Cancelar sessao', tone: 'danger', successMessage: 'Sessao cancelada ou acao simulada.', run: (session) => api.post('sessions', `/v1/sessions/${session.id}/cancel`, { reason: 'Cancelada pelo workspace web' }) },
      ]}
    />
  );
}
