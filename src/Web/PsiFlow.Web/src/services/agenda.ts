import { api } from './http';
import type { Appointment, AppointmentCancellation, AvailableSlot, ScheduleBlock, WeeklyAvailability } from '../types';

export const appointmentsApi = {
  list: () => api.get<Appointment[]>('/api/agenda/v1/appointments'),
  create: (payload: Appointment) => api.post<Appointment>('/api/agenda/v1/appointments', payload),
  update: (id: number, payload: Appointment) => api.put<Appointment>(`/api/agenda/v1/appointments/${id}`, payload),
  cancel: (id: number, payload: AppointmentCancellation) => api.post<unknown>(`/api/agenda/v1/appointments/${id}/cancel`, payload),
};

export const availabilityApi = {
  weekly: () => api.get<WeeklyAvailability[]>('/api/agenda/v1/availability/weekly'),
  scheduleBlocks: () => api.get<ScheduleBlock[]>('/api/agenda/v1/schedule-blocks'),
  availableSlots: (from: string, to: string) =>
    api.get<AvailableSlot[]>('/api/agenda/v1/available-slots', { query: { from, to } }),
};
