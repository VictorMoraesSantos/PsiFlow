import { api, setSessionTokens, ApiError } from './http';
import type { AuthLoginPayload, AuthRegisterPayload, AuthForgotPasswordPayload, AuthMfaVerifyPayload, UserProfile, UserPreferences } from '../types';

export async function login(payload: AuthLoginPayload): Promise<void> {
  const tokens = await api.post<Record<string, string>>('/api/auth/v1/auth/login', payload);
  const accessToken = tokens.accessToken ?? tokens.token ?? tokens.jwt;
  if (!accessToken) throw new ApiError('Login realizado, mas token nao foi retornado.', 500);
  setSessionTokens(accessToken, tokens.refreshToken);
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
