'use client';

import { useState } from 'react';
import { Button } from '../components/Button';
import { MonthCalendar } from '../components/MonthCalendar';
import { ResourcePage, statusBadge, type ResourceCrud } from '../components/ResourcePage';
import { Section } from '../components/Section';
import { useToast } from '../components/Toast';
import { appointmentsApi, availabilityApi } from '../services/agenda';
import { isLocalFallbackStatus } from '../services/http';
import { useApp } from '../state/AppContext';
import type {
  Appointment,
  DashboardData,
  FormField,
  LookupMap,
  ScheduleBlock,
  WeeklyAvailability,
} from '../types';

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

const emptyAppointment: Appointment = {
  id: 0,
  tenantId: 1,
  name: '',
  patientId: 0,
  psychologistId: 0,
  startsAt: '',
  endsAt: '',
  patientName: '',
  time: '',
  kind: 'Online',
  modality: 'online',
  status: 'Confirmada',
  createdBy: 1,
};

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

const appointmentsCrud: ResourceCrud<Appointment> = {
  create: (appointment) => appointmentsApi.create(toCreatePayload(appointment)),
  update: (id, appointment) => appointmentsApi.update(Number(id), toUpdatePayload(appointment)),
  remove: (id) => appointmentsApi.cancel(Number(id), { reason: 'Removida pelo workspace web' }).then(() => undefined),
};

function toCreatePayload(appointment: Appointment): Appointment {
  return {
    ...appointment,
    name: appointment.name || appointment.patientName,
    modality: appointment.modality || 'online',
    status: appointment.status,
  };
}

function toUpdatePayload(appointment: Appointment): Appointment {
  return {
    ...appointment,
    name: appointment.name || appointment.patientName,
    modality: appointment.modality || 'online',
    status: appointment.status,
  };
}

