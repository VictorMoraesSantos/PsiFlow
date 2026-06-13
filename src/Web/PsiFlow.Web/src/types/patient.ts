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

export type PatientInvite = {
  patientId: number;
  email: string;
  phone: string;
};

export type PatientStatusUpdate = {
  treatmentStatus: string;
  reason?: string;
};
