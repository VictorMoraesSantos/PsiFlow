# Agenda

## Responsabilidade
Gerenciar disponibilidade semanal, exceções, bloqueios, horários disponíveis, agendamentos avulsos e cancelamentos com flag `late_cancel`.

## Como rodar
1. `docker compose -f deploy/docker/docker-compose.dev.yml up -d`
2. `dotnet ef database update --project src/Services/Agenda/Agenda.Infrastructure --startup-project src/Services/Agenda/Agenda.API`
3. `dotnet run --project src/Services/Agenda/Agenda.API`

## Endpoints principais
- `POST/GET/PATCH/DELETE /v1/availability/weekly[/{id}]`
- `POST/GET/DELETE /v1/schedule-blocks[/{id}]`
- `POST/GET /v1/appointments`, `GET /v1/appointments/{id}`
- `POST /v1/appointments/{id}/cancel`
- `GET /v1/available-slots`

## Eventos publicados
AvailabilitySet, ScheduleBlockCreated, AppointmentScheduled, AppointmentCancelled, AppointmentLateCancelled

## Eventos consumidos
PatientCreated, PatientDeactivated