export function AgendaPage() {
  const { data, setData } = useApp();
  const onAppointmentsChange = (appointments: Appointment[]) =>
    setData((current) => ({ ...current, appointments }));
  const { notify } = useToast();
  const [availabilities, setAvailabilities] = useState<WeeklyAvailability[]>([]);
  const [blocks, setBlocks] = useState<ScheduleBlock[]>([]);
  const [slots, setSlots] = useState<Array<{ startsAt: string; endsAt: string; modality: string }>>([]);
  const [pendingAction, setPendingAction] = useState<string | null>(null);
  const [slotsRange, setSlotsRange] = useState<string | null>(null);

  async function loadAgendaSetup() {
    setPendingAction('load-setup');
    try {
      const [weekly, scheduleBlocks] = await Promise.all([
        availabilityApi.weekly(),
        availabilityApi.scheduleBlocks(),
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
    } finally {
      setPendingAction(null);
    }
  }

  async function loadSlots() {
    const from = new Date();
    const to = new Date(Date.now() + 7 * 86400000);
    setPendingAction('load-slots');
    setSlotsRange(`${formatDateTime(from.toISOString())} ate ${formatDateTime(to.toISOString())}`);
    try {
      const available = await availabilityApi.availableSlots(from.toISOString(), to.toISOString());
      setSlots(available);
      notify('Horarios disponiveis carregados.');
    } catch (error) {
      if (isLocalFallbackStatus(error)) {
        setSlots([]);
        notify('Consulta de slots registrada em modo local.', 'info');
        return;
      }
      notify(error instanceof Error ? error.message : 'Nao foi possivel carregar horarios livres.', 'danger');
    } finally {
      setPendingAction(null);
    }
  }

  async function createAppointmentForDay(date: Date) {
    notify(
      `Use "Agendar consulta" para escolher paciente, profissional e horario em ${date.toLocaleDateString('pt-BR')}.`,
      'info',
    );
  }

  return (
    <div className="resource-layout">
      <Section
        title="Calendario mensal"
        description="Cada dia mostra quantos compromissos existem. Use para se orientar antes de abrir detalhes da agenda."
      >
        <MonthCalendar appointments={data.appointments} onCreateForDay={createAppointmentForDay} />
      </Section>
      <ResourcePage
        title="Consultas agendadas"
        description="Lista operacional para revisar, editar e cancelar consultas ja cadastradas."
        createLabel="Agendar consulta"
        items={data.appointments}
        crud={appointmentsCrud}
        fields={fields}
        lookups={buildLookups(data)}
        emptyValue={emptyAppointment}
        getId={(appointment) => appointment.id}
        getTitle={(appointment) => appointment.name || appointment.patientName || `Consulta ${appointment.id}`}
        onItemsChange={onAppointmentsChange}
        toCreatePayload={toCreatePayload}
        toUpdatePayload={toUpdatePayload}
        columns={[
          { key: 'time', header: 'Horario', render: (appointment) => <strong>{formatAppointmentTime(appointment)}</strong> },
          { key: 'patientName', header: 'Consulta', render: (appointment) => appointment.name || appointment.patientName || 'Sem nome' },
          { key: 'kind', header: 'Formato', render: (appointment) => appointment.kind },
          { key: 'status', header: 'Status', render: (appointment) => statusBadge(appointment.status) },
        ]}
        detailFields={[
          { label: 'Paciente', value: (appointment) => appointment.patientName || `Paciente #${appointment.patientId}` },
          { label: 'Psicologo', value: (appointment) => (appointment.psychologistId ? `Psicologo #${appointment.psychologistId}` : 'Nao informado') },
          { label: 'Inicio', value: (appointment) => formatDateTime(appointment.startsAt) },
          { label: 'Fim', value: (appointment) => formatDateTime(appointment.endsAt) },
          { label: 'Modalidade', value: (appointment) => appointment.modality || appointment.kind },
          { label: 'Status', value: (appointment) => statusBadge(appointment.status) },
        ]}
        actions={[
          {
            label: 'Cancelar consulta',
            tone: 'danger',
            successMessage: 'Consulta cancelada.',
            run: (appointment) => appointmentsApi.cancel(appointment.id, { reason: 'Cancelada pelo workspace web' }),
          },
          { label: 'Consultar horarios livres', successMessage: 'Horarios disponiveis carregados.', run: loadSlots },
        ]}
      />
      <Section
        title="Disponibilidade e bloqueios"
        description="Configure janelas semanais, revise bloqueios e consulte slots sem sair da agenda."
      >
        <div className="workflow-actions">
          <Button
            type="button"
            variant="secondary"
            disabled={pendingAction !== null}
            onClick={loadAgendaSetup}
          >
            {pendingAction === 'load-setup' ? 'Carregando configuracao...' : 'Carregar configuracao'}
          </Button>
          <Button type="button" disabled={pendingAction !== null} onClick={loadSlots}>
            {pendingAction === 'load-slots' ? 'Consultando slots...' : 'Consultar slots'}
          </Button>
        </div>
        {slotsRange ? <p className="workflow-note">Slots consultados: {slotsRange}.</p> : null}
        <div className="workflow-grid">
          <WorkflowList
            title="Disponibilidade semanal"
            empty="Nenhuma janela carregada."
            items={availabilities.map(
              (item) =>
                `${weekdayLabel(item.weekday)} ${item.startTime} ate ${item.endTime}, ${item.slotDurationMinutes} min, ${item.modality}`,
            )}
          />
          <WorkflowList
            title="Bloqueios"
            empty="Nenhum bloqueio carregado."
            items={blocks.map(
              (item) =>
                `${formatDateTime(item.startsAt)} ate ${formatDateTime(item.endsAt)}${item.reason ? `, ${item.reason}` : ''}`,
            )}
          />
          <WorkflowList
            title="Slots disponiveis"
            empty="Consulte slots para os proximos 7 dias."
            items={slots
              .slice(0, 6)
              .map((item) => `${formatDateTime(item.startsAt)} ate ${formatDateTime(item.endsAt)}, ${item.modality}`)}
          />
        </div>
      </Section>
    </div>
  );
}

function WorkflowList({ title, empty, items }: { title: string; empty: string; items: string[] }) {
  return (
    <div className="workflow-panel">
      <h3>{title}</h3>
      {items.length ? (
        <ul>
          {items.map((item) => (
            <li key={item}>{item}</li>
          ))}
        </ul>
      ) : (
        <p>{empty}</p>
      )}
    </div>
  );
}

function weekdayLabel(value: number) {
  return ['Domingo', 'Segunda', 'Terca', 'Quarta', 'Quinta', 'Sexta', 'Sabado'][value] ?? `Dia ${value}`;
}

function formatDateTime(value?: string) {
  if (!value) return 'Sem horario';
  return new Date(value).toLocaleString('pt-BR', {
    day: '2-digit',
    month: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
  });
}

function formatAppointmentTime(appointment: Appointment) {
  if (appointment.time) return appointment.time;
  if (appointment.startsAt) {
    return new Date(appointment.startsAt).toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' });
  }
  return '--';
}
