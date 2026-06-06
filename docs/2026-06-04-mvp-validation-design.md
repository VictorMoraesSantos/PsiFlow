# MVP PsiFlow — Piloto Fechado de Validação

> Spec de validação do MVP do PsiFlow.
> Audiência: time interno de engenharia + psicólogas-piloto.
> Status: rascunho para revisão · 2026-06-04
> Referências: [`documentacao-produto-saas-psicologas.md`](../documentacao-produto-saas-psicologas.md) · [`system-design.md`](../system-design.md)

---

## 1. Objetivo do piloto

Validar, com uso real de 8-12 psicólogas convidadas, se o MVP do PsiFlow entrega valor central — **reduzir trabalho administrativo e organizar a rotina clínica** — em três frentes:

1. **Ativação:** psicólogas adotam o produto como ferramenta de trabalho diária.
2. **Retenção:** continuam usando após o efeito novidade.
3. **Segurança:** nenhum incidente com dados sensíveis durante o piloto.

O piloto é **fonte de decisão go/no-go** para a v2 (marketplace, pagamentos integrados, billing SaaS). Não é beta aberto e não busca aquisição.

---

## 2. Escopo

### 2.1 Dentro do MVP (7 serviços)

| # | Serviço | Entrega mínima viável |
|---|---|---|
| 1 | **Auth** | Cadastro/login de psicóloga e paciente, JWT self-issued, refresh token rotation, MFA opcional para psicólogas, aceite de termos, recuperação de senha, bloqueio por tentativas inválidas. |
| 2 | **Patients** | Cadastro manual, convite por link, perfil administrativo (dados pessoais, contato de emergência, responsável legal opcional), histórico de sessões, status do tratamento, inativação. |
| 3 | **Agenda** | Disponibilidade recorrente semanal, bloqueios pontuais, agendamento avulso (sem recorrência automática), cancelamento com flag `late_cancel` (>= 24h = normal; < 24h = late). Reagendamento via cancelamento + novo agendamento. |
| 4 | **Sessions** | Máquina de estados: `scheduled` → `in_progress` → `completed` / `no_show` / `canceled`. Sem conteúdo clínico (referência ao `ClinicalRecords`). |
| 5 | **ClinicalRecords** | Prontuário 1:1 com paciente, evolução por sessão, anamnese básica em texto livre, autosave, versionamento (append-only), AES-256 coluna-a-coluna, audit log de leitura/escrita. |
| 6 | **Notifications** | E-mail transacional (provider plugável: Resend em dev/prod inicial), templates versionados, lembretes T-24h e T-1h de sessão, confirmações de agendamento, recibos. **Sem dados clínicos no corpo** (RN013). |
| 7 | **OnlineSession** | Psicóloga cadastra URL externa (Zoom/Google Meet) por sessão. Link exibido na página da sessão + no email de lembrete. Sem sala própria. |

### 2.2 Fora do piloto (deferido para v2+)

| Item | Motivo do adiamento |
|---|---|
| **Documents** (termo, contrato, declaração, recibo PDF assinado) | Psicólogas-piloto usam template próprio; faremos upload + versionamento simples em v2. |
| **Billing SaaS** (Stripe/Asaas) | Piloto é gratuito; pricing real precisa de dados de willingness-to-pay. |
| **Marketplace** (busca pública de psicólogas) | Captação vem de convite no piloto. Marketplace só após operação clínica validada. |
| **Pagamentos integrados** (PIX/cartão para paciente) | v1 = controle manual. Gateway adiciona risco operacional e regulatório cedo demais. |
| **WhatsApp / SMS** | E-mail cobre o essencial do piloto; multicanal depende de consentimento + custo variável. |
| **Sala de vídeo própria (WebRTC)** | Alto custo de engenharia. Link externo cobre 100% do caso de uso no piloto. |
| **Sessões recorrentes automáticas** | Psicóloga recria manualmente durante piloto; vamos aprender o padrão real antes de automatizar. |
| **MFA obrigatório para psicóloga** | Opcional no piloto; obrigatório em produção com N clientes. |
| **Mobile nativo / PWA completo** | Web responsivo cobre o piloto (RF009 do product doc). |
| **Internacionalização, multi-moeda** | Brasil-only no piloto. |

---

## 3. Personas do piloto

Curadoria das 4 personas do product doc, focadas em quem realmente testa o MVP:

