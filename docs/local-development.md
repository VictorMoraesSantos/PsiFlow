# Desenvolvimento local com Docker

## Requisitos

- .NET SDK 10.0.300 ou superior
- Docker Desktop rodando com Linux containers
- Portas livres: 5001-5007, 5432, 5672, 6379, 15672

## Subir tudo

Na raiz do repositório:

```bash
docker compose -f deploy/docker/docker-compose.dev.yml up --build
```

Em segundo plano:

```bash
docker compose -f deploy/docker/docker-compose.dev.yml up --build -d
```

Parar:

```bash
docker compose -f deploy/docker/docker-compose.dev.yml down
```

Limpar dados do Postgres local:

```bash
docker compose -f deploy/docker/docker-compose.dev.yml down -v
```

## Serviços

| Serviço | URL local | Container |
|---|---|---|
| Auth | http://localhost:5001 | psiflow-auth-api |
| Patients | http://localhost:5002 | psiflow-patients-api |
| Agenda | http://localhost:5003 | psiflow-agenda-api |
| Sessions | http://localhost:5004 | psiflow-sessions-api |
| ClinicalRecords | http://localhost:5005 | psiflow-clinical-records-api |
| Notifications | http://localhost:5006 | psiflow-notifications-api |
| OnlineSession | http://localhost:5007 | psiflow-online-session-api |
| RabbitMQ Management | http://localhost:15672 | psiflow-rabbitmq |

RabbitMQ: `psiflow` / `psiflow_dev_password`
Postgres: `psiflow` / `psiflow_dev_password`

## Health checks

Cada API expõe:

```http
/health/live
/health/ready
/health/startup
```

Exemplo:

```bash
curl http://localhost:5002/health/live
```

## Swagger

Em Development, cada API expõe Swagger em:

```http
http://localhost:<porta>/swagger
```

## Build sem Docker

```bash
dotnet restore PsiFlow.slnx
dotnet build PsiFlow.slnx --no-restore
```

## Estrutura padronizada

Os microsserviços seguem o padrão do LifeSync/Auth:

```text
<Service>.API/
  Controllers/

<Service>.Application/
  Contracts/
  DTOs/<Entity>/
  Features/<Entity>/Commands/<Action>/
  Features/<Entity>/Queries/<Action>/
  Mapping/
  DependencyInjection.cs

<Service>.Domain/
  Entities/
  Errors/
  Filters/
  Repositories/
  ValueObjects/

<Service>.Infrastructure/
  Persistence/
  Persistence/Repositories/
  Services/
  DependencyInjection.cs
```

## Observação sobre Docker

Se o comando Docker falhar com `failed to connect to the docker API at npipe:////./pipe/dockerDesktopLinuxEngine`, inicie o Docker Desktop antes de rodar o Compose.