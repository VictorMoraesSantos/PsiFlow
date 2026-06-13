import {
  appointmentsApi,
  clinicalRecordsApi,
  notificationLogsApi,
  notificationTemplatesApi,
  patientsApi,
  sessionsApi,
  videoRoomsApi,
} from './resources';
import { fallbackData } from '../data/fallbackData';
import { isLocalFallbackStatus } from './http';
import type { Appointment, DashboardData, MedicalRecord, NotificationLog, NotificationTemplate, Patient, Session, VideoRoom } from '../types';

const asArray = <T,>(value: unknown): T[] => (Array.isArray(value) ? (value as T[]) : []);

const normalizePatients = (items: unknown): Patient[] => {
  const list = asArray<Record<string, unknown>>(items);
  return list.map((row, index) => ({
    id: Number(row.id) || index + 1,
    tenantId: Number(row.tenantId) || 1,
    name: (row.fullName as string) ?? (row.name as string) ?? `Paciente ${index + 1}`,
    fullName: (row.fullName as string) ?? (row.name as string) ?? `Paciente ${index + 1}`,
    email: (row.email as string) ?? '',
    phone: (row.phone as string) ?? '',
    birthDate: (row.birthDate as string | null) ?? null,
    status: row.status === 'inactive' ? 'Inativo' : row.status === 'pending' ? 'Aguardando' : 'Ativo',
    treatmentStatus: (row.treatmentStatus as string) ?? 'screening',
    emergencyContactName: (row.emergencyContactName as string | null) ?? null,
    emergencyContactPhone: (row.emergencyContactPhone as string | null) ?? null,
    nextSession: 'Sem sessao agendada',
    risk: 'Baixo',
  }));
};

const normalizeAppointments = (items: unknown): Appointment[] => {
  const list = asArray<Record<string, unknown>>(items);
  return list.map((row, index) => {
    const startsAt = (row.startsAt as string) ?? '';
    return {
      id: Number(row.id) || index + 1,
      tenantId: Number(row.tenantId) || 1,
      name: (row.name as string) ?? `Consulta ${index + 1}`,
      patientId: row.patientId ? Number(row.patientId) : undefined,
      psychologistId: row.psychologistId ? Number(row.psychologistId) : undefined,
      startsAt,
      endsAt: (row.endsAt as string) ?? '',
      patientName: (row.name as string) ?? `Paciente ${Number(row.patientId) || index + 1}`,
      time: startsAt
        ? new Date(startsAt).toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' })
        : '--:--',
      kind: row.modality === 'in_person' ? 'Presencial' : 'Online',
      modality: (row.modality as string) ?? 'online',
      status: row.status === 'canceled' ? 'Cancelada' : row.status === 'scheduled' ? 'Confirmada' : 'Pendente',
      createdBy: row.createdBy ? Number(row.createdBy) : 1,
    };
  });
};

const normalizeSessions = (items: unknown): Session[] => {
  const list = asArray<Record<string, unknown>>(items);
  return list.map((row, index) => ({
    id: Number(row.id) || index + 1,
    tenantId: Number(row.tenantId) || 1,
    name: (row.name as string) ?? `Sessao ${index + 1}`,
    appointmentId: row.appointmentId ? Number(row.appointmentId) : undefined,
    patientId: row.patientId ? Number(row.patientId) : undefined,
    psychologistId: row.psychologistId ? Number(row.psychologistId) : undefined,
    startsAt: (row.startsAt as string) ?? '',
    endsAt: (row.endsAt as string) ?? '',
    patientName: (row.name as string) ?? `Paciente ${Number(row.patientId) || index + 1}`,
    status: row.status === 'completed' ? 'Finalizada' : row.status === 'started' ? 'Em andamento' : 'Preparar',
    modality: (row.modality as string) ?? 'online',
    payment: 'Pendente',
    room: row.modality === 'in_person' ? 'Consultorio' : 'Sala online',
  }));
};

