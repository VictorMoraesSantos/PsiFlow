import { api } from './http';
import type {
  AnamnesisAutosave,
  AnamnesisDocument,
  AuditLogEntry,
  EvolutionAutosave,
  EvolutionVersion,
  MedicalRecord,
  PublishVersion,
} from '../types';

export const clinicalRecordsApi = {
  list: () => api.get<MedicalRecord[]>('/api/clinical-records/v1/clinical-records'),
  create: (payload: MedicalRecord) => api.post<MedicalRecord>('/api/clinical-records/v1/clinical-records', payload),
  update: (id: number, payload: MedicalRecord) => api.put<MedicalRecord>(`/api/clinical-records/v1/clinical-records/${id}`, payload),
  anamnesis: (id: number) => api.get<AnamnesisDocument>(`/api/clinical-records/v1/clinical-records/${id}/anamnesis`),
  autosaveAnamnesis: (id: number, payload: AnamnesisAutosave) =>
    api.post<unknown>(`/api/clinical-records/v1/clinical-records/${id}/anamnesis/autosave`, payload),
  publishAnamnesis: (id: number, payload: PublishVersion) =>
    api.post<unknown>(`/api/clinical-records/v1/clinical-records/${id}/anamnesis/publish-version`, payload),
  autosaveEvolution: (sessionId: number, payload: EvolutionAutosave) =>
    api.post<unknown>(`/api/clinical-records/v1/sessions/${sessionId}/evolution/autosave`, payload),
  publishEvolution: (sessionId: number, payload: PublishVersion) =>
    api.post<unknown>(`/api/clinical-records/v1/sessions/${sessionId}/evolution/publish-version`, payload),
  evolutionVersions: (sessionId: number) =>
    api.get<EvolutionVersion[]>(`/api/clinical-records/v1/sessions/${sessionId}/evolution/versions`),
  auditLog: (id: number) => api.get<AuditLogEntry[]>(`/api/clinical-records/v1/clinical-records/${id}/audit-log`),
};
