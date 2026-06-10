# Sessions

## Responsabilidade
Gerenciar a sessão clínica-administrativa derivada de um agendamento, com máquina de estados, histórico, comparecimento, falta, cancelamento e pagamento manual.

## Como rodar
1. `docker compose -f deploy/docker/docker-compose.dev.yml up -d`
2. `dotnet ef database update --project src/Services/Sessions/Sessions.Infrastructure --startup-project src/Services/Sessions/Sessions.API`
3. `dotnet run --project src/Services/Sessions/Sessions.API`

## Endpoints principais
- `GET /v1/sessions`
- `GET /v1/sessions/{id}`
- `GET /v1/patients/{patientId}/sessions`
- `POST /v1/sessions/{id}/start|complete|no-show|cancel`
- `POST /v1/sessions/{id}/payment/mark-received`
- `GET /v1/sessions/{id}/payment`
- `POST /v1/sessions/{id}/receipt/send`

## Eventos publicados
SessionCreated, SessionStarted, SessionCompleted, SessionMarkedNoShow, SessionCanceled, ManualPaymentReceived, ReceiptRequested

## Eventos consumidos
AppointmentScheduled, AppointmentCancelled, AppointmentLateCancelled, VideoRoomLinked

## Máquina de estados
scheduled → in_progress, canceled, no_show
in_progress → completed, no_show, canceled
completed / no_show / canceled → terminais
