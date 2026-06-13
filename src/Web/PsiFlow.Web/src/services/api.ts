import { fallbackData } from '../data/fallbackData';
import type { Appointment, DashboardData, Id, MedicalRecord, NotificationTemplate, Patient, Session, VideoRoom } from '../types';

export const serviceUrls = {
  auth: process.env.NEXT_PUBLIC_AUTH_API_URL ?? 'http://localhost:5001',
  patients: process.env.NEXT_PUBLIC_PATIENTS_API_URL ?? 'http://localhost:5002',
  agenda: process.env.NEXT_PUBLIC_AGENDA_API_URL ?? 'http://localhost:5003',
  sessions: process.env.NEXT_PUBLIC_SESSIONS_API_URL ?? 'http://localhost:5004',
  clinicalRecords: process.env.NEXT_PUBLIC_CLINICAL_RECORDS_API_URL ?? 'http://localhost:5005',
  notifications: process.env.NEXT_PUBLIC_NOTIFICATIONS_API_URL ?? 'http://localhost:5006',
  onlineSession: process.env.NEXT_PUBLIC_ONLINE_SESSION_API_URL ?? 'http://localhost:5007',
};

const tokenKey = 'psiflow.accessToken';
const refreshTokenKey = 'psiflow.refreshToken';
export const localFallbackEvent = 'psiflow:local-fallback';

export type ApiService = keyof typeof serviceUrls;

export class ApiError extends Error {
  constructor(message: string, public status?: number) {
    super(message);
  }
}

export function isLocalFallbackStatus(error: unknown) {
  return error instanceof ApiError && (error.status === 401 || error.status === 405);
}

function announceLocalFallback(status?: number) {
  window.dispatchEvent(new CustomEvent(localFallbackEvent, { detail: { status } }));
}

export function getAccessToken() {
  return localStorage.getItem(tokenKey);
}

export function setSessionTokens(accessToken: string, refreshToken?: string) {
  localStorage.setItem(tokenKey, accessToken);
  if (refreshToken) localStorage.setItem(refreshTokenKey, refreshToken);
}

export function clearSessionTokens() {
  localStorage.removeItem(tokenKey);
  localStorage.removeItem(refreshTokenKey);
}

async function request<T>(service: ApiService, path: string, init: RequestInit = {}): Promise<T> {
  const token = getAccessToken();
  const headers = new Headers(init.headers);
  headers.set('Accept', 'application/json');
  if (init.body && !headers.has('Content-Type')) headers.set('Content-Type', 'application/json');
  if (token) headers.set('Authorization', `Bearer ${token}`);

  const response = await fetch(`${serviceUrls[service]}${path}`, { ...init, headers });
  if (response.status === 204) return undefined as T;

  const text = await response.text();
  const payload = text ? JSON.parse(text) : null;

  if (!response.ok) {
    if (response.status === 401 || response.status === 405) announceLocalFallback(response.status);
    throw new ApiError(payload?.error ?? payload?.detail ?? payload?.title ?? 'Nao foi possivel concluir a operacao.', response.status);
  }

  if (Array.isArray(payload)) return payload as T;
  if (Array.isArray(payload?.items)) return payload.items as T;
  if (Array.isArray(payload?.value)) return payload.value as T;
  return (payload?.value ?? payload) as T;
}

export const api = {
  get: <T>(service: ApiService, path: string) => request<T>(service, path),
  post: <T>(service: ApiService, path: string, body?: unknown) => request<T>(service, path, { method: 'POST', body: body === undefined ? undefined : JSON.stringify(body) }),
  put: <T>(service: ApiService, path: string, body?: unknown) => request<T>(service, path, { method: 'PUT', body: body === undefined ? undefined : JSON.stringify(body) }),
  patch: <T>(service: ApiService, path: string, body?: unknown) => request<T>(service, path, { method: 'PATCH', body: body === undefined ? undefined : JSON.stringify(body) }),
  delete: <T>(service: ApiService, path: string) => request<T>(service, path, { method: 'DELETE' }),
};

