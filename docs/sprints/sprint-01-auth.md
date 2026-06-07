# Sprint 1 — Auth real (substituir template `weatherforecast`)

> Primeira sprint do MVP do PsiFlow.
> Objetivo: ter o serviço **Auth** funcionando ponta-a-ponta, validando que toda a cadeia (Identity + JWT + endpoints + DB + observabilidade) está de pé antes dos outros serviços nascerem.
>
> **Período-alvo:** 2026-06-09 → 2026-06-20 (2 semanas, conforme cronograma do MVP §15)
> **Marco:** Auth real pronto em **2026-06-18**
> **Referências:** [`2026-06-04-mvp-validation-design.md`](../2026-06-04-mvp-validation-design.md) §4.1, §6, §8 · [`system-design.md`](../system-design.md) §4.1, §5.1, §6 · [`documentacao-produto-saas-psicologas.md`](../documentacao-produto-saas-psicologas.md) RF001-RF010

---

## 1. Sprint Goal

Substituir o template `weatherforecast` do `Auth.API` por um serviço de autenticação **real, seguro e auto-contido**, capaz de:

1. Cadastrar psicóloga, paciente e saas_admin com aceite versionado de termos (LGPD).
2. Emitir e validar JWT self-issued (RSA 2048) + refresh token rotation.
3. Suportar MFA opcional (TOTP) para psicólogas.
4. Bloquear conta após 5 tentativas falhas.
5. Publicar evento `UserRegistered` para os próximos serviços consumirem.

Tudo entregue com **migrations, seed, OTel, health checks e testes de integração** rodando.

---

## 2. Fora do escopo desta sprint

Para evitar scope creep — estes ficam para sprints futuras:

- Patients, Agenda, Sessions, ClinicalRecords, Notifications, OnlineSession (sprints 2+).
- Recuperação de senha por e-mail real (depende de Notifications — sprint 7-8 do MVP). Endpoint existe, mas dispara só evento; envio físico fica mockado.
- Confirmação de e-mail real (idem acima).
- Login social (Google/Apple) — não está no MVP.
- 2FA obrigatório (MFA é opcional no piloto, §2.2 do MVP).
- Multi-tenancy avançado (RLS Postgres + global query filter): claim `tenant_id` no JWT já entra, mas RLS vai junto na sprint 5 (ClinicalRecords).
- Admin UI para gerenciar users/roles — gestão por API no piloto (§11 do system-design risco "Sem admin UI de IdP").

---

## 3. Definition of Done (Sprint 1)

A sprint só fecha quando **todas** estas condições forem verdadeiras:

- [ ] `Auth.API` sobe sem o template `weatherforecast`.
- [ ] Todos os 8 endpoints `/v1/auth/*` retornam 2xx no happy path e 4xx com `ProblemDetails` no erro.
- [ ] Migrations rodam no startup (`db.Database.Migrate()`).
- [ ] Seed cria: 1 saas_admin, 0 psicólogas, 0 pacientes, 21 PermissionGroups com 84 Permissions, 3 Roles (`psychologist`, `patient`, `saas_admin`).
- [ ] JWT assinado com RSA 2048, JWKS publicado em `/.well-known/jwks.json`.
- [ ] Access token = 15min, refresh = 7d com rotation.
- [ ] Lockout após 5 tentativas inválidas (5min).
- [ ] OpenTelemetry exporta traces + metrics; Serilog em JSON estruturado.
- [ ] Health checks `/health/live`, `/health/ready`, `/health/startup` respondem.
- [ ] Testes de integração com Testcontainers cobrem signup → login → refresh → logout → me.
- [ ] OpenAPI gerado e commitado em `docs/api-contracts/auth-v1.yaml`.
- [ ] Latência p95 < 500ms em `/v1/auth/login` rodando local (validação smoke).

---

## 4. Backlog da sprint

> Legenda de esforço: **S** ≤ 2h · **M** ≤ 1d · **L** ≤ 2-3d
> Marque `[x]` ao concluir. Itens já marcados estão prontos (verificado em código no início da sprint).

### 4.1 Infraestrutura & setup do projeto

