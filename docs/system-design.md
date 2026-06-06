# PsiFlow — System Design

> Design arquitetural da plataforma PsiFlow (SaaS para psicólogas).
> Documento técnico de referência para engenharia.
> Última atualização: 2026-06-03.

---

## 1. Contexto & Objetivos

**PsiFlow.** SaaS B2B/B2C para psicólogas gerenciarem agenda, pacientes, sessões, prontuários, documentos, pagamentos e comunicação. Backend multi-tenant com isolamento por profissional/clínica.

**Restrições de domínio.**
- Dados clínicos = **dados sensíveis de saúde** (LGPD art. 5º, II + art. 11).
- Obrigações CFP (Conselho Federal de Psicologia): prontuário 5 anos, registro de sessões, sigilo.
- Multi-tenant: psicóloga autônoma = tenant; clínica = tenant agregador.

**Objetivos arquiteturais.**
- **Autonomia de deploy** por serviço (independência de release).
- **Isolamento de dados** (1 banco por serviço, schema dedicado).
- **Observabilidade nativa** (traces + métricas + logs desde o template).
- **LGPD-by-design** (criptografia, auditoria, consentimento versionado).
- **Time-to-feature curto** para novo serviço (template copy-paste).
- **Risco baixo** em mudanças (bounded contexts claros, contratos versionados).

**Não-objetivos (YAGNI).**
- Multi-idioma / multi-moeda (v3+).
- Marketplace aberto (v2+).
- Mobile nativo (web responsivo + PWA).
- IA clínica (proibida por regulação; IA só administrativa).
- Sala de vídeo própria (v3+, MVP usa link externo).

---

## 2. Stack

| Camada | Tecnologia | Versão | Justificativa |
|---|---|---|---|
| Runtime | .NET | 10 (LTS) | Stack alvo do projeto |
| API | ASP.NET Core (Minimal APIs) | 10 | Performance, native AOT ready |
| Orquestração dev | .NET Aspire | 13+ | Service discovery, telemetry, dashboard grátis |
| ORM | EF Core | 10 | Migrations, LINQ, owned entities |
| DB | PostgreSQL | 16 | JSONB, RLS, maduro, open source |
| Cache | Redis | 7 | Idempotência, sessões, rate limit |
| Mensageria | RabbitMQ | 3.13 | Padrão, AMQP 0.9.1, fácil em dev |
| Mensageria SDK | MassTransit | 8+ | Consumer pattern, retry, outbox, sagas |
| IdP | ASP.NET Core Identity + JWT (self-issued) | 10 | Built-in, zero infra extra, mesmo DB do Auth. Migrável para Keycloak depois. |
| Tracing | OpenTelemetry SDK | 1.10+ | Vendor-neutral |
| Logs | Serilog | 4 + OTel sink | Structured logging |
| Resiliência | Polly | 8 | Retry, circuit breaker, timeout |
| Testes | xUnit + Testcontainers | — | Integração real em container |
| Container | Docker | 24+ | Dev + prod |
| Prod orchestr. | Kubernetes + Helm | — | Padrão mercado |
| CI | GitHub Actions | — | Já no ecossistema |
| Feature flags | Microsoft.FeatureManagement | — | Built-in .NET |
| API gateway (prod) | nginx / YARP | — | TLS, rate limit, routing |

**Stack proibida (decisões ativas):**
- ❌ SQL Server (custo, lock-in).
- ❌ Kafka (overhead para o tamanho atual; RabbitMQ resolve).
- ❌ gRPC entre serviços (overhead; só síncrono pontual via HTTP+Refit).
- ❌ Orleans/Dapr (complexidade desnecessária no MVP).

---

## 3. Estrutura da Solução

```
src/
  BuildingBlocks/                          # libs compartilhadas (NuGet interno)
    BuildingBlocks.Domain/                 # Entity, AggregateRoot, ValueObject, DomainEvent
    BuildingBlocks.Application/            # ICommand, IQuery, ICommandHandler, behaviors
    BuildingBlocks.Infrastructure/         # EF Core, MassTransit, OTel, Serilog
    BuildingBlocks.API/                    # Middlewares, ProblemDetails, AuthN/AuthZ, healthchecks
    BuildingBlocks.Tests/                  # Helpers, fixtures, builders
  Core/                                    # Domínio compartilhado PsiFlow (types cross-service)
  Services/
    <Name>/
      <Name>.API/                          # Host, endpoints, DI, middleware
      <Name>.Application/                  # Use cases, validators, mappers
      <Name>.Domain/                       # Entidades, regras de negócio
      <Name>.Infrastructure/               # EF Core DbContext, repos, integração externa
      <Name>.Contracts/                    # Eventos/mensagens publicados por ESTE serviço
tests/
  Services/<Name>.Tests.Unit/
  Services/<Name>.Tests.Integration/
  Architecture.Tests/                      # NetArchTest: regras de dependência
deploy/
  docker-compose.yml
  docker-compose.override.yml
  helm/
docs/
  system-design.md                         # este arquivo
  adr/                                     # Architecture Decision Records
  api-contracts/                           # OpenAPI specs versionadas
```

