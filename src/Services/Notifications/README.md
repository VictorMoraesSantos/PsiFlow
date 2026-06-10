# Notifications

## Responsabilidade
Enviar e registrar notificações transacionais por e-mail, com templates versionados, lembretes T-24h e T-1h de sessão, retry com backoff.

> **RN013**: nenhuma notificação pode conter conteúdo clínico. Apenas dados administrativos (data/hora, nomes, link de vídeo, status).

## Como rodar
1. `docker compose -f deploy/docker/docker-compose.dev.yml up -d`
2. `dotnet ef database update --project src/Services/Notifications/Notifications.Infrastructure --startup-project src/Services/Notifications/Notifications.API`
3. `dotnet run --project src/Services/Notifications/Notifications.API`

## Endpoints principais
- `GET/POST /v1/notification-templates`
- `POST /v1/notification-templates/{id}/versions`
- `GET /v1/notification-logs[/{id}]`
- `POST /v1/notifications/test-email`
- `POST /v1/notifications/retry/{id}`

## Templates mínimos
patient_invite_email, appointment_scheduled_patient/psychologist, appointment_cancelled_patient/psychologist, session_reminder_t24h, session_reminder_t1h, manual_payment_receipt, password_reset, welcome_psychologist, welcome_patient

## Eventos publicados
NotificationScheduled, NotificationSent, NotificationFailed, NotificationSkipped

## Eventos consumidos
PatientInvited, AppointmentScheduled, AppointmentCancelled, AppointmentLateCancelled, SessionCompleted, ManualPaymentReceived, ReceiptRequested, VideoRoomLinked
