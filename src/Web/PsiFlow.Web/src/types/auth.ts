export type AuthLoginPayload = {
  email: string;
  password: string;
};

export type AuthRegisterPayload = {
  role: 'psychologist' | 'patient';
  fullName: string;
  email: string;
  password: string;
  confirmPassword: string;
  phone?: string | null;
  crp?: string | null;
  acceptedTermsVersion: string;
  acceptedPrivacyVersion: string;
};

export type AuthForgotPasswordPayload = {
  email: string;
};

export type AuthMfaVerifyPayload = {
  code: string;
};

export type AuthSession = {
  accessToken: string;
  refreshToken?: string;
};

export type UserProfile = {
  fullName: string;
  email: string;
  phone?: string;
  crp?: string;
  specialty?: string;
};

export type UserPreferences = {
  timezone: string;
  appointmentReminder: string;
  sessionStartWindow: string;
  emailNotifications: boolean;
  clinicalSummaryEmail: boolean;
  requireMfa: boolean;
};