**Regras de dependência (enforçadas por `Architecture.Tests` via NetArchTest).**

| Projeto | Pode depender de | Não pode depender de |
|---|---|---|
| `*.Domain` | `BuildingBlocks.Domain` | qualquer outro projeto |
| `*.Application` | `*.Domain`, `BuildingBlocks.Application` | `*.Infrastructure`, `*.API` |
| `*.Contracts` | `*.Domain` (tipos básicos) | `*.Application`, `*.Infrastructure`, `*.API` |
| `*.Infrastructure` | `*.Application`, `BuildingBlocks.Infrastructure` | `*.API`, outros `*.Domain` |
| `*.API` | `*.Application`, `*.Infrastructure`, `BuildingBlocks.API` | outros `*.Domain`, outros `*.Contracts` |

**Regra de ouro:** nenhum serviço referencia o `Domain` ou `Application` de outro serviço. Comunicação entre serviços = HTTP (consumindo OpenAPI) ou eventos (consumindo `<OutroServico>.Contracts` via NuGet).

---

## 4. Catálogo de Serviços MVP

> Status: 🟡 scaffolding | 🟢 production-ready | ⚪ planned
> Convenções: `crud` = operações padrão; `cmd` = command CQRS; `qry` = query CQRS.

### 4.1 Auth 🟡

**Responsabilidade.** Identidade, autenticação, emissão/validação de JWT, refresh token, MFA, aceite de termos, gestão de papéis e permissões. **Serviço auto-contido** — emite e valida seus próprios tokens (sem IdP externo no MVP).

**Stack interna.** ASP.NET Core Identity (user store, roles, password hashing) + `Microsoft.AspNetCore.Authentication.JwtBearer` (emissão/validação JWT com chave RSA local).

**Entidades principais.** `User` (IdentityUser estendido), `Role`, `UserRole`, `RefreshToken`, `Consent`, `MfaChallenge`.

**APIs (v1).**
- `POST /v1/auth/register` — cadastro de psicóloga/paciente.
- `POST /v1/auth/login` — login (e-mail + senha → access + refresh).
- `POST /v1/auth/refresh` — refresh token (rotation).
- `POST /v1/auth/logout` — revoga refresh.
- `GET  /v1/auth/me` — perfil + claims.
- `POST /v1/auth/consent` — registra aceite de termo.
- `POST /v1/auth/mfa/setup` — inicia setup TOTP.
- `POST /v1/auth/change-password`

**Eventos publicados.** `UserRegistered`, `UserDeactivated`, `ConsentAccepted`, `RoleAssigned`.

**Eventos consumidos.** — (serviço raiz, sem deps internas).

**DB.** `psiflow_auth` — Postgres dedicado. Identity tables (`AspNetUsers`, `AspNetRoles`, etc.) + tabelas de domínio (refresh tokens, consents, MFA).

**Status.** Estrutura Clean Arch criada; substituir template `weatherforecast` por implementação real com Identity + JWT.

**Migrabilidade.** Chave de signing em arquivo/config; trocar por JWKS de Keycloak é isolado em `BuildingBlocks.API.JwtAuth` quando for hora.

---

### 4.2 Patients ⚪

**Responsabilidade.** Cadastro, perfil administrativo, convite por link, histórico administrativo. **Não** armazena prontuário (vai em `ClinicalRecords`).

**Entidades principais.** `Patient`, `PatientInvite`, `ResponsibleLegal` (para menor), `PatientNote` (anotação administrativa, não clínica).

**APIs (v1).**
- `POST/GET/PUT/DELETE /v1/patients[/{id}]`
- `POST /v1/patients/{id}/invite` — gera link/token.
- `POST /v1/patients/invite/{token}/accept` — paciente finaliza cadastro.

**Eventos publicados.** `PatientCreated`, `PatientInvited`, `PatientAcceptedInvite`, `PatientDeactivated`.

**Eventos consumidos.** `UserRegistered` (cria/atualiza perfil).

**DB.** `psiflow_patients`.

---

### 4.3 Agenda ⚪

**Responsabilidade.** Disponibilidade, bloqueios, agendamento, cancelamento, recorrência básica.

**Entidades principais.** `Availability` (janela recorrente), `AvailabilityException` (overrides), `ScheduleBlock` (bloqueio pontual), `Appointment`.

