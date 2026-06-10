import { ResourcePage, statusBadge } from '../components/ResourcePage';
import { api } from '../services/api';
import type { Appointment, DashboardData, FormField, LookupMap } from '../types';

const fields: Array<FormField<Appointment>> = [
  { name: 'name', label: 'Nome da consulta', type: 'text', required: true, placeholder: 'Ex: Sessao de retorno' },
  { name: 'patientId', label: 'Paciente', type: 'lookup', lookupKey: 'patients', required: true, helperText: 'Selecione o paciente atendido nesta consulta.' },
  { name: 'psychologistId', label: 'Psicologo', type: 'lookup', lookupKey: 'psychologists', required: true, helperText: 'Profissional responsavel pela consulta.' },
  { name: 'startsAt', label: 'Inicio', type: 'datetime-local', required: true },
  { name: 'endsAt', label: 'Fim', type: 'datetime-local', required: true },
  { name: 'modality', label: 'Modalidade', type: 'select', required: true, options: [{ label: 'Online', value: 'online' }, { label: 'Presencial', value: 'in_person' }] },
  { name: 'status', label: 'Status', type: 'select', required: true, options: [{ label: 'Confirmada', value: 'Confirmada' }, { label: 'Pendente', value: 'Pendente' }, { label: 'Cancelada', value: 'Cancelada' }] },
  { name: 'createdBy', label: 'Criado por', type: 'lookup', lookupKey: 'psychologists' },
];

const emptyAppointment: Appointment = { id: 0, tenantId: 1, name: '', patientId: 0, psychologistId: 0, startsAt: '', endsAt: '', patientName: '', time: '', kind: 'Online', modality: 'online', status: 'Confirmada', createdBy: 1 };

function buildLookups(data: DashboardData): LookupMap {
  return {
    patients: data.patients.map((patient) => ({ label: patient.fullName ?? patient.name, value: patient.id })),
    psychologists: data.sessions
      .map((session) => session.psychologistId)
      .filter((value): value is number => typeof value === 'number')
      .filter((value, index, list) => list.indexOf(value) === index)
      .map((id) => ({ label: `Psicologo #${id}`, value: id })),
  };
}

type AgendaPageProps = { data: DashboardData; onAppointmentsChange: (appointments: Appointment[]) => void };

export function AgendaPage({ data, onAppointmentsChange }: AgendaPageProps) {
  return (
    <ResourcePage
      title="Agenda"
      description="Consultas, modalidade, disponibilidade e cancelamentos."
      createLabel="Agendar consulta"
      items={data.appointments}
      service="agenda"
      path="/v1/appointments"
      fields={fields}
      lookups={buildLookups(data)}
      emptyValue={emptyAppointment}
      getId={(appointment) => appointment.id}
      getTitle={(appointment) => appointment.name || appointment.patientName || `Consulta ${appointment.id}`}
      onItemsChange={onAppointmentsChange}
      toCreatePayload={(appointment) => ({ tenantId: appointment.tenantId, name: appointment.name || appointment.patientName, patientId: appointment.patientId, psychologistId: appointment.psychologistId, startsAt: appointment.startsAt, endsAt: appointment.endsAt, modality: appointment.modality || 'online', status: appointment.status === 'Cancelada' ? 'canceled' : 'scheduled', createdBy: appointment.createdBy || 1 })}
      toUpdatePayload={(appointment) => ({ id: appointment.id, tenantId: appointment.tenantId, name: appointment.name || appointment.patientName, patientId: appointment.patientId, psychologistId: appointment.psychologistId, startsAt: appointment.startsAt, endsAt: appointment.endsAt, modality: appointment.modality || 'online', status: appointment.status === 'Cancelada' ? 'canceled' : 'scheduled', lateCancel: false, canceledAt: null, canceledBy: null, cancelReason: null })}
      columns={[
        { key: 'time', header: 'Horario', render: (appointment) => <strong>{appointment.time || appointment.startsAt || '--'}</strong> },
        { key: 'patientName', header: 'Consulta', render: (appointment) => appointment.name || appointment.patientName || 'Sem nome' },
        { key: 'kind', header: 'Formato', render: (appointment) => appointment.kind },
        { key: 'status', header: 'Status', render: (appointment) => statusBadge(appointment.status) },
      ]}
      detailFields={[
        { label: 'Paciente', value: (appointment) => appointment.patientName || `Paciente #${appointment.patientId}` },
        { label: 'Psicologo', value: (appointment) => appointment.psychologistId ? `Psicologo #${appointment.psychologistId}` : 'Nao informado' },
        { label: 'Inicio', value: (appointment) => appointment.startsAt || 'Nao informado' },
        { label: 'Fim', value: (appointment) => appointment.endsAt || 'Nao informado' },
        { label: 'Modalidade', value: (appointment) => appointment.modality || appointment.kind },
        { label: 'Status', value: (appointment) => statusBadge(appointment.status) },
      ]}
      actions={[
        { label: 'Cancelar consulta', tone: 'danger', successMessage: 'Consulta cancelada ou acao simulada.', run: (appointment) => api.post('agenda', `/v1/appointments/${appointment.id}/cancel`, { reason: 'Cancelada pelo workspace web' }) },
        { label: 'Ver horarios livres', successMessage: 'Consulta de horarios executada ou simulada.', run: () => api.get('agenda', `/v1/available-slots?from=${encodeURIComponent(new Date().toISOString())}&to=${encodeURIComponent(new Date(Date.now() + 7 * 86400000).toISOString())}`) },
      ]}
    />
  );
}