- [x] **(S)** Estrutura Clean Architecture criada (`Auth.API` / `Auth.Application` / `Auth.Domain` / `Auth.Infrastructure`).
- [x] **(S)** Refatoração: pasta `Aggregates/` no `Auth.Domain` com `PermissionGroup`, `Permission`, `User`.
- [x] **(S)** Seed de `PermissionGroup` alinhado ao MVP (21 grupos, 84 permissões).
- [x] **(S)** Adicionar NuGets ao `Auth.Infrastructure`: `Microsoft.AspNetCore.Identity.EntityFrameworkCore`, `Npgsql.EntityFrameworkCore.PostgreSQL`, `Microsoft.EntityFrameworkCore.Design`.
- [ ] **(S)** Adicionar NuGets ao `Auth.API`: `Microsoft.AspNetCore.Authentication.JwtBearer`, `Serilog.AspNetCore`, `OpenTelemetry.Extensions.Hosting`, `OpenTelemetry.Exporter.OpenTelemetryProtocol`.
- [ ] **(S)** Configurar `appsettings.json` com `ConnectionStrings:Auth`, `Jwt:Issuer`, `Jwt:Audience`, `Jwt:KeyPath`, `Identity:Lockout:MaxFailedAccessAttempts=5`, `Identity:Lockout:DefaultLockoutTimeSpan=00:05:00`.
- [ ] **(S)** Configurar `appsettings.Development.json` com chave de signing local (não commitar `.pem` em prod).
- [ ] **(S)** Atualizar `Auth.Infrastructure/DependencyInjection.cs` para registrar `DbContext`, `Identity`, repositórios, services.
- [ ] **(S)** Atualizar `Auth.Application/DependencyInjection.cs` para registrar handlers, validators e mappers.

### 4.2 Domínio — entidades faltantes

- [x] **(M)** `User : IdentityUser<UserId>` com `Name`, `Contact`, audit fields.
- [x] **(M)** `PermissionGroup` (AggregateRoot) e `Permission` (entidade-filha).
- [ ] **(M)** Criar agregado `RefreshToken` em `Auth.Domain/Aggregates/RefreshToken.cs` com: `Id`, `UserId`, `TokenHash`, `ExpiresAt`, `RevokedAt`, `ReplacedByTokenHash`, `CreatedByIp`.
- [ ] **(M)** Criar agregado `Consent` em `Auth.Domain/Aggregates/Consent.cs` com: `Id`, `UserId`, `DocumentType` (terms_of_use / privacy_policy), `DocumentVersion`, `AcceptedAt`, `AcceptedFromIp`, `DocumentHash`.
- [ ] **(M)** Criar agregado `MfaChallenge` em `Auth.Domain/Aggregates/MfaChallenge.cs` com: `Id`, `UserId`, `SharedSecret` (cifrado), `ConfirmedAt`, `RecoveryCodesHash[]`.
- [ ] **(S)** ValueObjects: `RefreshTokenId`, `ConsentId`, `MfaChallengeId`, `Crp` (validação básica: padrão `\d{2}/\d{4,6}`).
- [ ] **(S)** Eventos de domínio: `UserRegisteredEvent`, `UserDeactivatedEvent`, `ConsentAcceptedEvent`, `RoleAssignedEvent`, `MfaEnabledEvent`.

### 4.3 Persistência

- [x] **(S)** `ApplicationDbContext : IdentityDbContext<User, IdentityRole<UserId>, UserId>`.
- [x] **(S)** Configurations EF para `User`, `Permission`, `PermissionGroup`.
- [ ] **(M)** Configurations EF para `RefreshToken`, `Consent`, `MfaChallenge` (incluindo índices em `UserId` + `ExpiresAt`).
- [ ] **(S)** Trocar provider para Postgres no `DependencyInjection.cs` (`UseNpgsql`).
- [ ] **(M)** Gerar migration inicial: `dotnet ef migrations add InitialAuthSchema -p Auth.Infrastructure -s Auth.API`.
- [ ] **(S)** Configurar startup hook em `Auth.API/Program.cs` para rodar `db.Database.Migrate()` + seed de PermissionGroups + seed de Roles + seed de saas_admin (apenas em dev/staging).
- [ ] **(S)** Adicionar seed de Roles (`RoleSeed.cs`): `psychologist`, `patient`, `saas_admin`.
- [ ] **(S)** Adicionar seed de saas_admin (`AdminUserSeed.cs`): cria user padrão de dev com senha vinda de env var.

### 4.4 Application — services & handlers

