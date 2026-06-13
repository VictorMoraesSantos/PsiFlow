import 'server-only';

const required = (name: string, fallback?: string): string => {
  const value = process.env[name] ?? fallback;
  if (!value) {
    throw new Error(`Missing required server env var: ${name}`);
  }
  return value;
};

export const backendConfig = {
  auth: required('AUTH_API_URL', 'http://localhost:5001'),
  patients: required('PATIENTS_API_URL', 'http://localhost:5002'),
  agenda: required('AGENDA_API_URL', 'http://localhost:5003'),
  sessions: required('SESSIONS_API_URL', 'http://localhost:5004'),
  clinicalRecords: required('CLINICAL_RECORDS_API_URL', 'http://localhost:5005'),
  notifications: required('NOTIFICATIONS_API_URL', 'http://localhost:5006'),
  onlineSession: required('ONLINE_SESSION_API_URL', 'http://localhost:5007'),
  requestTimeoutMs: Number(process.env.UPSTREAM_TIMEOUT_MS ?? 15000),
} as const;

export type BackendService = keyof Omit<typeof backendConfig, 'requestTimeoutMs'>;

export const backendByPath: Record<string, BackendService> = {
  '/api/auth': 'auth',
  '/api/users': 'auth',
  '/api/patients': 'patients',
  '/api/patient-invites': 'patients',
  '/api/agenda': 'agenda',
  '/api/appointments': 'agenda',
  '/api/availability': 'agenda',
  '/api/schedule-blocks': 'agenda',
  '/api/available-slots': 'agenda',
  '/api/sessions': 'sessions',
  '/api/clinical-records': 'clinicalRecords',
  '/api/notification-templates': 'notifications',
  '/api/notification-logs': 'notifications',
  '/api/notifications': 'notifications',
  '/api/video-rooms': 'onlineSession',
  '/api/video-settings': 'onlineSession',
};