**Regras de negócio.**
- `Appointment` exige `Availability` válida (sem conflito com `ScheduleBlock`).
- Cancelamento < 24h = permitido mas com flag `late_cancel` (relatórios).
- Recorrência: semanal (v1); outras periodicidades (v2).

**APIs (v1).**
- `CRUD /v1/availabilities`
- `CRUD /v1/blocks`
- `POST /v1/appointments` (psicóloga agenda paciente).
- `POST /v1/appointments/{id}/cancel`
- `POST /v1/appointments/{id}/reschedule`
- `GET  /v1/appointments?from=&to=`

**Eventos publicados.** `AppointmentScheduled`, `AppointmentCancelled`, `AppointmentRescheduled`, `AppointmentReminderDue`.

**Eventos consumidos.** `PatientCreated`, `PatientDeactivated`.

**DB.** `psiflow_agenda`.

**Integrações.** Sessions (cria `Session` ao confirmar), Notifications (escuta `AppointmentScheduled`, agenda lembretes T-24h e T-1h).

---

### 4.4 Sessions ⚪

**Responsabilidade.** Ciclo de vida da sessão (scheduled → in_progress → completed/no_show/canceled). Não armazena conteúdo clínico (ref a `ClinicalRecords`).

**Entidades principais.** `Session`, `SessionStatusHistory`.

**APIs (v1).**
- `GET  /v1/sessions?from=&to=`
- `POST /v1/sessions/{id}/start` — status = in_progress.
- `POST /v1/sessions/{id}/complete` — status = completed.
- `POST /v1/sessions/{id}/no-show`
- `POST /v1/sessions/{id}/cancel`

**Eventos publicados.** `SessionStarted`, `SessionCompleted`, `SessionNoShow`, `SessionCancelled`.

**Eventos consumidos.** `AppointmentScheduled`, `AppointmentCancelled`.

**DB.** `psiflow_sessions`.

---

### 4.5 ClinicalRecords ⚪

**Responsabilidade.** Prontuário, evolução de sessão, anamnese, plano terapêutico. **Serviço mais sensível** (criptografia coluna-a-coluna obrigatória).

**Entidades principais.** `MedicalRecord` (1:1 com Patient), `Evolution` (1:N com Session), `Anamnesis`, `TherapeuticPlan`.

**Regras de segurança.**
- Acesso: **somente** psicóloga responsável + admin clínica.
- Toda leitura/gravação gera `audit_log` (actor, action, IP, ts).
- **Append-only**: edições geram nova versão, não sobrescrevem.

**APIs (v1).**
- `GET/PUT /v1/records/{patientId}`
- `POST /v1/records/{patientId}/evolutions` — vincula a `Session`.
- `GET  /v1/records/{patientId}/evolutions`
- `POST /v1/records/{patientId}/anamnesis`
- `POST /v1/records/{patientId}/therapeutic-plan`

**Eventos publicados.** `RecordCreated`, `EvolutionAdded`, `AnamnesisCompleted`.

**Eventos consumidos.** `PatientCreated` (cria `MedicalRecord` em branco).

**DB.** `psiflow_clinical` — **AES-256 coluna-a-coluna** em todos os campos textuais.

---

### 4.6 Payments ⚪

**Responsabilidade.** v1: controle manual (psicóloga marca como pago). v2: integração com gateway (Asaas/STP) e webhook.

**Entidades principais.** `Payment`, `Invoice`, `PaymentMethod` (manual | pix | card).

**APIs (v1).**
- `POST /v1/payments` — registra cobrança.
- `PUT  /v1/payments/{id}/mark-paid` — manual.
- `GET  /v1/payments?patientId=&from=&to=`
- `POST /v1/payments/{id}/cancel`

**Eventos publicados.** `PaymentRegistered`, `PaymentReceived`, `PaymentOverdue`, `PaymentCancelled`.

**Eventos consumidos.** `SessionCompleted` (sugere cobrança), `SessionCancelled` (cancela cobrança).

**DB.** `psiflow_payments`.

---

### 4.7 Notifications ⚪

**Responsabilidade.** Envio centralizado de mensagens (e-mail MVP; SMS/WhatsApp/push v2+). Templates versionados, preferências de canal por usuário.

**Entidades principais.** `Template`, `NotificationLog`, `NotificationPreference`.

**Canais (v1).** E-mail via SMTP (SendGrid/SES/Resend — provider via config).

**Consumidores (handlers de eventos).**
- `AppointmentScheduled` → agenda `T-24h` e `T-1h` reminders.
- `SessionCompleted` → recibo para paciente.
- `PaymentReceived` → comprovante.
- `PaymentOverdue` → aviso.
- `UserRegistered` → boas-vindas.

