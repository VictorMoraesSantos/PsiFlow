# Patients

## Responsabilidade
Gerenciar pacientes vinculados a uma psicóloga, dados administrativos, convites por link, contato de emergência, responsável legal, status de tratamento e inativação sem perda de histórico.

## Como rodar localmente
1. Subir infra: `docker compose -f deploy/docker/docker-compose.dev.yml up -d`
2. Aplicar migrations: `dotnet ef database update --project src/Services/Patients/Patients.Infrastructure --startup-project src/Services/Patients/Patients.API`
3. Rodar: `dotnet run --project src/Services/Patients/Patients.API`

## Endpoints principais
- `POST /v1/patients`
- `GET /v1/patients`
- `GET /v1/patients/{id}`
- `PATCH /v1/patients/{id}`
- `POST /v1/patients/{id}/deactivate`
- `POST /v1/patients/{id}/status`
- `GET /v1/patients/{id}/sessions-summary`
- `POST /v1/patient-invites`
- `GET /v1/patient-invites/{token}/preview`
- `POST /v1/patient-invites/{token}/accept`
- `POST /v1/patient-invites/{inviteId}/revoke`

## Eventos publicados
- PatientCreated
- PatientInvited
- PatientAcceptedInvite
- PatientDeactivated
- PatientTreatmentStatusChanged

## Eventos consumidos
- UserRegistered

## Testes
- Unitários: `dotnet test tests/Services/Patients/Patients.Tests.Unit`
- Arquitetura: `dotnet test tests/Services/Patients/Patients.Tests.Architecture`
