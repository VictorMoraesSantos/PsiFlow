# Docker Compose para desenvolvimento local
Levanta Postgres 16, Redis 7 e RabbitMQ 3.13 com management UI.

```bash
docker compose -f deploy/docker/docker-compose.dev.yml up -d
```

## Endereços
- Postgres: localhost:5432 (psiflow / psiflow_dev_password)
- Redis: localhost:6379
- RabbitMQ AMQP: localhost:5672 (psiflow / psiflow_dev_password)
- RabbitMQ Management UI: http://localhost:15672

## Databases criados
- psiflow_auth
- psiflow_patients
- psiflow_agenda
- psiflow_sessions
- psiflow_clinical_records
- psiflow_notifications
- psiflow_online_session