const normalizeRecords = (items: unknown): MedicalRecord[] => {
  const list = asArray<Record<string, unknown>>(items);
  return list.map((row, index) => ({
    id: Number(row.id) || index + 1,
    tenantId: Number(row.tenantId) || 1,
    patientId: row.patientId ? Number(row.patientId) : undefined,
    name: (row.name as string) ?? `Prontuario ${index + 1}`,
    patientName: `Paciente ${Number(row.patientId) || index + 1}`,
    updatedAt: row.updatedAt
      ? `Atualizado em ${new Date(String(row.updatedAt)).toLocaleDateString('pt-BR')}`
      : 'Sem atualizacao recente',
    status: row.status === 'active' ? 'Rascunho' : row.status === 'signed' ? 'Assinado' : 'Revisar',
  }));
};

const normalizeTemplates = (items: unknown): NotificationTemplate[] => {
  const list = asArray<Record<string, unknown>>(items);
  return list.map((row, index) => ({
    id: Number(row.id) || index + 1,
    tenantId: (row.tenantId as number | null) ?? null,
    key: (row.key as string) ?? `template_${index + 1}`,
    name: (row.name as string) ?? `Modelo ${index + 1}`,
    channel: row.channel === 'whatsapp' ? 'WhatsApp' : row.channel === 'system' ? 'Sistema' : 'Email',
    status: row.status === 'paused' || row.isActive === false ? 'Pausado' : 'Ativo',
    isActive: row.isActive !== false,
  }));
};

const normalizeVideoRooms = (items: unknown): VideoRoom[] => {
  const list = asArray<Record<string, unknown>>(items);
  return list.map((row, index) => ({
    id: Number(row.id) || index + 1,
    tenantId: Number(row.tenantId) || 1,
    sessionId: Number(row.sessionId) || 0,
    name: (row.name as string) ?? `Sala ${index + 1}`,
    provider: (row.provider as string) ?? 'external',
    urlEncrypted: (row.urlEncrypted as string) ?? '',
    urlHash: (row.urlHash as string) ?? '',
    instructions: (row.instructions as string | null) ?? null,
    createdBy: Number(row.createdBy) || 1,
    status: row.status === 'inactive' ? 'Inativo' : row.status === 'paused' ? 'Pausado' : 'Ativo',
  }));
};

const normalizeLogs = (items: unknown): NotificationLog[] => asArray<NotificationLog>(items);

const fetchResource = async <T>(loader: () => Promise<T>, fallback: T, normalize: (value: unknown) => T): Promise<T> => {
  try {
    const payload = await loader();
    return normalize(payload);
  } catch (error) {
    if (isLocalFallbackStatus(error)) return fallback;
    throw error;
  }
};

export async function loadDashboardData(): Promise<DashboardData> {
  const [patients, appointments, sessions, records, templates, videoRooms, notificationLogs] = await Promise.all([
    fetchResource(patientsApi.list, fallbackData.patients, normalizePatients),
    fetchResource(appointmentsApi.list, fallbackData.appointments, normalizeAppointments),
    fetchResource(sessionsApi.list, fallbackData.sessions, normalizeSessions),
    fetchResource(clinicalRecordsApi.list, fallbackData.records, normalizeRecords),
    fetchResource(notificationTemplatesApi.list, fallbackData.templates, normalizeTemplates),
    fetchResource(videoRoomsApi.list, fallbackData.videoRooms, normalizeVideoRooms),
    fetchResource(notificationLogsApi.list, [], normalizeLogs),
  ]);

  return { patients, appointments, sessions, records, templates, videoRooms, notificationLogs };
}

export { ApiError, isLocalFallbackStatus, request, api } from './http';
export {
  appointmentsApi,
  availabilityApi,
  clinicalRecordsApi,
  notificationLogsApi,
  notificationTemplatesApi,
  notificationsApi,
  patientsApi,
  sessionsApi,
  videoRoomsApi,
  videoSettingsApi,
} from './resources';
