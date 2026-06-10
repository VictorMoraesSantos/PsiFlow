# ClinicalRecords

## Responsabilidade
Gerenciar prontuário clínico, anamnese, evolução por sessão, autosave, versionamento append-only, criptografia AES-256-GCM coluna-a-coluna e audit log.

> **RN012**: nenhum conteúdo clínico em logs, eventos ou notificações. Criptografia obrigatória com AAD contendo `tenant_id`, `resource_type`, `resource_id`, `version_id`.

## Como rodar
1. `docker compose -f deploy/docker/docker-compose.dev.yml up -d`
2. `dotnet ef database update --project src/Services/ClinicalRecords/ClinicalRecords.Infrastructure --startup-project src/Services/ClinicalRecords/ClinicalRecords.API`
3. `dotnet run --project src/Services/ClinicalRecords/ClinicalRecords.API`

## Endpoints principais
- `GET/POST /v1/patients/{patientId}/clinical-record`
- `GET /v1/clinical-records/{id}`
- `GET/POST /v1/clinical-records/{id}/anamnesis[/autosave|/publish-version]`
- `GET/POST /v1/sessions/{sessionId}/evolution[/autosave|/publish-version|/versions]`
- `GET /v1/clinical-records/{id}/audit-log`

## Eventos publicados
ClinicalRecordCreated, AnamnesisAutosaved, AnamnesisVersionCreated, EvolutionAutosaved, EvolutionVersionCreated, ClinicalRecordAccessed

## Eventos consumidos
PatientCreated, PatientDeactivated, SessionCreated, SessionCompleted