| Persona | Por que está no piloto | O que precisa dar certo |
|---|---|---|
| **Psicóloga autônoma iniciante** (Persona 1) | Maior potencial de viralização se a UX for simples. | Consegue se cadastrar, configurar agenda, convidar 1 paciente, registrar 1ª evolução sem ajuda. |
| **Psicóloga experiente com agenda cheia** (Persona 2) | Testa retenção: vai aguentar uso real por 6 semanas? | Reduz tempo administrativo, controle financeiro, prontuário rápido. |
| **Psicóloga online** (Persona 3) | Testa fluxo 100% remoto: link + lembrete + comparecimento. | Link da sessão visível, paciente entra sem fricção, instruções pré-sessão claras. |
| **Paciente buscando terapia** (Persona 4) | Testa a outra ponta: recebe convite, agenda, paga (manual), entra na sessão. | Onboarding simples, entende contexto, confia na plataforma, lembra do horário. |

> **Clínicas com equipe e admins de clínica NÃO estão no piloto.** Funcionalidade multi-tenant agregador fica para v2.

---

## 4. User stories de validação (Must Have do MoSCoW)

### 4.1 Auth

- **Como psicóloga**, quero criar conta com e-mail + senha + CRP, aceitar termos e acessar a plataforma em menos de 3 minutos.
- **Como paciente**, quero aceitar um convite por link e criar conta com e-mail + senha para me vincular à psicóloga.
- **Como qualquer usuária**, quero recuperar senha por e-mail e voltar a acessar a plataforma.
- **Como psicóloga**, quero ativar MFA opcional (TOTP) para proteger prontuários.

### 4.2 Patients

- **Como psicóloga**, quero cadastrar uma paciente manualmente (nome, e-mail, telefone) e ela receber convite por e-mail.
- **Como psicóloga**, quero ver lista das minhas pacientes com status (ativa, inativa, em tratamento, alta) e busca por nome.
- **Como psicóloga**, quero ver histórico de sessões e pagamentos da paciente em uma única tela.
- **Como psicóloga**, quero inativar uma paciente sem perder histórico (RN040).

### 4.3 Agenda

- **Como psicóloga**, quero definir minha disponibilidade semanal (ex.: terças e quintas 14h-18h) em menos de 1 minuto.
- **Como psicóloga**, quero bloquear uma janela específica (ex.: viagem 10-20/07) sem afetar o resto da agenda.
- **Como paciente**, quero ver horários disponíveis da minha psicóloga e agendar uma sessão em até 3 cliques.
- **Como paciente ou psicóloga**, quero cancelar uma sessão. Cancelamentos com < 24h geram flag `late_cancel` (não bloqueia agendamento, mas vai para relatório).
- **Como psicóloga**, quero ver minha agenda no formato diário, semanal e mensal.

### 4.4 Sessions

- **Como psicóloga**, quero abrir a sessão 5 minutos antes e marcar como `in_progress`.
- **Como paciente ou psicóloga**, quero registrar comparecimento (`completed`) ou falta (`no_show`).
- **Como psicóloga**, quero ver histórico de sessões de uma paciente com status e link de acesso.

### 4.5 ClinicalRecords

- **Como psicóloga**, quero abrir o prontuário de uma paciente e ver anamnese + todas as evoluções em ordem cronológica.
- **Como psicóloga**, quero escrever uma evolução por sessão com autosave a cada 5 segundos.
- **Como psicóloga**, quero editar uma evolução. A versão anterior fica preservada (append-only) e o histórico registra quem/quando.
- **Como paciente**, **NÃO** devo ter acesso ao prontuário (RN004).
- **Como admin SaaS**, quero ver audit log de quem acessou prontuários e quando.

### 4.6 Notifications

- **Como paciente**, quero receber e-mail 24h antes da minha sessão com data, hora, nome da psicóloga e link de acesso.
- **Como paciente**, quero receber e-mail 1h antes da sessão como lembrete final.
- **Como psicóloga**, quero receber e-mail quando uma sessão é agendada, cancelada ou remarcada por uma paciente.
- **Como psicóloga**, quero receber e-mail de cobrança (recibo) após marcar pagamento manual.
- **Nenhum e-mail pode conter conteúdo clínico** (evolução, hipóteses, plano terapêutico). (RN013)

### 4.7 OnlineSession

