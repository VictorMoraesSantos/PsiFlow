import { api } from './http';
import type { Patient, PatientInvite, PatientStatusUpdate } from '../types';

export const patientsApi = {
  list: () => api.get<Patient[]>('/api/patients/v1/patients'),
  create: (payload: Patient) => api.post<Patient>('/api/patients/v1/patients', payload),
  update: (id: number, payload: Patient) => api.put<Patient>(`/api/patients/v1/patients/${id}`, payload),
  deactivate: (id: number, reason: string) => api.post<unknown>(`/api/patients/v1/patients/${id}/deactivate`, { reason }),
  invite: (payload: PatientInvite) => api.post<unknown>('/api/patients/v1/patient-invites', payload),
  updateStatus: (id: number, payload: PatientStatusUpdate) =>
    api.post<unknown>(`/api/patients/v1/patients/${id}/status`, payload),
  sessionsSummary: (id: number) => api.get<unknown>(`/api/patients/v1/patients/${id}/sessions-summary`),
};