export function safeFallback<T>(fallback: T) {
  return async (operation: Promise<T>): Promise<T> => {
    try {
      return await operation;
    } catch {
      return fallback;
    }
  };
}

async function getJson<T>(service: ApiService, path: string, fallback: T, normalize: (items: unknown[]) => T): Promise<T> {
  try {
    const payload = await api.get<unknown>(service, path);
    const items = Array.isArray(payload) ? payload : [];
    return normalize(items);
  } catch {
    return fallback;
  }
}

function asRecord(item: unknown): Record<string, unknown> {
  return item && typeof item === 'object' ? item as Record<string, unknown> : {};
}

function toNumber(value: unknown, fallback = 0) {
  return typeof value === 'number' ? value : Number(value) || fallback;
}

function toStringValue(value: unknown, fallback = '') {
  return typeof value === 'string' ? value : fallback;
}

function normalizePatients(items: unknown[]): Patient[] {
  return items.map((item, index) => {
    const row = asRecord(item);
    const fullName = toStringValue(row.fullName ?? row.name, `Paciente ${index + 1}`);
    return {
      id: toNumber(row.id, index + 1),
      tenantId: toNumber(row.tenantId, 1),
      name: fullName,
      fullName,
      email: toStringValue(row.email),
      phone: toStringValue(row.phone),
      birthDate: row.birthDate as string | null | undefined,
      status: row.status === 'inactive' ? 'Inativo' : row.status === 'pending' ? 'Aguardando' : 'Ativo',
      treatmentStatus: toStringValue(row.treatmentStatus, 'screening'),
      emergencyContactName: row.emergencyContactName as string | null | undefined,
      emergencyContactPhone: row.emergencyContactPhone as string | null | undefined,
      nextSession: 'Sem sessao agendada',
      risk: 'Baixo',
    };
  });
}

function normalizeAppointments(items: unknown[]): Appointment[] {
  return items.map((item, index) => {
    const row = asRecord(item);
    const startsAt = toStringValue(row.startsAt);
    return {
      id: toNumber(row.id, index + 1),
      tenantId: toNumber(row.tenantId, 1),
      name: toStringValue(row.name, `Consulta ${index + 1}`),
      patientId: toNumber(row.patientId),
      psychologistId: toNumber(row.psychologistId),
      startsAt,
      endsAt: toStringValue(row.endsAt),
      patientName: toStringValue(row.name, `Paciente ${toNumber(row.patientId, index + 1)}`),
      time: startsAt ? new Date(startsAt).toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' }) : '--:--',
      kind: row.modality === 'in_person' ? 'Presencial' : 'Online',
      modality: toStringValue(row.modality, 'online'),
      status: row.status === 'canceled' ? 'Cancelada' : row.status === 'scheduled' ? 'Confirmada' : 'Pendente',
      createdBy: toNumber(row.createdBy, 1),
    };
  });
}
            
function normalizeSessions(items: unknown[]): Session[] {
  return items.map((item, index) => {
    const row = asRecord(item);
    return {
      id: toNumber(row.id, index + 1),
      tenantId: toNumber(row.tenantId, 1),
      name: toStringValue(row.name, `Sessao ${index + 1}`),
      appointmentId: toNumber(row.appointmentId),
      patientId: toNumber(row.patientId),
      psychologistId: toNumber(row.psychologistId),
      startsAt: toStringValue(row.startsAt),
      endsAt: toStringValue(row.endsAt),
      patientName: toStringValue(row.name, `Paciente ${toNumber(row.patientId, index + 1)}`),
      status: row.status === 'completed' ? 'Finalizada' : row.status === 'started' ? 'Em andamento' : 'Preparar',
      modality: toStringValue(row.modality, 'online'),
      payment: 'Pendente',
      room: row.modality === 'in_person' ? 'Consultorio' : 'Sala online',
    };
  });
}