**APIs (v1).**
- `GET  /v1/notifications/logs?userId=`
- `PUT  /v1/notifications/preferences`

**Eventos publicados.** `NotificationSent`, `NotificationFailed`.

**DB.** `psiflow_notifications`.

---

### 4.8 Documents ⚪

**Responsabilidade.** Termos, contratos, documentos editáveis, versionamento, link de download temporário. v1: upload PDF + versionamento. v2: assinatura digital (Clicksign/ZapSign).

**Entidades principais.** `Document` (metadata), `DocumentVersion`, `DocumentSignature`.

**Storage.** S3-compatível (MinIO em dev, AWS S3/Azure Blob em prod). Metadados no DB; blob no object storage.

**APIs (v1).**
- `POST /v1/documents` — upload + metadata.
- `GET  /v1/documents/{id}/versions`
- `GET  /v1/documents/{id}/versions/{v}/download-url` — URL pré-assinada (TTL 15min).
- `POST /v1/documents/{id}/sign` — v2.

**Eventos publicados.** `DocumentUploaded`, `DocumentSigned`, `DocumentExpired`.

**Eventos consumidos.** `PatientCreated`, `ConsentAccepted`.

**DB.** `psiflow_documents` (apenas metadata).

---

### 4.9 Billing ⚪

**Responsabilidade.** Planos da plataforma (Free/Pro/Clinic), assinaturas, cobrança SaaS à psicóloga. **Não confundir com Payments** (que cobra paciente → psicóloga).

**Entidades principais.** `Plan`, `Subscription`, `Invoice` (psicóloga → PsiFlow), `BillingEvent`.

**Integração.** Stripe ou Asaas (decidir via ADR-011).

**APIs (v1).**
- `GET  /v1/plans`
- `POST /v1/subscriptions`
- `GET  /v1/billing/invoices?from=&to=`
- `POST /v1/billing/webhook` — recebe eventos do gateway.

**Eventos publicados.** `SubscriptionCreated`, `SubscriptionCancelled`, `InvoicePaid`, `InvoiceOverdue`.

**Eventos consumidos.** `UserRegistered` (sugere plano).

**DB.** `psiflow_billing`.

---

### 4.10 OnlineSession ⚪

**Responsabilidade.** v1: gera link externo (Zoom/Google Meet). v2: sala própria WebRTC.

**Entidades principais.** `VideoRoom` (referência ao provedor externo).

**APIs (v1).**
- `POST /v1/online-sessions/{sessionId}/link` — cria/recupera link.
- `DELETE /v1/online-sessions/{id}` — encerra sala.

**Eventos publicados.** `VideoLinkCreated`, `VideoSessionStarted`, `VideoSessionEnded`.

**Eventos consumidos.** `SessionStarted` (sinaliza sala ativa).

**DB.** `psiflow_online` (metadata leve).

---

## 5. Padrões Cross-Cutting

### 5.1 Autenticação & Autorização

- **Auth.API é auto-contido**: emite e valida seus próprios JWT. **Sem IdP externo** no MVP (Keycloak = migração futura, ver ADR-003).
- **ASP.NET Core Identity** = user store + roles + password hashing (PBKDF2/Argon2 via config).
- **JWT self-issued**: Auth.API assina com **RSA 2048** (chave em arquivo, rotacionável). Outros serviços validam via **JWKS endpoint** do Auth.API (`/.well-known/jwks.json`) com cache local.
- **Claims esperadas:**
  - `sub` — user ID (ULID/UUID).
  - `tenant_id` — psicóloga_id OU clínica_id (raiz do isolamento).
  - `roles[]` — `psychologist`, `patient`, `clinic_admin`, `saas_admin`.
  - `scopes[]` — permissões granulares.
  - `email`, `email_verified`.
- **Access token**: 15min. **Refresh token**: 7 dias, rotation (cada refresh emite novo e revoga o anterior), armazenado como hash no DB.
- **Policy-based authorization** centralizada em `BuildingBlocks.API.Authorization`:
  - `[Authorize(Policy = "PsychologistOnly")]`
  - `[Authorize(Policy = "SameTenant", Resource = "patientId")]`
- **Multi-tenancy** via `ITenantContext` injetado, validado em todo endpoint.
- **BFF pattern** recomendado no front (não no backend) — evita expor access token ao browser.

### 5.2 Comunicação Entre Serviços

