export type StatusTone = 'neutral' | 'success' | 'warning' | 'danger' | 'info';

export type Id = string | number;

export type Patient = {
  id: number;
  tenantId?: number;
  name: string;
  fullName?: string;
  email: string;
  phone: string;
  birthDate?: string | null;
  status: 'Ativo' | 'Aguardando' | 'Inativo';
  treatmentStatus?: string;
  emergencyContactName?: string | null;
  emergencyContactPhone?: string | null;
  nextSession: string;
  risk: 'Baixo' | 'Moderado' | 'Alto';
};

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

export type Session = {
  id: number;
  tenantId?: number;
  name?: string;
  appointmentId?: number;
  patientId?: number;
  psychologistId?: number;
  startsAt?: string;
  endsAt?: string;
  patientName: string;
  status: 'Preparar' | 'Em andamento' | 'Finalizada';
  modality?: string;
  payment: 'Pago' | 'Pendente';
  room: string;
};

export type MedicalRecord = {
  id: number;
  tenantId?: number;
  patientId?: number;
  name?: string;
  patientName: string;
  updatedAt: string;
  status: 'Rascunho' | 'Assinado' | 'Revisar';
};

export type NotificationTemplate = {
  id: number;
  tenantId?: number | null;
  key?: string;
  name: string;
  channel: 'Email' | 'WhatsApp' | 'Sistema';
  status: 'Ativo' | 'Pausado';
  isActive?: boolean;
};

export type VideoRoom = {
  id: number;
  tenantId?: number;
  sessionId: number;
  name: string;
  provider: string;
  urlEncrypted?: string;
  urlHash?: string;
  instructions?: string | null;
  createdBy?: number;
  status: 'Ativo' | 'Pausado' | 'Inativo';
};

export type DashboardData = {
  patients: Patient[];
  appointments: Appointment[];
  sessions: Session[];
  records: MedicalRecord[];
  templates: NotificationTemplate[];
  videoRooms: VideoRoom[];
};

export type FieldType = 'text' | 'email' | 'tel' | 'number' | 'date' | 'datetime-local' | 'select' | 'lookup' | 'textarea' | 'checkbox';

export type LookupOption = { label: string; value: string | number };

export type FormField<T> = {
  name: keyof T & string;
  label: string;
  type: FieldType;
  required?: boolean;
  placeholder?: string;
  options?: Array<{ label: string; value: string | number | boolean }>;
  lookupKey?: string;
  helperText?: string;
};

export type LookupMap = Record<string, LookupOption[]>;
