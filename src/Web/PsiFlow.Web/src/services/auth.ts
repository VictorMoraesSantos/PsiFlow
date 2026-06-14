import { api, setSessionTokens, ApiError } from './http';
import type {
  AuthForgotPasswordPayload,
  AuthLoginPayload,
  AuthMfaVerifyPayload,
  AuthRegisterPayload,
  UserPreferences,
  UserProfile,
} from '../types';

const extractAccessToken = (payload: unknown): string | undefined => {
  if (!payload || typeof payload !== 'object') return undefined;
  const record = payload as Record<string, unknown>;
  return (
    (record.accessToken as string | undefined) ??
    (record.access_token as string | undefined) ??
    (record.token as string | undefined) ??
    (record.jwt as string | undefined) ??
    (record.access as string | undefined) ??
    (typeof record.data === 'object' && record.data
      ? ((record.data as Record<string, unknown>).accessToken as string | undefined) ??
        ((record.data as Record<string, unknown>).token as string | undefined)
      : undefined)
  );
};

const extractRefreshToken = (payload: unknown): string | undefined => {
  if (!payload || typeof payload !== 'object') return undefined;
  const record = payload as Record<string, unknown>;
  return (
    (record.refreshToken as string | undefined) ??
    (record.refresh_token as string | undefined) ??
    (typeof record.data === 'object' && record.data
      ? ((record.data as Record<string, unknown>).refreshToken as string | undefined)
      : undefined)
  );
};

export async function login(payload: AuthLoginPayload): Promise<void> {
  const tokens = await api.post<Record<string, unknown>>('/api/auth/v1/auth/login', payload);
  const accessToken = extractAccessToken(tokens);
  if (!accessToken) {
    console.warn('[auth] resposta de login sem accessToken reconhecivel', tokens);
    throw new ApiError('Login realizado, mas o token nao foi retornado pelo servidor.', 500);
  }
  setSessionTokens(accessToken, extractRefreshToken(tokens));
}

export function register(payload: AuthRegisterPayload) {
  return api.post<unknown>('/api/auth/v1/auth/register', payload);
}

export function forgotPassword(payload: AuthForgotPasswordPayload) {
  return api.post<unknown>('/api/auth/v1/auth/forgot-password', payload);
}

export function verifyMfa(payload: AuthMfaVerifyPayload) {
  return api.post<unknown>('/api/auth/v1/auth/mfa/verify', payload);
}

export function updateProfile(payload: UserProfile) {
  return api.put<unknown>('/api/users/v1/users/me', payload);
}

export function updatePreferences(payload: UserPreferences) {
  return api.put<unknown>('/api/users/v1/users/me/settings', payload);
}
