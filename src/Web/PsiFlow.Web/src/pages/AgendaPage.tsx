import { ResourcePage, statusBadge } from '../components/ResourcePage';
import { Button } from '../components/Button';
import { MonthCalendar } from '../components/MonthCalendar';
import { Section } from '../components/Section';
import { useToast } from '../components/Toast';
import { api, isLocalFallbackStatus } from '../services/api';
import type { Appointment, DashboardData, FormField, LookupMap, ScheduleBlock, WeeklyAvailability } from '../types';
import { useState } from 'react';

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
  const { notify } = useToast();
  const [availabilities, setAvailabilities] = useState<WeeklyAvailability[]>([]);
  const [blocks, setBlocks] = useState<ScheduleBlock[]>([]);
  const [slots, setSlots] = useState<Array<{ startsAt: string; endsAt: string; modality: string }>>([]);

  async function loadAgendaSetup() {
    try {
      const [weekly, scheduleBlocks] = await Promise.all([
        api.get<WeeklyAvailability[]>('agenda', '/v1/availability/weekly'),
        api.get<ScheduleBlock[]>('agenda', '/v1/schedule-blocks'),
      ]);
      setAvailabilities(weekly);
      setBlocks(scheduleBlocks);
      notify('Disponibilidade e bloqueios atualizados.');
    } catch (error) {
      if (isLocalFallbackStatus(error)) {
        setAvailabilities([]);
        setBlocks([]);
        notify('Configuracao carregada em modo local.', 'info');
        return;
      }
      notify(error instanceof Error ? error.message : 'Nao foi possivel carregar a agenda.', 'danger');
    }
  }

  async function createDefaultAvailability() {
    try {
      const created = await api.post<WeeklyAvailability>('agenda', '/v1/availability/weekly', { weekday: 1, startTime: '09:00:00', endTime: '17:00:00', slotDurationMinutes: 50, modality: 'online', timezone: 'America/Sao_Paulo', isActive: true });
      setAvailabilities((current) => [created, ...current]);
      notify('Disponibilidade semanal criada.');
    } catch (error) {
      if (isLocalFallbackStatus(error)) {
        setAvailabilities((current) => [{ id: Date.now(), weekday: 1, startTime: '09:00:00', endTime: '17:00:00', slotDurationMinutes: 50, modality: 'online', timezone: 'America/Sao_Paulo', isActive: true }, ...current]);
        notify('Disponibilidade criada localmente.', 'info');
        return;
      }
      notify(error instanceof Error ? error.message : 'Nao foi possivel criar disponibilidade.', 'danger');
    }
  }

  async function createScheduleBlock() {
    const startsAt = new Date(Date.now() + 24 * 60 * 60 * 1000);
    const endsAt = new Date(startsAt.getTime() + 60 * 60 * 1000);
    try {
      const created = await api.post<ScheduleBlock>('agenda', '/v1/schedule-blocks', { startsAt: startsAt.toISOString(), endsAt: endsAt.toISOString(), reason: 'Bloqueio criado pelo workspace web' });
      setBlocks((current) => [created, ...current]);
      notify('Bloqueio pontual criado.');
    } catch (error) {
      if (isLocalFallbackStatus(error)) {
        setBlocks((current) => [{ id: Date.now(), startsAt: startsAt.toISOString(), endsAt: endsAt.toISOString(), reason: 'Bloqueio local' }, ...current]);
        notify('Bloqueio criado localmente.', 'info');
        return;
      }
      notify(error instanceof Error ? error.message : 'Nao foi possivel criar bloqueio.', 'danger');
    }
  }

  async function loadSlots() {
    const from = new Date();
    const to = new Date(Date.now() + 7 * 86400000);
    try {
      const available = await api.get<Array<{ startsAt: string; endsAt: string; modality: string }>>('agenda', `/v1/available-slots?from=${encodeURIComponent(from.toISOString())}&to=${encodeURIComponent(to.toISOString())}`);
      setSlots(available);
      notify('Horarios disponiveis carregados.');
    } catch (error) {
      if (isLocalFallbackStatus(error)) {
        setSlots([]);
        notify('Consulta de slots registrada em modo local.', 'info');
        return;
      }
      notify(error instanceof Error ? error.message : 'Nao foi possivel carregar horarios livres.', 'danger');
    }
  }

  async function createAppointmentForDay(date: Date) {
    const startsAt = new Date(date.getFullYear(), date.getMonth(), date.getDate(), 9, 0);
    const endsAt = new Date(startsAt.getTime() + 50 * 60 * 1000);
    const firstPatient = data.patients[0];
    const fallback: Appointment = {
      id: Date.now(),
      tenantId: 1,
      name: firstPatient ? `Consulta ${firstPatient.fullName || firstPatient.name}` : 'Consulta sem paciente',
      patientId: firstPatient?.id ?? 0,
      psychologistId: 1,
      startsAt: startsAt.toISOString(),
      endsAt: endsAt.toISOString(),
      patientName: firstPatient?.fullName || firstPatient?.name || 'Paciente a definir',
      time: startsAt.toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' }),
      kind: 'Online',
      modality: 'online',
      status: 'Pendente',
      createdBy: 1,
    };
    try {
      const created = await api.post<Appointment>('agenda', '/v1/appointments', { tenantId: 1, name: fallback.name, patientId: fallback.patientId, psychologistId: fallback.psychologistId, startsAt: fallback.startsAt, endsAt: fallback.endsAt, modality: 'online', status: 'scheduled', createdBy: 1 });
      onAppointmentsChange([created, ...data.appointments]);
      notify('Compromisso criado para o dia selecionado.');
    } catch (error) {
      if (isLocalFallbackStatus(error)) {
        onAppointmentsChange([fallback, ...data.appointments]);
        notify('Compromisso criado localmente para o dia selecionado.', 'info');
        return;
      }
      notify(error instanceof Error ? error.message : 'Nao foi possivel criar compromisso.', 'danger');
    }
  }

  return (
    <div className="resource-layout">
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
          { label: 'Cancelar consulta', tone: 'danger', successMessage: 'Consulta cancelada.', run: (appointment) => api.post('agenda', `/v1/appointments/${appointment.id}/cancel`, { reason: 'Cancelada pelo workspace web' }) },
          { label: 'Ver horarios livres', successMessage: 'Horarios disponiveis carregados.', run: loadSlots },
        ]}
      />
      <Section title="Calendario mensal" description="Cada dia mostra quantos compromissos existem. Use para se orientar antes de abrir detalhes da agenda.">
        <MonthCalendar appointments={data.appointments} onCreateForDay={createAppointmentForDay} />
      </Section>
      <Section title="Disponibilidade e bloqueios" description="Configure janelas semanais, revise bloqueios e consulte slots sem sair da agenda.">
        <div className="workflow-actions">
          <Button type="button" variant="secondary" onClick={loadAgendaSetup}>Carregar configuracao</Button>
          <Button type="button" variant="secondary" onClick={createDefaultAvailability}>Criar disponibilidade padrao</Button>
          <Button type="button" variant="secondary" onClick={createScheduleBlock}>Criar bloqueio de teste</Button>
          <Button type="button" onClick={loadSlots}>Consultar slots</Button>
        </div>
        <div className="workflow-grid">
          <WorkflowList title="Disponibilidade semanal" empty="Nenhuma janela carregada." items={availabilities.map((item) => `${weekdayLabel(item.weekday)} ${item.startTime} ate ${item.endTime}, ${item.slotDurationMinutes} min, ${item.modality}`)} />
          <WorkflowList title="Bloqueios" empty="Nenhum bloqueio carregado." items={blocks.map((item) => `${formatDateTime(item.startsAt)} ate ${formatDateTime(item.endsAt)}${item.reason ? `, ${item.reason}` : ''}`)} />
          <WorkflowList title="Slots disponiveis" empty="Consulte slots para os proximos 7 dias." items={slots.slice(0, 6).map((item) => `${formatDateTime(item.startsAt)} ate ${formatDateTime(item.endsAt)}, ${item.modality}`)} />
        </div>
      </Section>
    </div>
  );
}

function WorkflowList({ title, empty, items }: { title: string; empty: string; items: string[] }) {
  return <div className="workflow-panel"><h3>{title}</h3>{items.length ? <ul>{items.map((item) => <li key={item}>{item}</li>)}</ul> : <p>{empty}</p>}</div>;
}

function weekdayLabel(value: number) {
  return ['Domingo', 'Segunda', 'Terca', 'Quarta', 'Quinta', 'Sexta', 'Sabado'][value] ?? `Dia ${value}`;
}

function formatDateTime(value?: string) {
  if (!value) return 'Sem horario';
  return new Date(value).toLocaleString('pt-BR', { day: '2-digit', month: '2-digit', hour: '2-digit', minute: '2-digit' });
}