- [x] **(S)** Contratos: `IAuthService`, `IUserService`, `IRoleService`, `ITokenService`, `IPermissionService`.
- [x] **(S)** DTOs de Auth, Users, Roles, Permissions, Token.
- [x] **(S)** `ChangePassword` command handler (stub).
- [ ] **(L)** Implementar `AuthService` cobrindo: `SignUpAsync`, `LoginAsync`, `RefreshAsync`, `LogoutAsync`, `ConfirmEmailAsync`, `SendPasswordResetAsync`, `ResetPasswordAsync`, `ChangePasswordAsync`.
- [ ] **(M)** Implementar `TokenService`: `GenerateAccessToken`, `GenerateRefreshToken`, `ValidateRefreshToken`, `RevokeRefreshToken`, com hash do refresh persistido.
- [ ] **(M)** Implementar `UserService`: `GetCurrentUserAsync`, `UpdateProfileAsync`, `DeactivateAsync`.
- [ ] **(M)** Implementar `RoleService`: assign/remove/update user roles.
- [ ] **(M)** Implementar `PermissionService`: `AddPermissionToUser/Role`, `Remove`, `HasPermissionAsync`, `GetCurrentUserPermissionsAsync` (devolve claims serializadas).
- [ ] **(M)** Implementar `ConsentService` (novo): `RecordConsentAsync`, `GetActiveConsentsAsync`.
- [ ] **(M)** Implementar `MfaService` (novo): `SetupAsync` (gera secret + qrcode uri), `ConfirmAsync`, `ValidateAsync`, `GenerateRecoveryCodesAsync`.
- [ ] **(S)** FluentValidation para todos os DTOs (signup exige CRP para role=psychologist; senha mínima 12 chars + maiúscula + número + símbolo).
- [ ] **(S)** Mapeadores DTO↔Domain via `UserMapper` (extender o existente).

### 4.5 API — endpoints `/v1/auth/*`

> Todos com `ProblemDetails` no erro, `Idempotency-Key` em POSTs mutantes, rate-limit `10 req/min/IP` em `/login`, `/register`, `/password-reset`.

- [ ] **(S)** `POST /v1/auth/register` — cria user, aceita termos, dispara `UserRegistered`.
- [ ] **(S)** `POST /v1/auth/login` — credenciais → access + refresh. Se MFA habilitado, devolve `MfaChallenge` token (sem access).
- [ ] **(S)** `POST /v1/auth/login/mfa` — completa login com TOTP code.
- [ ] **(S)** `POST /v1/auth/refresh` — refresh rotation.
- [ ] **(S)** `POST /v1/auth/logout` — revoga refresh token atual.
- [ ] **(S)** `GET  /v1/auth/me` — retorna perfil + claims.
- [ ] **(S)** `POST /v1/auth/consent` — registra novo aceite (mudança de versão de termo).
- [ ] **(S)** `POST /v1/auth/mfa/setup` — inicia setup TOTP, retorna secret + qrcode uri.
- [ ] **(S)** `POST /v1/auth/mfa/confirm` — confirma setup com primeiro código válido.
- [ ] **(S)** `POST /v1/auth/mfa/disable` — desabilita MFA (requer senha).
- [ ] **(S)** `POST /v1/auth/change-password` — autenticado, exige senha atual.
- [ ] **(S)** `POST /v1/auth/password/forgot` — dispara evento `PasswordResetRequested` (envio físico mockado nesta sprint).
- [ ] **(S)** `POST /v1/auth/password/reset` — usa token + nova senha.
- [ ] **(S)** Remover endpoints `weatherforecast*` do `Program.cs`.

### 4.6 Segurança & autorização

- [ ] **(M)** Gerar par de chaves RSA 2048 local (script PowerShell em `tools/generate-jwt-keys.ps1`).
- [ ] **(M)** Configurar `AddAuthentication().AddJwtBearer(...)` com signing key RSA carregada do `.pem`.
- [ ] **(M)** Expor `GET /.well-known/jwks.json` (endpoint público, sem auth).
- [ ] **(S)** Claims no JWT: `sub`, `tenant_id` (= user_id no MVP), `roles[]`, `email`, `email_verified`, `mfa_used` (bool).
- [ ] **(S)** Policies em `BuildingBlocks.API.Authorization` (criar projeto se ainda não existir): `PsychologistOnly`, `PatientOnly`, `SaasAdminOnly`, `Authenticated`.
- [ ] **(S)** Aplicar `[Authorize]` nos endpoints `/me`, `/consent`, `/mfa/*`, `/change-password`, `/logout`.
- [ ] **(S)** Habilitar `Identity.Lockout` (5 tentativas / 5min) e testar manualmente.
- [ ] **(S)** Configurar `PasswordOptions` no Identity (min 12, requer upper + digit + non-alphanumeric).
- [ ] **(S)** Habilitar HSTS, HTTPS redirect, `X-Content-Type-Options: nosniff`, `Referrer-Policy: no-referrer`.