| Necessidade | Padrão | Ferramenta |
|---|---|---|
| Leitura imediata de outro serviço | HTTP síncrono | Refit + service discovery |
| Comando que precisa de feedback imediato | HTTP + idempotency-key | Refit + Polly |
| Evento de domínio (1:N) | Mensageria assíncrona | MassTransit + RabbitMQ |
| Long-running workflow | Saga | MassTransit Saga |
| Streaming/event sourcing | (não usar no MVP) | — |

**Regras.**
- **Síncrono** só se a operação **precisa** do resultado para continuar. Senão, assíncrono.
- **Eventos** carregam **fatos do passado** (passado: `AppointmentScheduled`, não imperativo: `ScheduleAppointment`).
- Contratos versionados em `BuildingBlocks.Contracts` (NuGet semver).
- `CorrelationId` em toda mensagem (W3C traceparent).

### 5.3 Persistência

- **1 banco por serviço** (Postgres isolado). Nunca shared DB.
- **EF Core 10** com migrations por serviço em `<Service>.Infrastructure/Migrations`.
- **Outbox pattern**: mensagem + mudança no DB = 1 transação. Dispatcher separado envia para RabbitMQ.
- **Soft delete** para entidades clínicas (auditoria). Hard delete só com `Purge` explícito + auditoria.
- **Global query filter** para tenant isolation: `WHERE tenant_id = @current`.
- **Row-Level Security** (Postgres RLS) como **defesa em profundidade** (não substitui o filtro EF).
- **Naming convention**: snake_case no DB, PascalCase no C#. EF converte via snake_case extension.

### 5.4 Observabilidade

- **OpenTelemetry** em todos os serviços (auto-instrumentation para HTTP, EF Core, MassTransit).
- **Traces** com W3C `traceparent` propagado entre serviços.
- **Métricas**:
  - `http.server.request.duration` (histogram).
  - `db.client.commands` (counter, success/failure).
  - `messaging.published.count`, `messaging.consumed.count`.
  - `tenant.requests` (counter, por tenant).
- **Logs** Serilog → console JSON (dev) / OTel → Loki (prod). Correlacionados por `traceId`.
- **Health checks**:
  - `/health/live` — processo vivo.
  - `/health/ready` — DB up, deps up.
  - `/health/startup` — migrations aplicadas, DB OK, chave de signing carregada.
- **Dev:** Aspire Dashboard (traces + logs + metrics + service map).
- **Prod:** OTel Collector → Grafana Tempo + Prometheus + Loki.

### 5.5 Resiliência

- **Polly v8** policies em todos os clients HTTP e consumers:
  - **Retry**: exponential backoff + jitter, 3 tentativas.
  - **Circuit breaker**: 5 falhas em 30s → abre por 60s.
  - **Timeout**: 5s síncrono. Assíncrono sem timeout (eventos podem demorar).
- **Idempotency-Key** header em todos os POSTs que criam recurso (`payments`, `appointments`, `documents`).
- **Idempotência no consumer** via `MessageId` deduplicado em Redis (TTL 7 dias).

### 5.6 Configuração

- **12-factor**: env vars para secrets, appsettings para defaults.
- **.NET Aspire AppHost** orquestra dev: Postgres, Redis, RabbitMQ, todos os serviços, com service discovery automático.
- **Feature flags** via `Microsoft.FeatureManagement` (flag por tenant ou global).
- **Perfis:** `Development`, `Staging`, `Production`. Nunca hardcode URL/credenciais.

### 5.7 Estilo de API

- **REST + JSON** (não gRPC, não GraphQL no MVP).
- **Versionamento** na URL: `/v1/`, `/v2/`. Nunca quebrar contrato v1.
- **ProblemDetails (RFC 7807)** para erros.
- **OpenAPI** gerado automaticamente + customizado, commitado em `docs/api-contracts/<service>-v1.yaml`.
- **Idempotency-Key** em POSTs mutantes.
- **Rate limit** por tenant (1000 req/min default, ajustável por plano).

### 5.8 Versionamento de Mensagens

- Eventos em `<Servico>.Contracts/Events/<Domain>/<Verb>edEvent.cs`.
- Mudança **breaking** = novo nome (`AppointmentScheduledV2`). Não mutar em lugar.
- Adicionar campo opcional = compatível. Remover campo = breaking.
- Consumer ignora campos desconhecidos (configurar MassTransit).

---

## 6. Segurança & LGPD

> PsiFlow lida com **dados sensíveis de saúde**. Esta seção não é opcional.

### 6.1 Camadas de Criptografia

