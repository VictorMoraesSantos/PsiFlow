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

const isMfaChallenge = (payload: unknown): payload is { mfaToken?: string; challengeId?: string } => {
  if (!payload || typeof payload !== 'object') return false;
  const record = payload as Record<string, unknown>;
  return record.mfaRequired === true || typeof record.challengeId === 'string';
};

export type LoginResult =
  | { kind: 'authenticated' }
  | { kind: 'mfa-required'; mfaToken?: string; challengeId?: string };

export async function login(payload: AuthLoginPayload): Promise<LoginResult> {
  const response = await api.post<Record<string, unknown>>('/api/auth/v1/auth/login', payload, { skipAuth: true });

  if (isMfaChallenge(response)) {
    return {
      kind: 'mfa-required',
      mfaToken: response.mfaToken as string | undefined,
      challengeId: response.challengeId as string | undefined,
    };
  }

  const accessToken = extractAccessToken(response);
  if (!accessToken) {
    console.warn('[auth] resposta de login sem accessToken reconhecivel', response);
    throw new ApiError('Login realizado, mas o token nao foi retornado pelo servidor.', 500);
  }
  setSessionTokens(accessToken, extractRefreshToken(response));
  return { kind: 'authenticated' };
}

export async function completeMfa(mfaToken: string, code: string): Promise<void> {
  const response = await api.post<Record<string, unknown>>(
    '/api/auth/v1/auth/mfa/complete',
    { mfaToken, code },
    { skipAuth: true },
  );
  const accessToken = extractAccessToken(response);
  if (!accessToken) throw new ApiError('MFA aceito, mas o token nao foi retornado pelo servidor.', 500);
  setSessionTokens(accessToken, extractRefreshToken(response));
}

export function register(payload: AuthRegisterPayload) {
  return api.post<unknown>('/api/auth/v1/auth/register', payload, { skipAuth: true });
}

export function forgotPassword(payload: AuthForgotPasswordPayload) {
  return api.post<unknown>('/api/auth/v1/auth/forgot-password', payload, { skipAuth: true });
}

export function verifyMfa(payload: AuthMfaVerifyPayload) {
  return api.post<unknown>('/api/auth/v1/auth/mfa/verify', payload);
}

export function logoutServer() {
  return api.post<unknown>('/api/auth/v1/auth/logout');
}

export function me() {
  return api.get<unknown>('/api/auth/v1/auth/me');
}

export function updateProfile(payload: UserProfile) {
  return api.put<unknown>('/api/users/v1/users/me', payload);
}

export function updatePreferences(payload: UserPreferences) {
  return api.put<unknown>('/api/users/v1/users/me/settings', payload);
}