### 4.7 Mensageria (Outbox + RabbitMQ)

> Outbox é só esqueleto nesta sprint — consumidores reais aparecem na sprint 2 (Patients).

- [ ] **(M)** Adicionar `MassTransit` + `MassTransit.RabbitMQ` ao `Auth.Infrastructure`.
- [ ] **(M)** Configurar MassTransit com outbox EF habilitado.
- [ ] **(S)** Criar projeto `Auth.Contracts` se ainda não existir, com events `UserRegistered`, `UserDeactivated`, `ConsentAccepted`, `RoleAssigned`.
- [ ] **(S)** Publicar `UserRegistered` no fim do `AuthService.SignUpAsync` (via outbox).
- [ ] **(S)** Smoke test: subir RabbitMQ local via Aspire e validar que mensagem aparece na queue.

### 4.8 Observabilidade

- [ ] **(M)** Configurar Serilog em `Program.cs` com sink Console JSON + enricher de `traceId`, `userId`, `tenantId`.
- [ ] **(M)** Configurar OpenTelemetry: traces (HTTP + EF + MassTransit), metrics (HTTP + custom), exportador OTLP.
- [ ] **(S)** Adicionar mascaramento em logs: `email`, `cpf`, `phone`, `password_hash`, `refresh_token` nunca em log.
- [ ] **(S)** Health checks: `/health/live` (process up), `/health/ready` (DB + RabbitMQ up), `/health/startup` (migrations OK + chave RSA carregada).
- [ ] **(S)** Métrica custom: `auth_login_attempts_total{result=success|failure|locked|mfa_required}`.
- [ ] **(S)** Métrica custom: `auth_active_refresh_tokens` (gauge).

### 4.9 Testes

- [ ] **(M)** Setup do projeto `Auth.Tests.Unit` com xUnit + FluentAssertions.
- [ ] **(M)** Setup do projeto `Auth.Tests.Integration` com Testcontainers (Postgres + RabbitMQ).
- [ ] **(M)** Testes unitários: `User` aggregate (validações), `Consent`, `RefreshToken` (rotation), `MfaChallenge`.
- [ ] **(M)** Testes unitários: `TokenService` (geração + validação + rotation), `AuthService` (signup edge cases).
- [ ] **(L)** Testes de integração — happy paths:
  - [ ] register → email confirma → login → me → refresh → logout
  - [ ] register psicóloga → setup MFA → login com MFA
  - [ ] change password autenticada
  - [ ] consent versionado (re-aceite ao mudar versão)
- [ ] **(L)** Testes de integração — sad paths:
  - [ ] 5 tentativas falhas → lockout
  - [ ] refresh com token revogado → 401
  - [ ] refresh com token expirado → 401
  - [ ] signup com CRP inválido (psicóloga) → 400
  - [ ] signup sem aceite de termos → 400
  - [ ] cross-tenant em endpoints futuros (preparar fixture).
- [ ] **(S)** Adicionar `Architecture.Tests` (NetArchTest) com regras do system-design §3.

### 4.10 Documentação & entrega

- [ ] **(S)** Gerar OpenAPI via Swashbuckle e commitar em `docs/api-contracts/auth-v1.yaml`.
- [ ] **(S)** Atualizar `README.md` do `Auth.API` com: como subir local, como gerar chaves, env vars necessárias, credenciais de seed.
- [ ] **(S)** Atualizar `system-design.md` §4.1: mudar status de Auth de "🟡 scaffolding" para "🟢 production-ready".
- [ ] **(S)** Criar ADR-013 "Lockout policy + password policy do MVP" em `docs/adr/013-auth-lockout-password-policy.md`.
- [ ] **(S)** Criar ADR-014 "Estrutura de claims JWT do PsiFlow" em `docs/adr/014-jwt-claims-shape.md`.
- [ ] **(S)** Demo gravada (loom/screenshare): fluxo register → login → me → refresh → MFA.

---

## 5. Critérios de aceite mapeados (do MVP §6)

Validação final da sprint, com referência direta aos critérios do MVP:

| Critério MVP (§6) | Como validar | OK? |
|---|---|---|
| Cadastro funciona em < 2s p95 | Bench local com k6/wrk em `/register` | [ ] |
| Login funciona em < 2s p95 | Bench local em `/login` | [ ] |
| Refresh funciona em < 2s p95 | Bench local em `/refresh` | [ ] |
| Logout funciona em < 2s p95 | Bench local em `/logout` | [ ] |
| MFA opcional funciona | Teste integração + demo manual | [ ] |
| Aceite de termos versionado | Teste integração: aceitar v1, subir v2, validar re-aceite | [ ] |
| Recuperação de senha funciona | Teste integração (envio mockado, token funcional) | [ ] |
| Bloqueio após 5 tentativas falhas | Teste integração: 5 logins ruins → 423 Locked | [ ] |
| JWT válido por 15min | Teste integração: token expira em 15min ± 5s | [ ] |
| Refresh válido por 7d com rotation | Teste integração: refresh emite novo + revoga anterior | [ ] |

---

## 6. Riscos da sprint

| Risco | Probabilidade | Impacto | Mitigação |
|---|---|---|---|
| Identity + UserId customizado (struct) tem fricção com EF/Identity | Média | M | Já tem `UserConfiguration` mapeando conversion; testar cedo no signup |
| Geração e rotação de chave RSA local atrapalha onboarding de devs | Média | S | Script `tools/generate-jwt-keys.ps1` + `.gitignore` para `.pem` + chave de exemplo em `appsettings.Development.json` |
| MFA TOTP com biblioteca externa adiciona dependência | Baixa | S | Usar `Otp.NET` (maduro, MIT) ou implementar RFC 6238 direto (~80 linhas) |
| Outbox MassTransit + EF Core 10 ainda em flux | Média | M | Se quebrar, publicar direto no fim da transação e migrar para outbox real na sprint 2 |
| Testcontainers lento em CI/dev | Média | S | Reusar container entre testes via `IClassFixture`; rodar paralelo só em CI |
| Subestimar tempo do `AuthService` (8 métodos não-triviais) | Alta | M | Quebrar em 4 PRs: signup+login / refresh+logout / mfa / password recovery |

---

## 7. Plano diário sugerido (10 dias úteis)

> Estimativa só pra ter referência — ajustar conforme realidade. Marcar dias concluídos.

- [ ] **D1 (qua 2026-06-09)** — Setup: NuGets, DI, appsettings, gerar chaves RSA, migration inicial rodando.
- [ ] **D2 (qui 2026-06-10)** — Entidades faltantes (`RefreshToken`, `Consent`, `MfaChallenge`) + configurations + migration aplicada.
- [ ] **D3 (sex 2026-06-11)** — `TokenService` + `JwtBearer` + JWKS endpoint funcionando.
- [ ] **D4 (seg 2026-06-14)** — `AuthService.SignUpAsync` + `LoginAsync` + endpoints + testes unitários.
- [ ] **D5 (ter 2026-06-15)** — `RefreshAsync` + `LogoutAsync` + rotation + lockout + testes.
- [ ] **D6 (qua 2026-06-16)** — MFA: `Setup/Confirm/Disable/login mfa` + testes.
- [ ] **D7 (qui 2026-06-17)** — Recuperação senha + change password + `/me` + consent + testes.
- [ ] **D8 (sex 2026-06-18)** — **Marco MVP**: OTel + Serilog + health checks + outbox + RabbitMQ + integração end-to-end.
- [ ] **D9 (seg 2026-06-19)** — OpenAPI + README + ADRs + bench p95 + correções.
- [ ] **D10 (ter 2026-06-20)** — Hardening, gravar demo, fechar sprint.

---

## 8. Pré-requisitos para Sprint 2 (Patients)

Para a Sprint 2 começar sem fricção, ao fim desta sprint precisamos ter:

- [ ] `Auth.Contracts` publicando `UserRegistered` numa exchange RabbitMQ que outros serviços possam assinar.
- [ ] JWKS endpoint estável em URL conhecida (`http://localhost:5001/.well-known/jwks.json` em dev).
- [ ] Documentação de como um novo serviço valida JWT do Auth.API (em `system-design.md` ou README).
- [ ] Seed de saas_admin acessível para devs testarem Patients.

---

## 9. Checklist de fechamento da sprint

- [ ] Todos os itens do **§3 Definition of Done** marcados.
- [ ] Todos os critérios de aceite do **§5** marcados.
- [ ] Sprint review feita (demo gravada compartilhada).
- [ ] Sprint retrospective registrada (lições aprendidas para Sprint 2).
- [ ] Backlog da Sprint 2 (Patients) iniciado em `docs/sprints/sprint-02-patients.md`.

---

> **Mantenedor:** Engenharia PsiFlow.
> **Status:** rascunho · aguardando início.
> **Última atualização:** 2026-06-07.