- **Como psicóloga**, quero cadastrar o link do Zoom/Meet na sessão no momento do agendamento (ou reutilizar padrão).
- **Como paciente**, quero ver o link da sessão na página "Minha próxima sessão" e nos e-mails de lembrete.
- **Como psicóloga**, quero ver se a paciente clicou no link (registro de `clicked_at` no log).
- **Como paciente**, quero ver instruções pré-sessão (ambiente silencioso, fone, teste de câmera).

---

## 5. Fluxos end-to-end críticos

### 5.1 Onboarding da psicóloga → 1ª sessão → 1ª evolução

```
1. Acessa /signup-psicologa
2. Informa: nome, e-mail, senha, telefone, CRP
3. Aceita termos de uso + política de privacidade (RF006)
4. Confirma e-mail (link de verificação)
5. Login → tela "Complete seu perfil"
6. Preenche: foto, bio, especialidades, abordagens, valor da sessão, modalidade (online/presencial/híbrido)
7. Configura disponibilidade semanal (ex.: seg/qua 9h-12h e 14h-18h)
8. Opcionalmente ativa MFA
9. Convida 1ª paciente (gera link, envia por e-mail)
10. Paciente aceita convite → paciente cadastrada e vinculada
11. Paciente agenda sessão para terça 15h
12. Sistema envia confirmação por e-mail para ambas
13. 24h antes: e-mail lembrete T-24h com link
14. 1h antes: e-mail lembrete T-1h
15. Paciente entra no link (Zoom) às 14:55
16. Psicóloga abre prontuário da paciente, marca sessão como in_progress
17. Após sessão: registra evolução (autosave a cada 5s)
18. Marca sessão como completed
19. Marca pagamento como recebido (manual) → recibo por e-mail à paciente
20. Psicóloga vê na agenda: sessão passada, status completed
```

**Critério de sucesso do fluxo:** psicóloga completa o ciclo inteiro sem suporte humano.

### 5.2 Convite paciente → agendamento → comparecimento

```
1. Psicóloga gera link de convite para "maria@exemplo.com"
2. Sistema envia e-mail com link de aceite
3. Maria clica, cria conta (e-mail, telefone, senha), aceita termos
4. Sistema vincula Maria à psicóloga (PatientCreated event)
5. Maria acessa "Agendar sessão" → vê calendário com horários disponíveis
6. Escolhe terça 15h → confirma
7. Sistema valida horário livre, cria Appointment, dispara AppointmentScheduled
8. Notifications agenda T-24h e T-1h
9. Maria recebe e-mails nos momentos corretos
10. No horário, Maria clica no link (Zoom cadastrado pela psicóloga)
11. Sistema registra click + (futuramente) entrada
12. Após sessão, psicóloga marca completed
```

**Critério de sucesso:** Maria agenda e comparece sem nunca precisar ligar/mandar mensagem para a psicóloga.

### 5.3 Cancelamento + política de 24h

```
1. Paciente acessa "Minha sessão" → "Cancelar"
2. Sistema verifica antecedência:
   - >= 24h: cancela sem flag, status financeiro permanece "pendente" (psicóloga decide)
   - < 24h: cancela COM flag late_cancel, registra motivo opcional
3. Sistema dispara AppointmentCancelled para Notifications
4. Paciente e psicóloga recebem e-mail
5. Sessão some da agenda de ambas
6. Relatório da psicóloga mostra % de cancelamentos tardios
```

**Fora do MVP:** reembolso automático, política configurável por psicóloga (no piloto é fixa: 24h).

---

## 6. Critérios de aceite por feature (resumo)

Detalhamento técnico em `system-design.md` §4 e ADRs. Resumo executivo:

| Serviço | Critério de aceite (validação piloto) |
|---|---|
| **Auth** | Cadastro, login, refresh, logout, MFA, termos, recuperação funcionam em < 2s p95. Bloqueio após 5 tentativas falhas. JWT válido 15min, refresh 7d rotation. |
| **Patients** | Convite por link expira em 7 dias. Inativação preserva histórico. Cross-tenant access retorna 403. |
| **Agenda** | Conflito de horário impossível (constraint DB + validação). Bloqueio afeta apenas psicólogo dono. Cancelamento < 24h marca `late_cancel`. |
| **Sessions** | Transição de estado válida enforçada (não pula `in_progress`). Histórico de transições preservado. |
| **ClinicalRecords** | Conteúdo clínico ilegível no DB sem chave. Toda leitura/escrita gera audit log. Append-only: edição cria nova versão, nunca sobrescreve. |
| **Notifications** | E-mail enviado em < 30s após evento. Sem dado clínico no corpo. Log de envio/falha acessível ao admin. |
| **OnlineSession** | Link clicável em e-mail e na UI. Click registrado (analytics básico). |

