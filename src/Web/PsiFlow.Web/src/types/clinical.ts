import type { Id } from './common';

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

export type SessionAction = {
  reason: string;
};

export type PaymentMarkReceived = {
  amountCents: number;
  currency: string;
  notes?: string;
};

export type EvolutionAutosave = {
  ciphertext: string;
  nonce: string;
  tag: string;
};

export type AnamnesisAutosave = EvolutionAutosave;

export type PublishVersion = {
  reason: string;
};

export type EvolutionVersion = {
  id: Id;
  sessionId: number;
  version: number;
  publishedAt: string;
  authorId: number;
};

export type AuditLogEntry = {
  id: Id;
  recordId: number;
  action: string;
  actorId: number;
  occurredAt: string;
  details?: string;
};

export type AnamnesisDocument = {
  recordId: number;
  ciphertext: string;
  nonce: string;
  tag: string;
  version: number;
};