| Camada | Mecanismo | Implementação |
|---|---|---|
| In-transit | TLS 1.3 obrigatório | cert-manager (Let's Encrypt) em prod |
| At-rest (disco) | AES-256 | default Postgres + provider cloud |
| At-rest (PII coluna) | AES-256-GCM por tenant | `BuildingBlocks.Infrastructure.Encryption` |
| Senhas | PBKDF2 (default Identity) / Argon2id (configurável) | ASP.NET Core Identity hasher |
| Secrets | Vault / KMS | AWS Secrets Manager / Azure Key Vault via External Secrets Operator |
| Backups | Criptografados | Provider cloud default |

### 6.2 PII & Dados Sensíveis

**Criptografado coluna-a-coluna** (com chave por tenant + chave mestra no KMS):
- `patient.name`, `patient.cpf`, `patient.email`, `patient.phone`, `patient.address`
- `record.*` (todo o prontuário)
- `evolution.content`
- `anamnesis.content`
- `therapeutic_plan.content`
- `document.content` (texto extraído via OCR)

**Mascaramento em logs**: email, telefone, CPF, ID de paciente, conteúdo clínico — nunca em log.

**Hash**: senha (PBKDF2 via Identity, ou Argon2id trocando o `PasswordHasher`).

### 6.3 Auditoria

- **Append-only audit log** em schema `audit` dedicado por serviço.
- Captura: `actor_id`, `actor_role`, `action` (read/create/update/delete), `resource_type`, `resource_id`, `ip`, `user_agent`, `correlation_id`, `timestamp`, `result` (allowed/denied).
- **Retenção:** 5 anos (CFP + LGPD).
- **Acesso:** apenas `saas_admin` e `clinic_admin` (escopo da clínica).
- **Exportável** para PDF/CSV sob demanda (LGPD art. 18, V).

### 6.4 Consentimento

- **Termo de consentimento versionado** (hash + ts + IP de aceite).
- Paciente pode revogar (LGPD art. 18, VI). Revogação = `deactivate`, **não** delete (prontuário mantido por obrigação CFP, mas inacessível à paciente).
- Mudança de termo = re-aceite obrigatório no próximo login.

### 6.5 Retenção

| Dado | Retenção | Base legal |
|---|---|---|
| Prontuário | 5 anos após último atendimento | CFP Res. 010/2005 |
| Billing (PsiFlow → Psicóloga) | 5 anos | Fiscal |
| Billing (Psicóloga → Paciente) | 5 anos | Fiscal |
| Audit log | 5 anos | LGPD + CFP |
| Logs de aplicação | 90 dias | Operacional |
| Marketing/comunicação | Até revogação | LGPD art. 18, VI |

### 6.6 Direito ao Esquecimento

- Endpoint `POST /v1/gdpr/forget-me` (autenticado como paciente).
- Anonimiza campos pessoais (substitui por hash irreversível).
- Mantém **registro mínimo da existência** do atendimento (data, hash da psicóloga) — obrigação CFP.
- Audit log da própria anonimização é preservado.

### 6.7 Isolamento por Tenant

- **Tenant raiz = Psicóloga** (ou Clínica, que agrega psicólogas).
- **Row-Level Security** no Postgres (`USING tenant_id = current_setting('app.tenant_id')`).
- **EF Core global query filter** injeta `tenant_id` automaticamente.
- **Cross-tenant access:** bloqueado por padrão. Exceção: `saas_admin` com `justification` obrigatório + audit log.
- **Teste de penetração mensal** automatizado: tenta acessar tenant A com token de tenant B, espera 403.

### 6.8 Outras Proteções

- **HTTPS-only** (HSTS, no `http://` em prod).
- **CSP** e headers de segurança (`X-Content-Type-Options`, `X-Frame-Options`, `Referrer-Policy`).
- **Rate limit** agressivo em endpoints sensíveis (`/auth/*`).
- **2FA** obrigatório para `saas_admin` e `clinic_admin`. Opcional para psicóloga. Paciente pode escolher.
- **Session timeout:** access token 15min, refresh token 7 dias, sliding.
- **Bloqueio de conta** após 5 tentativas falhas (Identity lockout, configurável).

---

## 7. Topologia de Deploy

### 7.1 Dev (local)

```
┌────────────────────────────────────────────────────────────┐
│  .NET Aspire AppHost                                       │
│                                                            │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐         │
│  │  Postgres   │  │    Redis    │  │  RabbitMQ   │         │
│  │  (container)│  │ (container) │  │ (container) │         │
│  └─────────────┘  └─────────────┘  └─────────────┘         │
│                                                            │
│  ┌─────────────┐                                           │
│  │  (Auth.API é auto-contido: Identity + JWT, sem IdP)  │  │
│                                                            │
│  ┌──────────────────────────────────────────────────┐      │
│  │  Auth.API   │  Patients.API  │  Agenda.API  │ ... │      │
│  └──────────────────────────────────────────────────┘      │
│                                                            │
│  Aspire Dashboard: http://localhost:15000                  │
└────────────────────────────────────────────────────────────┘
```

- `dotnet aspire run` sobe tudo.
- Hot reload nativo.
- Service discovery automático (Aspire resolve DNS entre serviços).
- Seed de dados de dev no `psiflow_auth` (1 psicóloga, 1 paciente, 1 admin).

### 7.2 Prod

```
                            ┌──────────────┐
                            │   CloudFlare │
                            │   (CDN+WAF)  │
                            └──────┬───────┘
                                   │
                            ┌──────▼───────┐
                            │   Ingress    │
                            │ nginx + TLS  │
                            └──────┬───────┘
                                   │
         ┌──────────────┬───────────┼───────────┬──────────────┐
         │              │           │           │
    ┌────▼────┐   ┌─────▼─────┐ ┌───▼────┐ ┌────▼────┐
    │Auth.API │   │Patients.  │ │Agenda. │ │ ...     │
   │  (k8s)  │   │API (k8s)  │ │API(k8s)│ │         │
   └────┬────┘   └─────┬─────┘ └───┬────┘ └────┬────┘
        │              │           │           │
   ┌────▼────┐   ┌─────▼─────┐ ┌───▼────┐ ┌────▼────┐
   │psql-auth│   │psql-pat.  │ │psql-ag.│ │  ...    │
   └─────────┘   └───────────┘ └────────┘ └─────────┘
        │
   ┌────▼──────────────┐  ┌─────────┐  ┌─────────┐
   │ OTel Collector    │  │Prometheus│  │  Loki   │
   └───────────────────┘  └─────────┘  └─────────┘
            │
   ┌────────▼──────────┐  ┌──────────┐
   │   Grafana         │  │  Tempo   │
   └───────────────────┘  └──────────┘
```

- **k8s** (EKS/AKS/GKE) com Helm charts por serviço.
- **HPA** baseado em CPU + custom metric (req/s).
- **PDB** (Pod Disruption Budget) mínimo 1 réplica sempre.
- **Secrets via External Secrets Operator** → AWS Secrets Manager / Azure Key Vault.
- **Backups:** PITR do Postgres (point-in-time recovery, 7 dias).
- **Multi-AZ** obrigatório em prod.

---

## 8. Decisões Arquiteturais (ADRs)

ADRs em `docs/adr/`, formato MADR. Status atual:

| # | Título | Status |
|---|---|---|
| [ADR-001](adr/001-microservices.md) | Microsserviços (vs monólito modular) | ✅ Accepted |
| [ADR-002](adr/002-clean-architecture.md) | Clean Architecture por serviço | ✅ Accepted |
| [ADR-003](adr/003-aspnet-identity.md) | ASP.NET Core Identity + JWT self-issued (sem IdP externo) | ✅ Accepted |
| [ADR-004](adr/004-postgres-per-service.md) | PostgreSQL por serviço (1 DB cada) | ✅ Accepted |
| [ADR-005](adr/005-rabbitmq-masstransit.md) | RabbitMQ + MassTransit | ✅ Accepted |
| [ADR-006](adr/006-outbox-pattern.md) | Outbox pattern para mensageria | ✅ Accepted |
| [ADR-007](adr/007-aspire-dev.md) | .NET Aspire para dev | ✅ Accepted |
| [ADR-008](adr/008-tenant-psychologist.md) | Tenant = Psicóloga (multi-tenant) | ✅ Accepted |
| [ADR-009](adr/009-soft-delete-audit.md) | Soft-delete clínico + auditoria imutável | ✅ Accepted |
| [ADR-010](adr/010-pii-column-encryption.md) | Criptografia coluna-a-coluna para PII | ✅ Accepted |
| [ADR-011](adr/011-billing-gateway.md) | Gateway de pagamento SaaS (Stripe vs Asaas) | ⚪ Pending |
| [ADR-012](adr/012-bff-pattern.md) | BFF pattern para frontend | ⚪ Pending |

---

## 9. Glossário (Ubiquitous Language)

> Termos canônicos do domínio. Use estes em código, comentários, PRs e conversas.

| Termo | Definição |
|---|---|
| **Psicóloga** | Profissional com CRP ativo, tenant raiz do sistema. |
| **Paciente** | Pessoa em atendimento (vinculada a 1+ Psicólogas). |
| **Clínica** | Organização que agrega N Psicólogas (tenant agregador). |
| **Admin Clínica** | Usuário que gerencia a Clínica (não é Psicóloga). |
| **Admin SaaS** | Equipe PsiFlow (suporte, billing, auditoria global). |
| **Sessão** | 1 atendimento realizado ou agendado. Status: `scheduled`, `in_progress`, `completed`, `no_show`, `canceled`. |
| **Atendimento** | Sinônimo de Sessão (foco na perspectiva do paciente). |
| **Agendamento (Appointment)** | Reserva de horário (vira Sessão ao iniciar). |
| **Disponibilidade (Availability)** | Janela recorrente em que a Psicóloga atende. |
| **Bloqueio (ScheduleBlock)** | Janela indisponível (férias, compromisso). |
| **Prontuário (MedicalRecord)** | Registro clínico completo do Paciente (1:1). |
| **Evolução (Evolution)** | Nota clínica feita após uma Sessão. |
| **Anamnese** | Histórico inicial do Paciente (entrevista de entrada). |
| **Plano Terapêutico** | Objetivos, estratégias e cronograma do tratamento. |
| **Convite (PatientInvite)** | Link/token para Paciente se cadastrar vinculado a Psicóloga. |
| **Consentimento** | Aceite formal de termo (LGPD), versionado. |
| **CRP** | Conselho Regional de Psicologia — registro profissional. |
| **Plano (SaaS)** | Assinatura da plataforma (Free/Pro/Clinic). |
| **Tenant** | Unidade de isolamento de dados (Psicóloga ou Clínica). |
| **Pagamento (Payment)** | Transação financeira Psicóloga ↔ Paciente. |
| **Cobrança (Invoice)** | Comprovante de pagamento. |
| **Lembrete (Reminder)** | Mensagem automática antes de Agendamento. |
| **Audit Log** | Registro imutável de acesso/modificação (LGPD). |
| **PII** | Personally Identifiable Information — dado pessoal identificável. |

---

## 10. Roadmap de Implementação

Ordem sugerida (tracer-bullet vertical slices):

1. **Auth** — base para tudo. Substituir template `weatherforecast` por ASP.NET Identity + JWT. ✅ Começar aqui.
2. **Patients** — depende de Auth. Global query filter de tenant.
3. **Agenda** — depende de Patients. Adiciona complexidade de slots/recorrência.
4. **Sessions** — depende de Agenda. Introduz máquina de estados.
5. **ClinicalRecords** — depende de Sessions. Criptografia coluna-a-coluna. **Mais sensível.**
6. **Notifications** — depende de Agenda. Primeiro consumidor de eventos.
7. **Documents** — depende de Auth. Storage S3 + versionamento.
8. **Payments** — depende de Sessions. v1 manual, v2 gateway.
9. **Billing** — depende de Auth. Cobra a Psicóloga.
10. **OnlineSession** — depende de Sessions. Última do MVP.

**Fora do MVP (v2+).**
- Marketplace público de Psicólogas.
- Multi-idioma / multi-moeda.
- Sala de vídeo própria (WebRTC).
- Mobile nativo / PWA completo.
- Analytics avançado (predição de falta, churn).
- Templates por abordagem terapêutica.

---

## 11. Riscos & Mitigações

| Risco | Probabilidade | Impacto | Mitigação |
|---|---|---|---|
| Vazamento de dados clínicos | Baixa | Catastrófico | Criptografia + auditoria + pentest mensal |
| Indisponibilidade do Auth.API | Média | Alta | 2+ réplicas + cache JWKS local + health check |
| Explosão de serviços (overhead operacional) | Média | Média | Service template copy-paste + Aspire |
| Lock-in de provider cloud | Baixa | Média | Abstração de storage (S3-compatível) e secrets |
| Mudança regulatória CFP/LGPD | Média | Média | ADRs + time jurídico revisando releases |
| Performance do tenant filter | Média | Média | Índices em `tenant_id`, RLS Postgres, monitorar |
| Acoplamento entre serviços via contratos | Média | Média | Versionamento semver + testes de contrato (Pact) |
| Sem admin UI de IdP (gestão de users/roles manual via API) | Média | Baixa | Endpoints `/v1/users`, `/v1/roles` no Auth. Migrar pra Keycloak se virar gargalo (ADR-003). |

---

## 12. Referências

- Documento de produto: [`docs/documentacao-produto-saas-psicologas.md`](documentacao-produto-saas-psicologas.md)
- ADRs: [`docs/adr/`](adr/)
- Clean Architecture (Uncle Bob): https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html
- LGPD: https://www.gov.br/cidadania/pt-br/acesso-a-informacao/lgpd
- CFP Res. 010/2005 (prontuário): https://site.cfp.org.br/
- .NET Aspire: https://learn.microsoft.com/dotnet/aspire/
- MassTransit: https://masstransit.io/
- OpenTelemetry: https://opentelemetry.io/

---

> **Mantenedor:** Engenharia PsiFlow.
> **Próxima revisão:** 2026-09-01 ou após decisão de Billing gateway (ADR-011).
