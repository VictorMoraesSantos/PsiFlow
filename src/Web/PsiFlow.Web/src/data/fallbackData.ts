import type { DashboardData } from '../types';

export const fallbackData: DashboardData = {
  patients: [
    { id: 1, tenantId: 1, name: 'Marina Duarte', fullName: 'Marina Duarte', email: 'marina.duarte@email.com', phone: '(11) 98841-2001', birthDate: '1992-05-14', status: 'Ativo', treatmentStatus: 'screening', emergencyContactName: 'Paula Duarte', emergencyContactPhone: '(11) 97711-4400', nextSession: 'Hoje, 14:30', risk: 'Baixo' },
    { id: 2, tenantId: 1, name: 'Rafael Nogueira', fullName: 'Rafael Nogueira', email: 'rafael.nogueira@email.com', phone: '(21) 99712-4408', birthDate: '1987-09-22', status: 'Aguardando', treatmentStatus: 'active', emergencyContactName: 'Clara Nogueira', emergencyContactPhone: '(21) 96622-1000', nextSession: 'Amanha, 09:00', risk: 'Moderado' },
    { id: 3, tenantId: 1, name: 'Bianca Torres', fullName: 'Bianca Torres', email: 'bianca.torres@email.com', phone: '(31) 98401-7722', birthDate: '1998-01-03', status: 'Ativo', treatmentStatus: 'active', emergencyContactName: 'Renato Torres', emergencyContactPhone: '(31) 95511-8890', nextSession: 'Sex, 11:00', risk: 'Alto' },
  ],
  appointments: [
    { id: 1, tenantId: 1, name: 'Sessao Marina', patientId: 1, psychologistId: 1, startsAt: '2026-06-10T14:30', endsAt: '2026-06-10T15:20', patientName: 'Marina Duarte', time: '14:30', kind: 'Online', modality: 'online', status: 'Confirmada', createdBy: 1 },
    { id: 2, tenantId: 1, name: 'Sessao Rafael', patientId: 2, psychologistId: 1, startsAt: '2026-06-10T16:00', endsAt: '2026-06-10T16:50', patientName: 'Rafael Nogueira', time: '16:00', kind: 'Presencial', modality: 'in_person', status: 'Pendente', createdBy: 1 },
    { id: 3, tenantId: 1, name: 'Sessao Bianca', patientId: 3, psychologistId: 1, startsAt: '2026-06-10T18:15', endsAt: '2026-06-10T19:05', patientName: 'Bianca Torres', time: '18:15', kind: 'Online', modality: 'online', status: 'Confirmada', createdBy: 1 },
  ],
  sessions: [
    { id: 1, tenantId: 1, name: 'Sessao clinica Marina', appointmentId: 1, patientId: 1, psychologistId: 1, startsAt: '2026-06-10T14:30', endsAt: '2026-06-10T15:20', patientName: 'Marina Duarte', status: 'Preparar', modality: 'online', payment: 'Pago', room: 'Sala online pronta' },
    { id: 2, tenantId: 1, name: 'Sessao clinica Rafael', appointmentId: 2, patientId: 2, psychologistId: 1, startsAt: '2026-06-10T16:00', endsAt: '2026-06-10T16:50', patientName: 'Rafael Nogueira', status: 'Em andamento', modality: 'in_person', payment: 'Pendente', room: 'Consultorio 2' },
    { id: 3, tenantId: 1, name: 'Sessao clinica Bianca', appointmentId: 3, patientId: 3, psychologistId: 1, startsAt: '2026-06-10T18:15', endsAt: '2026-06-10T19:05', patientName: 'Bianca Torres', status: 'Finalizada', modality: 'online', payment: 'Pago', room: 'Sala online encerrada' },
  ],
  records: [
    { id: 1, tenantId: 1, patientId: 1, name: 'Prontuario Marina', patientName: 'Marina Duarte', updatedAt: 'Atualizado ha 18 min', status: 'Rascunho' },
    { id: 2, tenantId: 1, patientId: 2, name: 'Prontuario Rafael', patientName: 'Rafael Nogueira', updatedAt: 'Atualizado ontem', status: 'Assinado' },
    { id: 3, tenantId: 1, patientId: 3, name: 'Prontuario Bianca', patientName: 'Bianca Torres', updatedAt: 'Atualizado ha 2 dias', status: 'Revisar' },
  ],
  templates: [
    { id: 1, tenantId: 1, key: 'appointment_reminder', name: 'Lembrete de consulta', channel: 'WhatsApp', status: 'Ativo', isActive: true },
    { id: 2, tenantId: 1, key: 'payment_confirmation', name: 'Confirmacao de pagamento', channel: 'Email', status: 'Ativo', isActive: true },
    { id: 3, tenantId: 1, key: 'reschedule_pending', name: 'Reagendamento pendente', channel: 'Sistema', status: 'Pausado', isActive: false },
  ],
  videoRooms: [
    { id: 1, tenantId: 1, sessionId: 1, name: 'Sala Marina', provider: 'external', urlEncrypted: '', urlHash: 'room-marina', instructions: 'Entrar cinco minutos antes.', createdBy: 1, status: 'Ativo' },
    { id: 2, tenantId: 1, sessionId: 3, name: 'Sala Bianca', provider: 'external', urlEncrypted: '', urlHash: 'room-bianca', instructions: 'Link de contingencia disponivel.', createdBy: 1, status: 'Ativo' },
  ],
};
