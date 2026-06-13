import { api } from './http';
import type { Session, SessionAction, PaymentMarkReceived } from '../types';

export const sessionsApi = {
  list: () => api.get<Session[]>('/api/sessions/v1/sessions'),
  create: (payload: Session) => api.post<Session>('/api/sessions/v1/sessions', payload),
  update: (id: number, payload: Session) => api.put<Session>(`/api/sessions/v1/sessions/${id}`, payload),
  start: (id: number, payload: SessionAction) => api.post<unknown>(`/api/sessions/v1/sessions/${id}/start`, payload),
  complete: (id: number, payload: SessionAction) => api.post<unknown>(`/api/sessions/v1/sessions/${id}/complete`, payload),
  noShow: (id: number, payload: SessionAction) => api.post<unknown>(`/api/sessions/v1/sessions/${id}/no-show`, payload),
  cancel: (id: number, payload: SessionAction) => api.post<unknown>(`/api/sessions/v1/sessions/${id}/cancel`, payload),
  payment: (id: number) => api.get<unknown>(`/api/sessions/v1/sessions/${id}/payment`),
  markPaymentReceived: (id: number, payload: PaymentMarkReceived) =>
    api.post<unknown>(`/api/sessions/v1/sessions/${id}/payment/mark-received`, payload),
  sendReceipt: (id: number) => api.post<unknown>(`/api/sessions/v1/sessions/${id}/receipt/send`),
};
