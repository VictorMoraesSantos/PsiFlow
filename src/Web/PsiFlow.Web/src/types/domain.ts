import type { Id, StatusTone, FieldType, LookupOption, FormField, LookupMap } from './common';
import type { Patient, PatientInvite, PatientStatusUpdate } from './patient';
import type { Appointment, WeeklyAvailability, ScheduleBlock, AvailableSlot, AppointmentCancellation } from './agenda';
import type { Session, MedicalRecord, SessionAction, PaymentMarkReceived, EvolutionAutosave, AnamnesisAutosave, PublishVersion, EvolutionVersion, AuditLogEntry, AnamnesisDocument } from './clinical';
import type { NotificationTemplate, NotificationLog, NotificationTestEmail, NotificationScheduleReminders, NotificationTemplateVersion } from './notification';
import type { VideoRoom, VideoRoomLink, VideoRoomClick, DefaultVideoLink } from './video';
import type { AuthLoginPayload, AuthRegisterPayload, AuthForgotPasswordPayload, AuthMfaVerifyPayload, AuthSession, UserProfile, UserPreferences } from './auth';

export type DashboardData = {
  patients: Patient[];
  appointments: Appointment[];
  sessions: Session[];
  records: MedicalRecord[];
  templates: NotificationTemplate[];
  videoRooms: VideoRoom[];
  notificationLogs?: NotificationLog[];
};

export type {
  Id,
  StatusTone,
  FieldType,
  LookupOption,
  FormField,
  LookupMap,
  Patient,
  PatientInvite,
  PatientStatusUpdate,
  Appointment,
  WeeklyAvailability,
  ScheduleBlock,
  AvailableSlot,
  AppointmentCancellation,
  Session,
  MedicalRecord,
  SessionAction,
  PaymentMarkReceived,
  EvolutionAutosave,
  AnamnesisAutosave,
  PublishVersion,
  EvolutionVersion,
  AuditLogEntry,
  AnamnesisDocument,
  NotificationTemplate,
  NotificationLog,
  NotificationTestEmail,
  NotificationScheduleReminders,
  NotificationTemplateVersion,
  VideoRoom,
  VideoRoomLink,
  VideoRoomClick,
  DefaultVideoLink,
  AuthLoginPayload,
  AuthRegisterPayload,
  AuthForgotPasswordPayload,
  AuthMfaVerifyPayload,
  AuthSession,
  UserProfile,
  UserPreferences,
};
