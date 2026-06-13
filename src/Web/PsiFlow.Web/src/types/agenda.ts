import type { Id, StatusTone } from './common';

export type Appointment = {
  id: number;
  tenantId?: number;
  name?: string;
  patientId?: number;
  psychologistId?: number;
  startsAt?: string;
  endsAt?: string;
  patientName: string;
  time: string;
  kind: 'Online' | 'Presencial';
  status: 'Confirmada' | 'Pendente' | 'Cancelada';
  modality?: string;
  createdBy?: number;
};

export type WeeklyAvailability = {
  id: number;
  weekday: number;
  startTime: string;
  endTime: string;
  slotDurationMinutes: number;
  modality: string;
  timezone: string;
  isActive: boolean;
};

export type ScheduleBlock = {
  id: Id;
  startsAt: string;
  endsAt: string;
  reason?: string | null;
};

export type AvailableSlot = {
  startsAt: string;
  endsAt: string;
  modality: string;
};

export type AppointmentCancellation = {
  reason: string;
};