function normalizeRecords(items: unknown[]): MedicalRecord[] {
  return items.map((item, index) => {
    const row = asRecord(item);
    return {
      id: toNumber(row.id, index + 1),
      tenantId: toNumber(row.tenantId, 1),
      patientId: toNumber(row.patientId),
      name: toStringValue(row.name, `Prontuario ${index + 1}`),
      patientName: `Paciente ${toNumber(row.patientId, index + 1)}`,
      updatedAt: row.updatedAt ? `Atualizado em ${new Date(String(row.updatedAt)).toLocaleDateString('pt-BR')}` : 'Sem atualizacao recente',
      status: row.status === 'active' ? 'Rascunho' : row.status === 'signed' ? 'Assinado' : 'Revisar',
    };
  });
}

function normalizeTemplates(items: unknown[]): NotificationTemplate[] {
  return items.map((item, index) => {
    const row = asRecord(item);
    return {
      id: toNumber(row.id, index + 1),
      tenantId: row.tenantId as number | null | undefined,
      key: toStringValue(row.key, `template_${index + 1}`),
      name: toStringValue(row.name, `Modelo ${index + 1}`),
      channel: row.channel === 'whatsapp' ? 'WhatsApp' : row.channel === 'system' ? 'Sistema' : 'Email',
      status: row.status === 'paused' || row.isActive === false ? 'Pausado' : 'Ativo',
      isActive: row.isActive !== false,
    };
  });
}

function normalizeVideoRooms(items: unknown[]): VideoRoom[] {
  return items.map((item, index) => {
    const row = asRecord(item);
    return {
      id: toNumber(row.id, index + 1),
      tenantId: toNumber(row.tenantId, 1),
      sessionId: toNumber(row.sessionId),
      name: toStringValue(row.name, `Sala ${index + 1}`),
      provider: toStringValue(row.provider, 'external'),
      urlEncrypted: toStringValue(row.urlEncrypted),
      urlHash: toStringValue(row.urlHash),
      instructions: row.instructions as string | null | undefined,
      createdBy: toNumber(row.createdBy, 1),
      status: row.status === 'inactive' ? 'Inativo' : row.status === 'paused' ? 'Pausado' : 'Ativo',
    };
  });
}

export async function loadDashboardData(): Promise<DashboardData> {
  const [patients, appointments, sessions, records, templates, videoRooms] = await Promise.all([
    getJson('patients', '/v1/patients', fallbackData.patients, normalizePatients),
    getJson('agenda', '/v1/appointments', fallbackData.appointments, normalizeAppointments),
    getJson('sessions', '/v1/sessions', fallbackData.sessions, normalizeSessions),
    getJson('clinicalRecords', '/v1/clinical-records', fallbackData.records, normalizeRecords),
    getJson('notifications', '/v1/notification-templates', fallbackData.templates, normalizeTemplates),
    getJson('onlineSession', '/v1/video-rooms', fallbackData.videoRooms, normalizeVideoRooms),
  ]);

  return { patients, appointments, sessions, records, templates, videoRooms };
}

export async function createResource<T>(service: ApiService, path: string, body: unknown, fallbackItem: T): Promise<T> {
  try {
    return await api.post<T>(service, path, body);
  } catch (error) {
    if (isLocalFallbackStatus(error)) return fallbackItem;
    return fallbackItem;
  }
}

export async function updateResource<T>(service: ApiService, path: string, id: Id, body: unknown, fallbackItem: T): Promise<T> {
  try {
    return await api.put<T>(service, `${path}/${id}`, body);
  } catch (error) {
    if (isLocalFallbackStatus(error)) return fallbackItem;
    return fallbackItem;
  }
}

export async function deleteResource(service: ApiService, path: string, id: Id): Promise<void> {
  try {
    await api.delete(service, `${path}/${id}`);
  } catch (error) {
    if (isLocalFallbackStatus(error)) return;
    return;
  }
}

export async function login(email: string, password: string) {
  const payload = await api.post<Record<string, string>>('auth', '/v1/auth/login', { email, password });
  const accessToken = payload.accessToken ?? payload.token ?? payload.jwt;
  const refreshToken = payload.refreshToken;
  if (!accessToken) throw new ApiError('Login realizado, mas token nao foi retornado.');
  setSessionTokens(accessToken, refreshToken);
}