---

## 7. Modelo de dados resumido

Detalhamento de colunas, migrations e contratos em `system-design.md` §4 e ADRs. Para o piloto, entidades principais:

- **Auth/DB:** `User`, `Role`, `UserRole`, `RefreshToken`, `Consent`, `MfaChallenge`
- **Patients/DB:** `Patient`, `PatientInvite`, `ResponsibleLegal`, `PatientNote` (admin only, não clínico)
- **Agenda/DB:** `Availability`, `AvailabilityException`, `ScheduleBlock`, `Appointment`
- **Sessions/DB:** `Session`, `SessionStatusHistory`
- **ClinicalRecords/DB:** `MedicalRecord` (1:1 com Patient), `Evolution`, `Anamnesis`, `TherapeuticPlan` (todos com `tenant_id` + AES-256-GCM)
- **Notifications/DB:** `Template`, `NotificationLog`, `NotificationPreference`
- **OnlineSession/DB:** `VideoRoom` (ref provider + URL)

**Multi-tenancy:** `tenant_id` = `psychologist_id` em todos os DBs. Global query filter + RLS Postgres.

**Não usado no piloto:** `Clinic` entity, `ClinicAdmin`, billing, marketplace.

---

## 8. Segurança & LGPD (mínimo do piloto)

Como o PsiFlow lida com **dados sensíveis de saúde** (LGPD art. 5º, II + art. 11), o piloto precisa de baseline mínimo, mesmo informal:

### 8.1 Criptografia
- TLS 1.3 em trânsito (Let's Encrypt via Caddy/nginx no VPS).
- AES-256-GCM coluna-a-coluna em PII e **todo** `ClinicalRecords` (chave por tenant, mestra no KMS).
- Senhas: PBKDF2 (Identity default).
- Backups: criptografados (restic + repo criptografado).

### 8.2 Controle de acesso
- Auth.API self-issued JWT (RSA 2048, JWKS endpoint).
- `tenant_id` em claim; global query filter no EF + RLS Postgres (defesa em profundidade).
- Cross-tenant access = 403 + audit log.
- RBAC mínimo: `psychologist`, `patient`, `saas_admin` (sem `clinic_admin` no piloto).

### 8.3 Auditoria
- Audit log append-only em schema `audit` por serviço.
- Captura: actor, action, resource, IP, user-agent, correlation_id, timestamp, result.
- Retenção 5 anos (CFP + LGPD).
- Acesso: apenas `saas_admin` (no piloto = founder).

### 8.4 Consentimento
- Aceite de termos versionado (hash + ts + IP).
- Paciente pode revogar → conta `deactivated`, prontuário mantido por obrigação CFP.
- Mudança de termo = re-aceite obrigatório no próximo login.

### 8.5 Retenção
- Prontuário: 5 anos após último atendimento (CFP Res. 010/2005).
- Audit log: 5 anos.
- Logs de aplicação: 90 dias.
- Backups: 30 dias PITR.

### 8.6 Direito ao esquecimento
- Endpoint `POST /v1/gdpr/forget-me` (autenticado como paciente).
- Anonimiza PII; preserva hash mínimo da existência do atendimento (CFP).

### 8.7 Não-objetivos de segurança do piloto
- Pentest formal externo (deferido para v2, pré-mercado).
- RIPD formal (recomendado pré-v2 com DPO contratado).
- 2FA obrigatório (opcional no piloto).
- Certificação ISO 27001 / SOC 2 (pós-piloto, se virar B2B).

---

## 9. Métricas & instrumentação

### 9.1 Métricas de produto (North Star + secundárias)

| Categoria | Métrica | Meta piloto | Como medir |
|---|---|---|---|
| Ativação | % psicólogas que completam perfil + agenda + 1 paciente + 1 evolução | ≥ 70% | Eventos `ProfileCompleted`, `AvailabilitySet`, `PatientCreated`, `EvolutionAdded` |
| Tempo até 1ª sessão | Mediana do tempo entre signup e primeiro AppointmentScheduled | ≤ 7 dias | Diferença de timestamps |
| Retenção | % psicólogas que usam ≥ 1x na semana 4 | ≥ 60% D30 | Login events |
| Engajamento | Sessões completadas/psicóloga/semana | ≥ 5 | SessionCompleted count |
| Prontuário | % psicólogas que registram evolução em ≥ 80% das sessões completed | ≥ 50% | Cross-ref SessionCompleted × EvolutionAdded |
| Qualidade | NPS psicólogas (survey semana 4 e 6) | ≥ 40 | Survey simples (1-5 + NPS calculado) |
| Qualidade | NPS pacientes (survey anônimo ao final) | ≥ 30 | Survey |
| Operacional | Taxa de comparecimento | ≥ 80% | Completed / Scheduled |
| Operacional | Taxa de cancelamento tardio | < 20% | late_cancel / total agendamentos |
| **Segurança** | **Incidentes de segurança** | **0 (bloqueador)** | Audit log + reporting manual |

### 9.2 Métricas técnicas

- Latência p95 endpoints principais: < 500ms
- Uptime serviços críticos (Auth, Agenda): ≥ 99% no piloto
- E-mails entregues (não bounce): ≥ 95%
- Erro 5xx: < 0.5% das requests

### 9.3 Instrumentação

- OpenTelemetry em todos os serviços (traces + metrics).
- Serilog → console JSON → Loki ou arquivo.
- Aspire dashboard em dev; OpenTelemetry Collector + Prometheus em prod (mínimo no VPS).
- Eventos de domínio são a **fonte primária** de métricas de produto. Dashboards derivados.

---

## 10. Critérios go/no-go para v2

**GO** se TODAS as condições:

- [ ] ≥ 70% psicólogas completaram ativação (perfil + agenda + 1 paciente + 1 evolução)
- [ ] ≥ 60% retenção D30
- [ ] NPS psicólogas ≥ 40
- [ ] NPS pacientes ≥ 30
- [ ] Zero incidentes de segurança ou vazamento
- [ ] ≥ 3 feedbacks qualitativos estruturados por psicóloga (entrevistas 30min)
- [ ] ≥ 5 feedbacks qualitativos de pacientes
- [ ] Latência p95 < 500ms, uptime ≥ 99%
- [ ] Pelo menos 1 psicóloga-piloto reporta que **pagaria** pelo produto (willingness-to-pay testado em survey)

**NO-GO** se:

- [ ] Qualquer incidente de segurança
- [ ] Ativação < 40%
- [ ] Retenção D30 < 30%
- [ ] NPS < 20
- [ ] Feedback recorrente: "usaria se tivesse X" onde X é inviável/anti-ético

**Repetir piloto** (estende 4 semanas) se: resultados mistos (zona cinzenta). Decisão do founder com base nos dados.

---

## 11. Riscos do piloto + mitigação

| Risco | Probabilidade | Impacto | Mitigação |
|---|---|---|---|
| Psicóloga-piloto abandona no meio | Média | Reduz N, distorce métricas | Suporte ativo via Slack/WhatsApp, 1:1 semanal, encuesta de fricção no dia 7. |
| Vazamento de dados clínicos | Baixa | Catastrófico (LGPD + CFP + reputação) | Criptografia desde dia 1, audit log, 1 teste de penetração interno pré-piloto, RLS Postgres, revisão de código focada em ClinicalRecords. |
| Incidente com gateway de e-mail (Resend) | Média | Baixa (lembretes falham) | Fallback para SMTP genérico configurável, retry exponencial, alerta de bounce > 5%. |
| Psicóloga expõe URL de Zoom errada | Média | Média (paciente não entra) | Validação de URL no cadastro, "testar link" antes de confirmar sessão. |
| Inadimplência no piloto (pagamento manual não feito) | Alta | Distorce métrica financeira | Não cobrar no piloto. Tratar "pagamento" como binário "marcou como pago" sem valor real. |
| Paciente confunde plataforma com serviço emergencial | Baixa | Risco ético | Disclaimer no login + email de boas-vindas + primeira tela da área do paciente com "PsiFlow não é emergência. Em crise, ligue 188 (CVV) ou SAMU 192." |
| Time subestima tempo de ClinicalRecords (criptografia coluna-a-coluna + audit) | Média | Atrasa piloto 2-3 sem | Cortar escopo: anamnese simples em texto único (sem múltiplas sub-entidades), autosave sem conflict resolution sofisticado. |
| VPS fica offline | Baixa | Piloto interrompido | Backup offsite diário (restic → S3/Backblaze B2), plano de restore testado, status page simples (Better Uptime free tier). |
| Psicóloga-piloto convida paciente "real" sem consentimento claro | Baixa | UX ruim para paciente real | Onboarding destaca: "traga 1-3 pacientes de confiança, explique que é piloto." |

---

## 12. Roadmap de construção

Tracer-bullet vertical slices. Cada serviço nasce mínimo viável e é refinado se necessário.

```
Semana 1-2   ┃ Auth real (substituir template weatherforecast)
             ┃   · ASP.NET Identity + JWT self-issued
             ┃   · Endpoints /v1/auth/*
             ┃   · MFA setup (TOTP)
             ┃   · Consent versionado
             ┃
Semana 3     ┃ Patients
             ┃   · CRUD + invite flow
             ┃   · Consumir UserRegistered do Auth
             ┃
Semana 4-5   ┃ Agenda
             ┃   · Availability + Block + Appointment
             ┃   · Lógica anti-conflito + late_cancel
             ┃
Semana 6     ┃ Sessions
             ┃   · Máquina de estados
             ┃   · Consumir AppointmentScheduled
             ┃
Semana 7-8   ┃ ClinicalRecords (paralelo com Notifications)
             ┃   · AES-256-GCM coluna-a-coluna
             ┃   · Audit log
             ┃   · Autosave (5s)
             ┃
Semana 7-8   ┃ Notifications (paralelo)
             ┃   · Templates versionados
             ┃   · Consumir AppointmentScheduled/Completed
             ┃
Semana 9     ┃ OnlineSession
             ┃   · CRUD VideoRoom
             ┃
Semana 10    ┃ Hardening + seed + deploy staging
             ┃   · Migrations rodadas em prod
             ┃   · Seed: 1 admin + 0 psicólogas
             ┃   · TLS + segredos em vars
             ┃   · Backup automático
             ┃   · Health checks
             ┃
Semana 11-12 ┃ Recrutamento + onboarding
             ┃   · 8-12 psicólogas convidadas
             ┃   · Onboarding 1:1 (30min) cada
             ┃
Semana 13-18 ┃ Piloto em uso
             ┃   · Sem 13-14: ramp-up (1-2 sessões/psicóloga)
             ┃   · Sem 15-16: uso normal
             ┃   · Sem 17-18: survey final + entrevistas
             ┃
Semana 19    ┃ Go/no-go decision
```

**Total:** ~19 semanas (4.5 meses) da decisão de começar até a decisão de seguir para v2.

> Se houver **2 devs** em paralelo, semanas 1-9 podem cair para 5-6. Recrutamento pode começar na semana 6 em paralelo com hardening.

---

## 13. Infraestrutura do piloto

### 13.1 Topologia

```
┌──────────────────────────────────────────────────────────────┐
│  Hostinger VPS / DigitalOcean (single-node)                  │
│  · 4 vCPU · 8GB RAM · 160GB SSD · Ubuntu 24.04              │
│  · $20-40/mês                                               │
│                                                              │
│  ┌────────────────────────────────────────────────────────┐  │
│  │  Docker Compose                                        │  │
│  │                                                        │  │
│  │  [caddy]  ←  TLS termination + reverse proxy           │  │
│  │    │                                                   │  │
│  │    ▼                                                   │  │
│  │  [auth-api] [patients-api] [agenda-api] [sessions-api] │  │
│  │  [clinical-api] [notifications-api] [online-api]       │  │
│  │    │                                                   │  │
│  │    ▼                                                   │  │
│  │  [postgres-16] [redis-7] [rabbitmq-3.13]               │  │
│  └────────────────────────────────────────────────────────┘  │
│                                                              │
│  [restic] → Backblaze B2 / S3 (backup offsite criptografado) │
│  [UptimeRobot / Better Uptime free] → alertas                │
└──────────────────────────────────────────────────────────────┘
```

### 13.2 Decisões de provisionamento

- **Domínio:** `app.psiflow.com.br` (ou similar). DNS no Cloudflare (free tier). TLS via Caddy + Let's Encrypt automático.
- **Segredos:** variáveis de ambiente em arquivo `.env` com permissões 600. NUNCA commit. Mover para Vault/KMS em prod real.
- **Migrations:** rodam no startup de cada API (`db.Database.Migrate()`) — aceitável no piloto, automatizar CI/CD em v2.
- **Logs:** stdout JSON → Docker logs → arquivo rotacionado (logrotate).
- **Monitoramento:** UptimeRobot pinga `/health/ready` a cada 5min. Alerta via e-mail/SMS.
- **Backups:** `restic` diário, retention 30 dias, repo em Backblaze B2 (~$0.50/mês).
- **Restore drill:** testar restore 1x durante hardening.

### 13.3 Não-objetivos de infra do piloto

- Multi-AZ, HA, k8s, auto-scaling, blue-green deploy.
- CDN (não necessário; web app leve).
- WAF comercial (Cloudflare free tier dá proteção básica).
- Disaster recovery multi-região.

---

## 14. Recrutamento + onboarding das psicólogas-piloto

### 14.1 Perfil ideal

- Autônoma ou de clínica pequena, atende online ou híbrido.
- 5-30 pacientes ativos.
- Já usa WhatsApp + planilha + Google Calendar (perfil "fragmentado").
- Disposta a dar feedback honesto (não yes-woman).
- Assina termo de piloto (uso gratuito + responsabilidade de não usar dados reais sensíveis no prontuário).

### 14.2 Processo

1. **Sourcing (sem 11):** lista de 20 candidatas via rede do founder, LinkedIn, grupos de psicologia.
2. **Screening (15min):** call para explicar piloto, validar fit, alinhar expectativas.
3. **Seleção:** 8-12 escolhidas.
4. **Onboarding individual (30min, sem 12):** demo ao vivo da plataforma, criação de conta guiada, configuração de agenda, explicação de "isto é piloto, bugs vão acontecer, teu feedback é ouro."
5. **Suporte contínuo:** canal único (Slack workspace ou grupo WhatsApp) com resposta em < 4h úteis.
6. **1:1 semanal (15min):** checar fricção, coletar feedback.
7. **Survey intermediária (sem 15):** NPS + perguntas abertas.
8. **Survey final + entrevista (sem 18):** NPS + 30min de entrevista semiestruturada.

### 14.3 Incentivo

- Uso gratuito durante 6 semanas.
- Acesso early à v2 com desconto de founding member.
- Co-autoria em case study (opcional, com aprovação).

---

## 15. Cronograma consolidado

| Marco | Data estimada (a partir de hoje) |
|---|---|
| Spec aprovado | 2026-06-04 (este doc) |
| Plano de implementação (writing-plans) | 2026-06-04/05 |
| Auth real pronto | 2026-06-18 |
| Patients pronto | 2026-06-25 |
| Agenda pronto | 2026-07-09 |
| Sessions pronto | 2026-07-16 |
| ClinicalRecords + Notifications prontos | 2026-07-30 |
| OnlineSession pronto | 2026-08-06 |
| Hardening + deploy staging | 2026-08-13 |
| Recrutamento concluído | 2026-08-27 |
| Onboarding das psicólogas | 2026-09-03 |
| Piloto em uso (4 semanas) | 2026-09-03 a 2026-09-30 |
| Survey final + entrevistas | 2026-10-01 a 2026-10-07 |
| **Decisão go/no-go v2** | **2026-10-08** |

> Datas são **estimativas**. Refinadas no writing-plans.

---

## 16. Não-objetivos do piloto

Pra evitar scope creep:

- ❌ Não validar pricing (sem cobrança).
- ❌ Não validar aquisição (sem landing pública, sem funil).
- ❌ Não validar multi-clínica (1 tenant = 1 psicóloga).
- ❌ Não validar prontuário compartilhado entre psicólogas.
- ❌ Não validar marketplace, busca pública, SEO.
- ❌ Não validar pagamentos integrados (manual é suficiente).
- ❌ Não validar app mobile nativo (web responsivo).
- ❌ Não validar integrações (Google Calendar, Google Meet, Zoom nativo).
- ❌ Não validar conformidade regulatória completa (DPO formal, RIPD, ISO).
- ❌ Não validar performance com > 12 psicólogas simultâneas.

Tudo isso é v2+, pós-go.

---

## 17. Próximos passos imediatos

1. ✅ Tu revisar este spec.
2. ⏭️ Self-review do spec (placeholders, consistência, ambiguidade) — farei antes de te entregar.
3. ⏭️ Tu aprovar ou pedir ajustes.
4. ⏭️ `writing-plans` skill → plano de implementação detalhado por serviço.
5. ⏭️ Execução começa pelo Auth.

---

> **Mantenedor:** Engenharia PsiFlow.
> **Próxima revisão:** após feedback das psicólogas-piloto (sem 16).
