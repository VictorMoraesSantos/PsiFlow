# Documentação de Produto — SaaS para Psicólogas e Pacientes

**Nome provisório:** CliniPsi  
**Data:** 02/06/2026  
**Formato:** Documentação de produto, requisitos, UX, SaaS e negócio  
**Públicos principais:** Psicólogas e pacientes  

---

## Sumário

1. [Visão geral do produto](#1-visão-geral-do-produto)
2. [Premissas regulatórias e éticas](#2-premissas-regulatórias-e-éticas)
3. [Levantamento de dores e necessidades](#3-levantamento-de-dores-e-necessidades)
4. [Personas](#4-personas)
5. [Jornadas do usuário](#5-jornadas-do-usuário)
6. [Funcionalidades por módulo](#6-funcionalidades-por-módulo)
7. [Requisitos funcionais](#7-requisitos-funcionais)
8. [Requisitos não funcionais](#8-requisitos-não-funcionais)
9. [Regras de negócio](#9-regras-de-negócio)
10. [Fluxos principais](#10-fluxos-principais)
11. [Backlog inicial](#11-backlog-inicial)
12. [MVP](#12-mvp)
13. [Métricas do produto](#13-métricas-do-produto)
14. [Planos de assinatura](#14-planos-de-assinatura)
15. [Riscos e cuidados](#15-riscos-e-cuidados)
16. [Diferenciais estratégicos](#16-diferenciais-estratégicos)
17. [Perguntas de descoberta](#17-perguntas-de-descoberta)
18. [Priorização MoSCoW](#18-priorização-moscow)
19. [Resumo executivo](#19-resumo-executivo)
20. [Próximos passos recomendados](#20-próximos-passos-recomendados)
21. [Referências regulatórias](#21-referências-regulatórias)

---

## 1. Visão geral do produto

### 1.1 Nome

**PsiFlow**

### 1.2 Descrição do SaaS

Plataforma SaaS para psicólogas gerenciarem atendimentos online, presenciais ou híbridos, centralizando agenda, pacientes, sessões, prontuários, documentos, pagamentos, lembretes, comunicação e acompanhamento administrativo.

Para pacientes, a plataforma oferece busca ou acesso por convite, agendamento, pagamento, lembretes, acesso à sessão online, documentos e uma experiência de acompanhamento mais clara e acolhedora.

### 1.3 Objetivo principal

Reduzir o trabalho administrativo da psicóloga e melhorar a experiência do paciente, mantendo segurança, privacidade, organização clínica e conformidade com boas práticas de proteção de dados.

### 1.4 Problema que o produto resolve

Psicólogas costumam operar com ferramentas desconectadas: agenda no Google Calendar, pagamentos no banco, prontuário em documentos, lembretes no WhatsApp, contratos em PDF e controle financeiro em planilhas.

Isso aumenta risco de erro, perda de informação, faltas, retrabalho e exposição indevida de dados sensíveis.

Pacientes, por sua vez, enfrentam dificuldade para encontrar profissionais confiáveis, entender disponibilidade, preço, abordagem, modalidade de atendimento, forma de pagamento e como acessar a consulta.

### 1.5 Proposta de valor

**Para psicólogas:** uma operação clínica e administrativa centralizada, segura e simples.

**Para pacientes:** uma jornada de terapia mais fácil, confiável, privada e acolhedora.

### 1.6 Diferenciais competitivos

| Diferencial | Valor gerado |
|---|---|
| Agenda inteligente com lembretes | Reduz faltas e cancelamentos |
| Prontuário simples, seguro e auditável | Organiza evolução clínica |
| Pagamentos integrados | Diminui inadimplência e cobrança manual |
| Página pública profissional | Ajuda na captação de pacientes |
| Documentos padronizados | Reduz retrabalho |
| Fluxo híbrido e online | Atende consultórios físicos e atendimento remoto |
| Comunicação segura | Reduz dependência de WhatsApp para dados sensíveis |
| Experiência acolhedora para paciente | Aumenta confiança e conversão |
| Controles de LGPD e auditoria | Mitiga riscos de privacidade |

### 1.7 Público-alvo

1. Psicólogas autônomas.
2. Psicólogas em clínicas.
3. Psicólogas com atendimento online.
4. Consultórios compartilhados.
5. Clínicas de psicologia.
6. Pacientes em busca de terapia.
7. Pacientes convidados diretamente por uma psicóloga.

### 1.8 Tipos de usuários

| Tipo de usuário | Descrição |
|---|---|
| Psicóloga autônoma | Usa agenda, pacientes, prontuário, pagamentos e documentos |
| Psicóloga clínica/equipe | Atua dentro de uma organização com permissões |
| Administradora da clínica | Gerencia profissionais, agenda, planos e relatórios |
| Paciente | Agenda, paga, acessa sessão e recebe comunicações |
| Responsável legal | Vinculado a paciente menor de idade, quando aplicável |
| Administrador SaaS | Gerencia plataforma, suporte, planos, auditoria e billing |

---

## 2. Premissas regulatórias e éticas

O produto deverá tratar dados de saúde como dados pessoais sensíveis, pois a Lei Geral de Proteção de Dados Pessoais classifica dados referentes à saúde como dados pessoais sensíveis. Isso exige controles fortes de segurança, finalidade, minimização, transparência, gestão de consentimento e governança de acesso.

A Autoridade Nacional de Proteção de Dados é a autoridade responsável por zelar pela proteção de dados pessoais e pela privacidade no Brasil. Para um SaaS nessa categoria, é recomendável prever processos de resposta a incidentes, atendimento a direitos dos titulares, registro de operações de tratamento, política de retenção e, quando aplicável, Relatório de Impacto à Proteção de Dados Pessoais.

No campo profissional, a Resolução CFP nº 09/2024 regulamenta o exercício profissional da Psicologia mediado por Tecnologias Digitais da Informação e Comunicação, revogando normas anteriores sobre o tema. A psicóloga permanece responsável por avaliar a viabilidade técnica, ética e profissional do atendimento mediado por tecnologia, bem como por proteger sigilo, privacidade e qualidade do serviço.

Segundo orientação de Conselho Regional, com a Resolução CFP nº 09/2024 não é mais necessário cadastro na plataforma e-Psi para atendimento online; permanece a necessidade de inscrição ativa no respectivo CRP e observância das diretrizes éticas e técnicas pertinentes.

> Esta documentação é de produto e requisitos, não parecer jurídico. Como o SaaS trata dados de saúde e registros clínicos, a validação final deve envolver jurídico, encarregado de dados/DPO e profissional responsável técnico da área de Psicologia.

---

## 3. Levantamento de dores e necessidades

### 3.1 Dores das psicólogas

| Dor | Impacto | Necessidade de produto |
|---|---|---|
| Dificuldade em organizar agenda | Conflitos de horário, esquecimentos, retrabalho | Agenda integrada, bloqueios, disponibilidade recorrente |
| Faltas e cancelamentos | Perda de receita e ociosidade | Lembretes, confirmação, política de cancelamento |
| Gestão manual de pagamentos | Cobrança desconfortável, inadimplência | Pagamento online, status financeiro, recibos |
| Falta de controle da evolução clínica | Perda de contexto terapêutico | Prontuário, evolução, histórico e tags clínicas |
| Ferramentas desconectadas | Dados duplicados e risco operacional | Plataforma centralizada |
| Preocupação com sigilo | Risco ético e jurídico | Controle de acesso, criptografia, logs, permissões |
| Dificuldade em captar pacientes | Dependência de indicação | Página pública, marketplace, SEO local |
| Excesso administrativo | Menos tempo para atendimento | Automação de lembretes, documentos e cobranças |
| Falta de padronização documental | Contratos e anamneses inconsistentes | Modelos editáveis e assinatura |
| Gestão do atendimento online | Links perdidos, atrasos, falhas de acesso | Sala virtual, link integrado, instruções pré-sessão |

### 3.2 Dores dos pacientes

| Dor | Impacto | Necessidade de produto |
|---|---|---|
| Dificuldade em encontrar psicóloga confiável | Adiamento do início da terapia | Perfis claros, CRP, abordagem, bio e disponibilidade |
| Falta de clareza sobre preço e modalidade | Fricção na decisão | Informações transparentes |
| Esquecimento de sessões | Faltas e cobrança inesperada | Lembretes multicanal |
| Dificuldade para remarcar | Abandono ou contato manual | Reagendamento assistido |
| Insegurança sobre privacidade | Baixa confiança | Comunicação clara sobre sigilo e dados |
| Barreiras no pagamento | Perda de conversão | PIX, cartão, comprovantes |
| Falta de acolhimento inicial | Ansiedade e desistência | Onboarding humanizado |
| Dificuldade de acessar consulta online | Atrasos e frustração | Link visível, teste técnico, instruções |
| Pouca visibilidade do processo terapêutico | Sensação de desorganização | Histórico administrativo, próximos passos, documentos |

---

## 4. Personas

### 4.1 Persona 1 — Psicóloga autônoma iniciante

| Campo | Descrição |
|---|---|
| Perfil | 27 anos, recém-formada, atende poucos pacientes, usa Instagram e indicações |
| Objetivos | Conseguir pacientes, parecer profissional, organizar agenda e pagamentos |
| Dores | Não sabe quanto cobrar, esquece cobranças, usa planilhas e WhatsApp |
| Necessidades | Página pública, agenda simples, pagamento fácil, documentos prontos |
| Comportamentos | Usa celular como principal ferramenta, busca soluções acessíveis |
| Critérios de decisão | Preço baixo, facilidade de uso, aparência profissional |
| Funcionalidades mais importantes | Perfil público, agenda, lembretes, PIX, contrato terapêutico |

### 4.2 Persona 2 — Psicóloga experiente com agenda cheia

| Campo | Descrição |
|---|---|
| Perfil | 42 anos, carteira consolidada, agenda recorrente, atende presencial e online |
| Objetivos | Reduzir faltas, controlar recorrência, organizar prontuários e finanças |
| Dores | Muitos reagendamentos, controle manual de pagamentos, pouco tempo |
| Necessidades | Automação, recorrência, relatórios, prontuário rápido |
| Comportamentos | Valoriza estabilidade e suporte, menos tolerante a bugs |
| Critérios de decisão | Segurança, confiabilidade, economia de tempo |
| Funcionalidades mais importantes | Agenda recorrente, evolução clínica, cobranças, recibos, relatórios |

### 4.3 Persona 3 — Psicóloga que atende online

| Campo | Descrição |
|---|---|
| Perfil | 35 anos, atende pacientes em diferentes cidades, usa Google Meet ou Zoom |
| Objetivos | Padronizar atendimento remoto, facilitar acesso do paciente |
| Dores | Links perdidos, pacientes com dificuldade técnica, insegurança sobre sigilo |
| Necessidades | Integração com videochamada, orientações pré-sessão, registro de presença |
| Comportamentos | Usa ferramentas digitais, prefere automação e integrações |
| Critérios de decisão | Integração, privacidade, experiência do paciente |
| Funcionalidades mais importantes | Sala online, lembrete com link, confirmação, termos e consentimento |

### 4.4 Persona 4 — Paciente buscando terapia pela primeira vez

| Campo | Descrição |
|---|---|
| Perfil | 29 anos, trabalha em tempo integral, nunca fez terapia |
| Objetivos | Encontrar profissional confiável e agendar sem constrangimento |
| Dores | Não entende abordagens, tem receio de exposição, não sabe preço |
| Necessidades | Informações claras, acolhimento, privacidade, pagamento simples |
| Comportamentos | Pesquisa pelo celular, compara perfis, prefere agendamento online |
| Critérios de decisão | Confiança, preço, disponibilidade, empatia |
| Funcionalidades mais importantes | Marketplace, perfil claro, agenda, pagamento, lembretes |

---

## 5. Jornadas do usuário

### 5.1 Jornada da psicóloga

| Etapa | Ação | Necessidade | Oportunidade de produto |
|---|---|---|---|
| Cadastro | Cria conta | Segurança e rapidez | Cadastro orientado por etapas |
| Perfil profissional | Informa CRP, bio, foto, abordagem | Credibilidade | Checklist de perfil completo |
| Agenda | Define horários, duração e modalidade | Evitar conflitos | Disponibilidade recorrente |
| Pacientes | Cadastra ou convida pacientes | Centralização | Convite por link |
| Agendamento | Marca sessão manualmente ou por paciente | Reduzir negociação | Autoagendamento |
| Atendimento | Acessa link ou local presencial | Organização | Botão “iniciar sessão” |
| Evolução | Registra evolução clínica | Sigilo e rapidez | Templates e autosave |
| Cobrança | Gera cobrança e acompanha status | Menos inadimplência | Pagamento automático |
| Histórico | Consulta sessões, documentos e evolução | Continuidade clínica | Timeline do paciente |
| Retenção | Mantém recorrência e lembretes | Continuidade | Sessões recorrentes e follow-up |

### 5.2 Jornada do paciente

| Etapa | Ação | Necessidade | Oportunidade de produto |
|---|---|---|---|
| Descoberta | Encontra plataforma ou recebe convite | Confiança | Perfil profissional validado |
| Busca/convite | Escolhe psicóloga ou aceita convite | Clareza | Filtros e página pública |
| Cadastro | Cria conta básica | Baixa fricção | Login sem burocracia |
| Agendamento | Escolhe horário | Facilidade | Calendário simples |
| Pagamento | Paga por PIX/cartão | Segurança | Checkout claro |
| Lembrete | Recebe aviso | Não esquecer | E-mail, WhatsApp, SMS |
| Atendimento | Acessa online ou presencial | Orientação | Link e instruções visíveis |
| Pós-sessão | Recebe próximos passos | Continuidade | Próxima sessão sugerida |
| Reagendamento | Altera horário | Flexibilidade | Fluxo guiado |
| Continuidade | Mantém tratamento | Previsibilidade | Sessões recorrentes |

---

## 6. Funcionalidades por módulo

### 6.1 Tabela geral de módulos

| Módulo | Usuários principais | Objetivo |
|---|---|---|
| Autenticação e usuários | Todos | Acesso seguro e perfis |
| Perfil da psicóloga | Psicóloga, paciente | Credibilidade profissional |
| Agenda | Psicóloga, paciente | Agendamento e disponibilidade |
| Pacientes | Psicóloga | Gestão administrativa do paciente |
| Clínico | Psicóloga | Prontuário e evolução |
| Atendimento online | Psicóloga, paciente | Sessões remotas |
| Financeiro | Psicóloga, paciente, admin | Cobrança, pagamento e relatórios |
| Comunicação | Todos | Lembretes e mensagens |
| Documentos | Psicóloga, paciente | Contratos, termos e recibos |
| Marketplace | Paciente, psicóloga | Descoberta e captação |
| Administrativo SaaS | Admin | Planos, suporte e auditoria |

### 6.2 Módulo de autenticação e usuários

- Cadastro de psicóloga.
- Cadastro de paciente.
- Login.
- Recuperação de senha.
- Verificação de e-mail.
- Autenticação multifator para psicólogas.
- Perfis de acesso.
- Consentimento de termos de uso.
- Consentimento de política de privacidade.
- Controle de sessão.
- Bloqueio por tentativas inválidas.

### 6.3 Módulo da psicóloga

- Perfil profissional.
- CRP.
- Especialidades.
- Abordagens terapêuticas.
- Bio.
- Foto.
- Valores.
- Modalidades: online, presencial, híbrido.
- Endereço profissional, quando presencial.
- Horários disponíveis.
- Página pública.
- Status de perfil completo.
- Validação manual ou automatizada de dados profissionais.

### 6.4 Módulo de agenda

- Calendário diário, semanal e mensal.
- Duração padrão da sessão.
- Horários recorrentes.
- Bloqueio de horários.
- Sessões recorrentes.
- Reagendamento.
- Cancelamento.
- Política de cancelamento.
- Confirmação de presença.
- Lembretes automáticos.
- Sincronização futura com Google Calendar.

### 6.5 Módulo de pacientes

- Cadastro de pacientes.
- Dados pessoais.
- Contatos.
- Contato de emergência.
- Responsável legal, quando aplicável.
- Status do tratamento.
- Observações administrativas.
- Histórico de sessões.
- Histórico financeiro.
- Consentimentos.
- Convite por link.
- Inativação de paciente.

### 6.6 Módulo clínico

- Prontuário psicológico.
- Evolução de sessão.
- Anamnese.
- Plano terapêutico.
- Hipóteses clínicas.
- Registro de demandas.
- Upload de arquivos.
- Documentos clínicos.
- Autosave.
- Versionamento.
- Controle de acesso.
- Log de visualização e alteração.
- Separação entre dados administrativos e clínicos.

### 6.7 Módulo de atendimento online

- Link externo de videochamada.
- Integração com Google Meet ou Zoom em versão futura.
- Sala virtual própria em versão avançada.
- Instruções pré-sessão.
- Confirmação de entrada.
- Registro de comparecimento.
- Link visível para paciente.
- Botão “entrar na sessão”.
- Aviso de privacidade e ambiente adequado.

### 6.8 Módulo financeiro

- Cobrança por sessão.
- Status de pagamento.
- PIX.
- Cartão de crédito.
- Recibos.
- Pagamentos pendentes.
- Relatórios financeiros.
- Pacotes de sessões.
- Cupons.
- Repasses, se houver marketplace.
- Exportação CSV.
- Integração com gateway de pagamento.

### 6.9 Módulo de comunicação

- Notificações por e-mail.
- WhatsApp ou SMS.
- Lembretes automáticos.
- Mensagens pré-definidas.
- Avisos de reagendamento.
- Avisos de cancelamento.
- Avisos de pagamento.
- Comunicação segura dentro da plataforma.
- Histórico de comunicações administrativas.

### 6.10 Módulo de documentos

- Contrato terapêutico.
- Termo de consentimento.
- Política de cancelamento.
- Declaração de comparecimento.
- Recibos.
- Modelos editáveis.
- Upload de documentos.
- Assinatura digital, se aplicável.
- Histórico de aceite.

### 6.11 Módulo de busca e marketplace

- Listagem de psicólogas.
- Filtros por especialidade.
- Filtros por abordagem.
- Filtros por preço.
- Filtros por disponibilidade.
- Filtros por modalidade.
- Filtros por localização.
- Perfil público.
- Agendamento direto.
- Página pública compartilhável.
- Avaliações com restrição ética.
- Moderação de perfis.

### 6.12 Módulo administrativo

- Dashboard SaaS.
- Métricas de uso.
- Gestão de usuários.
- Planos e assinaturas.
- Gestão de clínicas.
- Suporte.
- Controle de permissões.
- Logs de acesso.
- Auditoria.
- Gestão de incidentes.
- Exportação de relatórios.

---

## 7. Requisitos funcionais

### 7.1 Autenticação e usuários

- **RF001** - O sistema deve permitir que a psicóloga crie uma conta informando nome, e-mail, senha, CRP e dados profissionais.
- **RF002** - O sistema deve permitir que pacientes criem conta informando nome, e-mail, telefone e senha.
- **RF003** - O sistema deve permitir login por e-mail e senha.
- **RF004** - O sistema deve permitir recuperação de senha por e-mail.
- **RF005** - O sistema deve verificar o e-mail do usuário.
- **RF006** - O sistema deve registrar aceite dos termos de uso e política de privacidade.
- **RF007** - O sistema deve diferenciar perfis de acesso entre psicóloga, paciente, administrador de clínica e administrador SaaS.
- **RF008** - O sistema deve permitir autenticação multifator para psicólogas e administradores.
- **RF009** - O sistema deve bloquear temporariamente login após múltiplas tentativas inválidas.
- **RF010** - O sistema deve permitir encerramento manual de sessão ativa.

### 7.2 Perfil profissional

- **RF011** - O sistema deve permitir que a psicóloga cadastre foto, bio, CRP, especialidades, abordagens e modalidades de atendimento.
- **RF012** - O sistema deve permitir que a psicóloga informe valor da consulta.
- **RF013** - O sistema deve permitir que a psicóloga informe endereço para atendimento presencial.
- **RF014** - O sistema deve permitir que a psicóloga configure atendimento online, presencial ou híbrido.
- **RF015** - O sistema deve gerar página pública da psicóloga.
- **RF016** - O sistema deve exibir status de completude do perfil.
- **RF017** - O sistema deve permitir ocultar ou publicar o perfil no marketplace.
- **RF018** - O sistema deve permitir revisão administrativa de perfis publicados.

### 7.3 Agenda

- **RF019** - O sistema deve permitir que a psicóloga configure dias e horários disponíveis.
- **RF020** - O sistema deve permitir definir duração padrão das sessões.
- **RF021** - O sistema deve permitir bloquear horários específicos.
- **RF022** - O sistema deve permitir criar sessões avulsas.
- **RF023** - O sistema deve permitir criar sessões recorrentes.
- **RF024** - O sistema deve permitir que pacientes agendem sessões conforme horários disponíveis.
- **RF025** - O sistema deve impedir agendamento em horários ocupados.
- **RF026** - O sistema deve permitir reagendamento conforme política definida.
- **RF027** - O sistema deve permitir cancelamento conforme política definida.
- **RF028** - O sistema deve enviar confirmação após agendamento.
- **RF029** - O sistema deve enviar lembretes automáticos antes das sessões.
- **RF030** - O sistema deve registrar presença, falta ou cancelamento.

### 7.4 Pacientes

- **RF031** - O sistema deve permitir que a psicóloga cadastre pacientes.
- **RF032** - O sistema deve permitir convite de paciente por link.
- **RF033** - O sistema deve permitir armazenar dados pessoais do paciente.
- **RF034** - O sistema deve permitir cadastrar contato de emergência.
- **RF035** - O sistema deve permitir cadastrar responsável legal.
- **RF036** - O sistema deve permitir visualizar histórico de sessões do paciente.
- **RF037** - O sistema deve permitir alterar status do tratamento.
- **RF038** - O sistema deve permitir registrar observações administrativas.
- **RF039** - O sistema deve permitir registrar consentimentos do paciente.
- **RF040** - O sistema deve permitir inativar paciente sem excluir histórico clínico.

### 7.5 Clínico

- **RF041** - O sistema deve permitir criar prontuário psicológico.
- **RF042** - O sistema deve permitir registrar evolução de sessão.
- **RF043** - O sistema deve permitir criar anamnese.
- **RF044** - O sistema deve permitir registrar plano terapêutico.
- **RF045** - O sistema deve permitir registrar hipóteses clínicas.
- **RF046** - O sistema deve permitir upload de arquivos clínicos.
- **RF047** - O sistema deve permitir autosave de anotações clínicas.
- **RF048** - O sistema deve permitir versionamento de registros clínicos.
- **RF049** - O sistema deve restringir acesso ao prontuário à psicóloga responsável.
- **RF050** - O sistema deve registrar logs de acesso e alteração em dados clínicos.
- **RF051** - O sistema deve separar informações administrativas de informações clínicas.

### 7.6 Atendimento online

- **RF052** - O sistema deve permitir cadastrar link externo de videochamada.
- **RF053** - O sistema deve exibir o link da sessão para psicóloga e paciente.
- **RF054** - O sistema deve enviar lembrete contendo link da sessão.
- **RF055** - O sistema deve permitir registrar comparecimento à sessão online.
- **RF056** - O sistema deve exibir orientações pré-sessão ao paciente.
- **RF057** - O sistema deve permitir integração futura com Google Meet ou Zoom.
- **RF058** - O sistema deve permitir sala virtual própria em versão futura.

### 7.7 Financeiro

- **RF059** - O sistema deve permitir cobrança por sessão.
- **RF060** - O sistema deve permitir pagamento via PIX.
- **RF061** - O sistema deve permitir pagamento via cartão de crédito.
- **RF062** - O sistema deve registrar status de pagamento.
- **RF063** - O sistema deve alertar a psicóloga sobre pagamentos pendentes.
- **RF064** - O sistema deve emitir recibo.
- **RF065** - O sistema deve permitir marcar pagamento como recebido manualmente.
- **RF066** - O sistema deve gerar relatório financeiro.
- **RF067** - O sistema deve permitir criação de pacotes de sessões.
- **RF068** - O sistema deve permitir aplicação de cupom ou desconto.
- **RF069** - O sistema deve exportar dados financeiros em CSV.

### 7.8 Comunicação

- **RF070** - O sistema deve enviar notificações por e-mail.
- **RF071** - O sistema deve enviar notificações por WhatsApp ou SMS, conforme plano e consentimento.
- **RF072** - O sistema deve permitir mensagens pré-definidas.
- **RF073** - O sistema deve enviar aviso de reagendamento.
- **RF074** - O sistema deve enviar aviso de cancelamento.
- **RF075** - O sistema deve enviar aviso de pagamento pendente.
- **RF076** - O sistema deve permitir comunicação segura dentro da plataforma.
- **RF077** - O sistema deve registrar histórico de notificações enviadas.

### 7.9 Documentos

- **RF078** - O sistema deve permitir gerar contrato terapêutico.
- **RF079** - O sistema deve permitir gerar termo de consentimento.
- **RF080** - O sistema deve permitir gerar declaração de comparecimento.
- **RF081** - O sistema deve permitir gerar recibo.
- **RF082** - O sistema deve permitir modelos editáveis.
- **RF083** - O sistema deve registrar aceite de documentos.
- **RF084** - O sistema deve permitir assinatura digital quando integrada.
- **RF085** - O sistema deve permitir upload de documentos externos.

### 7.10 Marketplace

- **RF086** - O sistema deve listar psicólogas com perfil público ativo.
- **RF087** - O sistema deve permitir busca por nome.
- **RF088** - O sistema deve permitir filtros por especialidade.
- **RF089** - O sistema deve permitir filtros por abordagem.
- **RF090** - O sistema deve permitir filtros por preço.
- **RF091** - O sistema deve permitir filtros por disponibilidade.
- **RF092** - O sistema deve permitir filtros por modalidade e localização.
- **RF093** - O sistema deve permitir agendamento direto pelo perfil público.
- **RF094** - O sistema deve permitir desativar avaliações públicas caso violem diretrizes éticas.
- **RF095** - O sistema deve permitir moderação de perfis e conteúdos.

### 7.11 Administração SaaS

- **RF096** - O sistema deve permitir gestão de usuários.
- **RF097** - O sistema deve permitir gestão de planos.
- **RF098** - O sistema deve permitir gestão de assinaturas.
- **RF099** - O sistema deve permitir dashboard administrativo.
- **RF100** - O sistema deve permitir consulta de logs.
- **RF101** - O sistema deve permitir auditoria de eventos críticos.
- **RF102** - O sistema deve permitir abertura e acompanhamento de chamados.
- **RF103** - O sistema deve permitir suspender contas por violação de termos.
- **RF104** - O sistema deve permitir exportação de dados mediante solicitação autorizada.

---

## 8. Requisitos não funcionais

- **RNF001** - O sistema deve criptografar dados sensíveis em repouso e em trânsito.
- **RNF002** - O sistema deve estar disponível 99,5% do tempo mensal no MVP e evoluir para 99,9%.
- **RNF003** - O sistema deve seguir boas práticas de proteção de dados conforme a LGPD.
- **RNF004** - O sistema deve manter logs de acesso a dados clínicos.
- **RNF005** - O sistema deve permitir auditoria de ações críticas.
- **RNF006** - O sistema deve ter backup automático diário.
- **RNF007** - O sistema deve ter plano de recuperação de desastre.
- **RNF008** - O sistema deve responder às principais telas em até 2 segundos em condições normais.
- **RNF009** - O sistema deve suportar uso responsivo em desktop, tablet e celular.
- **RNF010** - O sistema deve seguir diretrizes de acessibilidade WCAG 2.1 nível AA como meta.
- **RNF011** - O sistema deve impedir acesso não autorizado por controle de permissões.
- **RNF012** - O sistema deve aplicar princípio de menor privilégio.
- **RNF013** - O sistema deve permitir revogação de sessões.
- **RNF014** - O sistema deve registrar consentimentos com data, hora, versão do documento e IP.
- **RNF015** - O sistema deve ter monitoramento de erros e indisponibilidade.
- **RNF016** - O sistema deve escalar horizontalmente os serviços de agenda, notificações e marketplace.
- **RNF017** - O sistema deve manter segregação lógica de dados entre psicólogas e clínicas.
- **RNF018** - O sistema deve permitir exportação dos dados da psicóloga conforme política e requisitos legais.
- **RNF019** - O sistema deve usar provedores de pagamento compatíveis com padrões de segurança do setor.
- **RNF020** - O sistema deve ter política de retenção e descarte de dados.
- **RNF021** - O sistema deve registrar incidentes de segurança e permitir análise posterior.
- **RNF022** - O sistema deve evitar exposição de dados clínicos em notificações externas.
- **RNF023** - O sistema deve suportar trilhas de auditoria imutáveis para eventos críticos.
- **RNF024** - O sistema deve manter ambientes separados de produção, homologação e desenvolvimento.
- **RNF025** - O sistema não deve usar dados clínicos para treinamento de modelos ou analytics sem base legal, transparência e controles específicos.

---

## 9. Regras de negócio

- **RN001** - A psicóloga só poderá atender pacientes após completar o cadastro profissional mínimo.
- **RN002** - O CRP deve ser solicitado no cadastro da profissional.
- **RN003** - Uma sessão só pode ser marcada em horário disponível.
- **RN004** - O paciente não pode acessar dados clínicos internos da psicóloga.
- **RN005** - A psicóloga só pode visualizar pacientes vinculados a ela.
- **RN006** - Administradores de clínica só podem acessar dados conforme permissões configuradas.
- **RN007** - Cancelamentos devem obedecer à política configurada pela psicóloga.
- **RN008** - Reagendamentos devem respeitar antecedência mínima configurada.
- **RN009** - Prontuários não devem ser excluídos definitivamente sem controle, retenção e auditoria.
- **RN010** - Pagamentos pendentes devem gerar alerta para psicóloga.
- **RN011** - Pacientes menores de idade podem exigir responsável legal cadastrado.
- **RN012** - O paciente só poderá confirmar uma sessão paga se o pagamento for aprovado ou se a psicóloga permitir pagamento posterior.
- **RN013** - Lembretes não devem conter conteúdo clínico sensível.
- **RN014** - Registros clínicos devem ser acessíveis apenas à psicóloga responsável e perfis autorizados.
- **RN015** - O marketplace deve permitir que a psicóloga oculte preço, se desejar.
- **RN016** - Avaliações públicas não devem expor conteúdo clínico, diagnóstico, evolução ou detalhes terapêuticos.
- **RN017** - Uma sessão online deve exibir instruções de privacidade e ambiente adequado.
- **RN018** - A psicóloga deve poder desativar autoagendamento.
- **RN019** - Sessões recorrentes devem respeitar bloqueios, férias e indisponibilidades.
- **RN020** - Toda alteração em documento assinado ou aceito deve gerar nova versão.

---

## 10. Fluxos principais

### 10.1 Cadastro da psicóloga

| Campo | Descrição |
|---|---|
| Ator principal | Psicóloga |
| Pré-condições | Nenhuma |
| Passos | Acessa cadastro; informa nome, e-mail, senha, telefone e CRP; aceita termos; confirma e-mail; completa perfil profissional |
| Exceções | E-mail já cadastrado; senha fraca; CRP inválido ou pendente de validação; termo não aceito |
| Resultado esperado | Conta criada e perfil em estado “incompleto” ou “apto para configuração” |

### 10.2 Cadastro do paciente

| Campo | Descrição |
|---|---|
| Ator principal | Paciente |
| Pré-condições | Convite recebido ou acesso pelo marketplace |
| Passos | Informa nome, e-mail, telefone e senha; aceita termos; confirma e-mail; completa dados básicos |
| Exceções | Link expirado; e-mail já cadastrado; menor de idade sem responsável |
| Resultado esperado | Paciente cadastrado e vinculado à psicóloga ou apto a agendar |

### 10.3 Agendamento de sessão

| Campo | Descrição |
|---|---|
| Ator principal | Paciente ou psicóloga |
| Pré-condições | Psicóloga com agenda configurada |
| Passos | Escolhe modalidade; seleciona horário; confirma dados; realiza pagamento, se exigido; recebe confirmação |
| Exceções | Horário indisponível; pagamento recusado; política não permite agendamento |
| Resultado esperado | Sessão criada e notificação enviada |

### 10.4 Reagendamento

| Campo | Descrição |
|---|---|
| Ator principal | Paciente ou psicóloga |
| Pré-condições | Sessão existente e política permitir alteração |
| Passos | Acessa sessão; solicita reagendamento; escolhe novo horário; confirma; sistema atualiza agenda |
| Exceções | Prazo mínimo expirado; horário indisponível; pagamento precisa ser revalidado |
| Resultado esperado | Sessão atualizada e envolvidos notificados |

### 10.5 Cancelamento

| Campo | Descrição |
|---|---|
| Ator principal | Paciente ou psicóloga |
| Pré-condições | Sessão existente |
| Passos | Acessa sessão; solicita cancelamento; informa motivo opcional; sistema aplica política; envia aviso |
| Exceções | Cancelamento fora do prazo; sessão já realizada; conflito com pagamento |
| Resultado esperado | Sessão cancelada e status financeiro atualizado |

### 10.6 Pagamento

| Campo | Descrição |
|---|---|
| Ator principal | Paciente |
| Pré-condições | Sessão ou pacote com cobrança gerada |
| Passos | Acessa checkout; escolhe PIX ou cartão; paga; gateway retorna status; sistema confirma |
| Exceções | Pagamento recusado; PIX expirado; gateway indisponível |
| Resultado esperado | Pagamento aprovado, pendente ou recusado com registro financeiro |

### 10.7 Atendimento online

| Campo | Descrição |
|---|---|
| Ator principal | Psicóloga e paciente |
| Pré-condições | Sessão online agendada |
| Passos | Recebem lembrete; acessam link; entram na sala; sistema registra comparecimento |
| Exceções | Link inválido; paciente atrasado; falha de integração |
| Resultado esperado | Atendimento realizado ou registrado como falta/cancelamento |

### 10.8 Registro de evolução clínica

| Campo | Descrição |
|---|---|
| Ator principal | Psicóloga |
| Pré-condições | Paciente vinculado e sessão realizada ou em andamento |
| Passos | Acessa paciente; abre sessão; registra evolução; salva; sistema versiona e audita |
| Exceções | Falha de conexão; tentativa de acesso sem permissão |
| Resultado esperado | Evolução registrada com segurança |

### 10.9 Emissão de recibo

| Campo | Descrição |
|---|---|
| Ator principal | Psicóloga |
| Pré-condições | Pagamento recebido ou marcado como recebido |
| Passos | Seleciona pagamento; gera recibo; revisa dados; envia ao paciente |
| Exceções | Dados fiscais incompletos; pagamento pendente |
| Resultado esperado | Recibo gerado e disponível |

### 10.10 Envio de lembrete

| Campo | Descrição |
|---|---|
| Ator principal | Sistema |
| Pré-condições | Sessão futura agendada |
| Passos | Verifica agenda; aplica regra de antecedência; envia e-mail/WhatsApp/SMS; registra status |
| Exceções | Canal sem consentimento; erro de envio; telefone inválido |
| Resultado esperado | Lembrete enviado e registrado |

### 10.11 Convite de paciente por link

| Campo | Descrição |
|---|---|
| Ator principal | Psicóloga |
| Pré-condições | Psicóloga autenticada |
| Passos | Gera link; envia ao paciente; paciente acessa; cria conta; fica vinculado |
| Exceções | Link expirado; paciente já vinculado; convite revogado |
| Resultado esperado | Paciente cadastrado e associado à psicóloga |

### 10.12 Busca de psicóloga pelo paciente

| Campo | Descrição |
|---|---|
| Ator principal | Paciente |
| Pré-condições | Marketplace ativo |
| Passos | Acessa busca; aplica filtros; visualiza perfil; escolhe horário; agenda |
| Exceções | Sem profissionais disponíveis; perfil indisponível; horário ocupado |
| Resultado esperado | Paciente encontra profissional e inicia fluxo de agendamento |

---

## 11. Backlog inicial

### 11.1 Épico: Autenticação

**Feature:** Cadastro e login seguro.

**User Story:**  
Como psicóloga, quero criar minha conta com segurança para acessar a plataforma e configurar meus atendimentos.

**Critérios de aceite:**

- Deve permitir cadastro com e-mail e senha.
- Deve exigir aceite dos termos.
- Deve verificar e-mail.
- Deve permitir recuperação de senha.
- Deve diferenciar perfil de psicóloga e paciente.

### 11.2 Épico: Perfil profissional

**Feature:** Página profissional.

**User Story:**  
Como psicóloga, quero configurar meu perfil profissional para que pacientes entendam minha atuação antes de agendar.

**Critérios de aceite:**

- Deve permitir foto, bio, CRP, especialidades e abordagens.
- Deve permitir configurar valor e modalidade.
- Deve gerar página pública.
- Deve permitir ocultar perfil do marketplace.

### 11.3 Épico: Gestão de agenda

**Feature:** Agendamento de sessões.

**User Story:**  
Como psicóloga, quero configurar meus horários disponíveis para que pacientes possam agendar sessões sem precisar falar comigo diretamente.

**Critérios de aceite:**

- Deve definir dias e horários disponíveis.
- Deve bloquear horários específicos.
- Deve impedir agendamento duplicado.
- Deve enviar confirmação após agendamento.

### 11.4 Épico: Gestão de pacientes

**Feature:** Cadastro e histórico do paciente.

**User Story:**  
Como psicóloga, quero manter dados e histórico dos pacientes em um único lugar para organizar minha rotina clínica e administrativa.

**Critérios de aceite:**

- Deve cadastrar paciente.
- Deve vincular paciente à psicóloga.
- Deve exibir histórico de sessões.
- Deve permitir status do tratamento.

### 11.5 Épico: Prontuário

**Feature:** Evolução clínica.

**User Story:**  
Como psicóloga, quero registrar a evolução de cada sessão para acompanhar o processo terapêutico com segurança.

**Critérios de aceite:**

- Deve criar evolução por sessão.
- Deve salvar automaticamente.
- Deve restringir acesso ao prontuário.
- Deve registrar logs de acesso e edição.

### 11.6 Épico: Pagamentos

**Feature:** Controle financeiro por sessão.

**User Story:**  
Como psicóloga, quero controlar pagamentos das sessões para reduzir inadimplência e cobrança manual.

**Critérios de aceite:**

- Deve registrar valor da sessão.
- Deve marcar pagamento como pendente, aprovado ou recebido.
- Deve permitir pagamento online.
- Deve gerar recibo.

### 11.7 Épico: Notificações

**Feature:** Lembretes automáticos.

**User Story:**  
Como paciente, quero receber lembretes antes da sessão para não esquecer meu horário.

**Critérios de aceite:**

- Deve enviar lembrete antes da sessão.
- Deve suportar e-mail no MVP.
- Deve registrar status do envio.
- Não deve conter dados clínicos sensíveis.

### 11.8 Épico: Atendimento online

**Feature:** Link de sessão.

**User Story:**  
Como paciente, quero acessar facilmente o link da consulta para entrar na sessão online sem dificuldades.

**Critérios de aceite:**

- Deve exibir link na página da sessão.
- Deve enviar link no lembrete.
- Deve permitir link externo no MVP.
- Deve registrar comparecimento.

### 11.9 Épico: Documentos

**Feature:** Modelos de documentos.

**User Story:**  
Como psicóloga, quero gerar documentos padronizados para reduzir retrabalho administrativo.

**Critérios de aceite:**

- Deve ter modelos de contrato, consentimento e declaração.
- Deve permitir edição.
- Deve registrar versão.
- Deve permitir envio ao paciente.

### 11.10 Épico: Marketplace

**Feature:** Busca de psicólogas.

**User Story:**  
Como paciente, quero buscar psicólogas por especialidade, preço e disponibilidade para escolher uma profissional adequada.

**Critérios de aceite:**

- Deve listar perfis públicos.
- Deve permitir filtros.
- Deve mostrar disponibilidade.
- Deve iniciar agendamento pelo perfil.

### 11.11 Épico: Administração SaaS

**Feature:** Gestão de planos e usuários.

**User Story:**  
Como administrador SaaS, quero gerenciar usuários, planos e métricas para operar a plataforma.

**Critérios de aceite:**

- Deve listar usuários.
- Deve gerenciar assinaturas.
- Deve exibir métricas básicas.
- Deve permitir auditoria de eventos críticos.

---

## 12. MVP

### 12.1 Funcionalidades obrigatórias para a primeira versão

| Área | Funcionalidade |
|---|---|
| Autenticação | Cadastro, login, recuperação de senha, aceite de termos |
| Psicóloga | Perfil profissional básico com CRP, bio, modalidade e valores |
| Agenda | Disponibilidade, bloqueios, agendamento, cancelamento simples |
| Pacientes | Cadastro, convite por link, histórico administrativo |
| Clínico | Prontuário básico e evolução de sessão |
| Atendimento online | Link externo de videochamada |
| Comunicação | Lembretes por e-mail |
| Financeiro | Controle manual de pagamentos e status |
| Documentos | Termo de consentimento e contrato em modelo editável |
| Segurança | Permissões, criptografia, logs básicos |
| Administração | Gestão simples de usuários e planos |

### 12.2 Funcionalidades importantes para segunda versão

| Área | Funcionalidade |
|---|---|
| Financeiro | PIX e cartão integrados |
| Comunicação | WhatsApp/SMS |
| Agenda | Sessões recorrentes avançadas |
| Documentos | Assinatura digital |
| Clínico | Anamnese estruturada e plano terapêutico |
| Marketplace | Perfil público e busca básica |
| Relatórios | Financeiro e comparecimento |
| Integrações | Google Calendar, Google Meet ou Zoom |

### 12.3 Funcionalidades avançadas para versões futuras

| Área | Funcionalidade |
|---|---|
| Marketplace | Recomendação de psicólogas |
| Atendimento | Sala própria de videochamada |
| Inteligência administrativa | Assistente para tarefas administrativas sem inferência clínica |
| Analytics | Predição de faltas e risco de churn |
| Clínico | Templates por abordagem |
| Clínica/equipe | Multiunidade, repasses e permissões avançadas |
| Paciente | Diário terapêutico controlado pela psicóloga |
| Internacionalização | Multi-idioma e multi-moeda |

---

## 13. Métricas do produto

### 13.1 Aquisição

- Número de psicólogas cadastradas.
- Número de pacientes cadastrados.
- Origem dos cadastros.
- Taxa de conversão do perfil público.
- Custo de aquisição por psicóloga.

### 13.2 Ativação

- Taxa de psicólogas que completam perfil.
- Taxa de psicólogas que configuram agenda.
- Tempo médio até primeira sessão agendada.
- Percentual de psicólogas que cadastram pelo menos 1 paciente.
- Percentual de psicólogas que registram primeira evolução.

### 13.3 Engajamento

- Número de sessões agendadas.
- Número de sessões realizadas.
- Uso do prontuário.
- Uso de lembretes.
- Uso de documentos.
- Frequência semanal de login da psicóloga.

### 13.4 Retenção

- Churn mensal.
- Retenção D30, D60 e D90.
- Número médio de pacientes ativos por psicóloga.
- Número médio de sessões por paciente.
- Continuidade de tratamento.

### 13.5 Receita

- MRR.
- ARR.
- ARPA por psicóloga.
- Conversão de trial para pago.
- Receita por plano.
- Volume de pagamentos processados.
- Taxa de inadimplência.

### 13.6 Qualidade

- NPS de psicólogas.
- NPS de pacientes.
- Taxa de comparecimento.
- Taxa de cancelamento.
- Taxa de faltas.
- Tempo médio de suporte.
- Erros em lembretes ou agendamentos.

---

## 14. Planos de assinatura

### 14.1 Plano Trial ou Gratuito

Indicado para psicólogas iniciantes testarem o produto.

| Recurso | Limite sugerido |
|---|---|
| Pacientes | Até 5 |
| Sessões/mês | Até 10 |
| Agenda | Básica |
| Prontuário | Básico |
| Lembretes | E-mail |
| Pagamentos | Controle manual |
| Marketplace | Não incluso ou perfil limitado |

### 14.2 Plano Individual

Indicado para psicólogas autônomas.

| Recurso | Inclusão |
|---|---|
| Pacientes | Até 50 |
| Sessões | Ilimitadas ou limite alto |
| Agenda | Completa |
| Prontuário | Completo |
| Documentos | Modelos básicos |
| Lembretes | E-mail |
| Financeiro | Controle manual e relatórios básicos |
| Página pública | Sim |

### 14.3 Plano Profissional

Indicado para psicólogas com agenda cheia.

| Recurso | Inclusão |
|---|---|
| Pacientes | Ilimitados |
| Lembretes | E-mail + WhatsApp/SMS |
| Pagamentos | PIX/cartão |
| Documentos | Modelos avançados |
| Relatórios | Financeiro e comparecimento |
| Marketplace | Destaque básico |
| Integrações | Calendar e videochamada |
| Suporte | Prioritário |

### 14.4 Plano Clínica/Equipe

Indicado para clínicas, consultórios compartilhados e equipes.

| Recurso | Inclusão |
|---|---|
| Psicólogas | Multiusuário |
| Permissões | Avançadas |
| Agenda | Por profissional e unidade |
| Pacientes | Base segmentada |
| Relatórios | Por profissional, unidade e período |
| Repasses | Sim |
| Administração | Gestão de equipe |
| Suporte | Prioritário ou dedicado |

### 14.5 Cobrança

- Mensal.
- Anual com desconto.
- Add-ons: WhatsApp, assinatura digital, storage adicional, marketplace premium, sala de vídeo própria.
- Taxa transacional em pagamentos online, se aplicável.

---

## 15. Riscos e cuidados

| Risco | Impacto | Mitigação |
|---|---|---|
| Vazamento de dados sensíveis | Alto risco jurídico, ético e reputacional | Criptografia, logs, pentest, menor privilégio, resposta a incidentes |
| Uso inadequado de informações clínicas | Violação de sigilo | Separar dados clínicos e administrativos, treinar suporte, restringir acesso |
| Falta de conformidade com LGPD | Multas e perda de confiança | DPO, mapeamento de dados, RIPD quando aplicável, políticas claras |
| Baixa adesão das psicólogas | Churn alto | UX simples, onboarding guiado, migração de planilhas |
| Dependência de integrações externas | Falhas em agenda, vídeo ou pagamento | Fallback manual, monitoramento, múltiplos provedores |
| Problemas com pagamentos | Perda financeira | Gateway confiável, conciliação, logs financeiros |
| Avaliações públicas antiéticas | Exposição indevida | Moderação, avaliações limitadas, foco em experiência administrativa |
| Suporte inadequado em crise | Risco ao paciente | Avisos claros de que a plataforma não é serviço emergencial |
| Confusão entre SaaS e atendimento emergencial | Risco de segurança | Mensagens de orientação e encaminhamento para serviços locais |
| Perda de dados | Prejuízo clínico | Backups, versionamento, disaster recovery |
| Acesso indevido por clínica | Violação de confidencialidade | Permissões granulares e auditoria |
| Notificação com dado sensível | Exposição externa | Templates neutros e sem conteúdo clínico |

---

## 16. Diferenciais estratégicos

1. **Experiência acolhedora para pacientes**  
   Onboarding sem linguagem fria, com explicação clara sobre privacidade, primeira sessão e expectativas.

2. **Prontuário simples e seguro**  
   Menos campos obrigatórios, mais foco em evolução, anamnese, plano terapêutico e histórico.

3. **Agenda inteligente**  
   Recorrência, bloqueios, lembretes, confirmação e sugestão de horários.

4. **Redução de faltas**  
   Lembretes multicanal, confirmação ativa, política de cancelamento e pagamento antecipado opcional.

5. **Página pública profissional**  
   Perfil elegante, compartilhável, com CRP, abordagem, modalidade, valores e horários.

6. **Pagamentos automatizados**  
   PIX, cartão, recibos, pendências e relatórios.

7. **Modelos de documentos**  
   Contrato terapêutico, consentimento, declaração e recibos.

8. **Relatórios financeiros**  
   Receita mensal, pendências, comparecimento, cancelamentos e ocupação da agenda.

9. **Integração com WhatsApp**  
   Apenas para comunicação administrativa e lembretes sem exposição clínica.

10. **Assistente administrativo**  
   Ajuda a organizar tarefas, lembretes, pendências e documentos, sem substituir julgamento clínico.

11. **Crescimento para psicólogas autônomas**  
   Marketplace, SEO local, página pública e analytics de conversão.

---

## 17. Perguntas de descoberta

### 17.1 Entrevistas com psicólogas

1. Como você organiza sua agenda hoje?
2. Quais ferramentas usa no dia a dia?
3. O que mais toma tempo fora das sessões?
4. Como controla pagamentos?
5. Como registra evolução dos pacientes?
6. Quais são suas maiores preocupações com atendimento online?
7. Como lida com faltas e cancelamentos?
8. Como pacientes chegam até você?
9. O que faria você pagar por uma plataforma?
10. Quais funcionalidades seriam indispensáveis?
11. Você já usou algum software de prontuário ou agenda?
12. O que te faria abandonar uma plataforma?
13. Como você lida com contratos e termos?
14. Quais informações considera sensíveis demais para colocar em sistema?
15. Você prefere cobrar antes ou depois da sessão?
16. Como funciona sua política de cancelamento?
17. Você atende menores de idade?
18. Você trabalha sozinha ou em clínica?
19. Como emite recibos hoje?
20. Que relatórios seriam úteis para sua rotina?

### 17.2 Entrevistas com pacientes

1. Como você procura uma psicóloga?
2. O que te impede de começar terapia?
3. O que te transmite confiança em uma profissional?
4. Você prefere atendimento online ou presencial?
5. Quais dificuldades já teve para agendar consultas?
6. Como gostaria de receber lembretes?
7. O que espera após uma sessão?
8. Como se sente em relação a pagamentos online?
9. Que informações gostaria de ver antes de marcar uma sessão?
10. O que faria você desistir antes da primeira sessão?
11. Você prefere escolher horário sozinho ou conversar antes?
12. Como gostaria de remarcar uma sessão?
13. O que te deixaria seguro sobre privacidade?
14. Você gostaria de ver preço antes de agendar?
15. Você entende diferenças entre abordagens terapêuticas?
16. Você teria receio de usar uma plataforma para terapia?
17. Qual canal prefere: e-mail, WhatsApp, SMS ou app?
18. O que te ajudaria a chegar preparado para a primeira sessão?

---

## 18. Priorização MoSCoW

### 18.1 Must Have

| Item | Justificativa |
|---|---|
| Cadastro e login | Base do produto |
| Perfil da psicóloga | Credibilidade e operação |
| Agenda básica | Núcleo do fluxo |
| Cadastro de pacientes | Gestão mínima |
| Agendamento | Valor central |
| Prontuário/evolução | Necessidade clínica essencial |
| Lembretes por e-mail | Redução de faltas |
| Controle de pagamentos | Redução administrativa |
| Link externo de atendimento online | MVP remoto viável |
| Termos e política de privacidade | Base de conformidade |
| Permissões e logs básicos | Segurança mínima |

### 18.2 Should Have

| Item | Justificativa |
|---|---|
| PIX e cartão | Melhora conversão e inadimplência |
| WhatsApp/SMS | Reduz faltas |
| Sessões recorrentes | Muito relevante para terapia |
| Recibos automáticos | Reduz trabalho manual |
| Modelos de documentos | Alta utilidade |
| Marketplace básico | Captação |
| Relatórios financeiros | Gestão profissional |
| Google Calendar | Evita conflitos externos |

### 18.3 Could Have

| Item | Justificativa |
|---|---|
| Sala própria de vídeo | Diferencial, mas caro |
| Assinatura digital | Importante, mas pode ser integração |
| Recomendação inteligente | Futuro marketplace |
| Diário do paciente | Depende de validação clínica |
| Assistente administrativo | Diferencial futuro |
| Multiunidade | Foco em clínicas maiores |

### 18.4 Won't Have no MVP

| Item | Motivo |
|---|---|
| Diagnóstico automatizado | Risco ético e clínico |
| IA sugerindo conduta terapêutica | Alto risco e baixa adequação inicial |
| Rede social de pacientes | Não alinhado a privacidade |
| Avaliações abertas com detalhes | Risco ético |
| Atendimento emergencial | Produto não deve se posicionar como emergência |
| Gravação automática de sessões | Alto risco de privacidade e baixa necessidade inicial |

---

## 19. Resumo executivo

O **CliniPsi** é um SaaS para psicólogas que centraliza agenda, pacientes, sessões, prontuário, documentos, pagamentos, lembretes e atendimento online. O produto resolve a fragmentação operacional da rotina clínica, reduz faltas, melhora controle financeiro e oferece ao paciente uma experiência mais clara, simples e acolhedora.

O MVP deve focar em psicólogas autônomas e online/híbridas, com cadastro, perfil profissional, agenda, pacientes, evolução clínica, lembretes, controle de pagamentos e link externo de videochamada. Marketplace, pagamentos integrados, WhatsApp, assinatura digital e sala própria de vídeo devem entrar em versões posteriores.

A principal vantagem estratégica será combinar **simplicidade operacional**, **segurança de dados sensíveis**, **experiência acolhedora para pacientes** e **ferramentas reais de crescimento para psicólogas autônomas**.

---

## 20. Próximos passos recomendados

1. **Validar problema com 15 a 20 psicólogas**  
   Separar por perfil: iniciante, experiente, online, clínica/equipe.

2. **Entrevistar 10 a 15 pacientes**  
   Focar em primeira terapia, remarcação, confiança, pagamento e privacidade.

3. **Mapear jornada atual manual**  
   Identificar quais ferramentas usam hoje e quanto tempo gastam por semana.

4. **Prototipar MVP em baixa fidelidade**  
   Fluxos prioritários: cadastro, agenda, paciente, evolução e pagamento.

5. **Testar usabilidade com psicólogas**  
   Medir tempo para configurar agenda, cadastrar paciente e registrar evolução.

6. **Definir arquitetura de segurança e dados**  
   Separação clínica/administrativa, criptografia, logs, backup, política de retenção.

7. **Validar jurídico e ético**  
   Revisar LGPD, termos, política de privacidade, contratos e limites do atendimento online.

8. **Construir piloto fechado**  
   Lançar para 20 a 50 psicólogas, medir ativação, sessões agendadas, uso do prontuário e retenção.

9. **Definir pricing real com base em disposição de pagamento**  
   Testar trial, plano individual e plano profissional.

10. **Evoluir para marketplace somente após resolver operação clínica**  
    Primeiro entregar valor para psicólogas; depois ampliar captação de pacientes.

---

## 21. Referências regulatórias

- Lei nº 13.709/2018 — Lei Geral de Proteção de Dados Pessoais: https://www.planalto.gov.br/ccivil_03/_ato2015-2018/2018/lei/l13709.htm
- Autoridade Nacional de Proteção de Dados — Portal oficial: https://www.gov.br/anpd/pt-br
- Comunicação de Incidente de Segurança — ANPD: https://www.gov.br/anpd/pt-br/assuntos/comunicacao-de-incidentes-de-seguranca-cis
- Atendimento psicológico online — Transparência CRP 12: https://transparencia.cfp.org.br/crp12/pergunta-frequente/atendimentopsicologicoonline/
- Código de Ética Profissional da/o Psicóloga/o — CFP: https://transparencia.cfp.org.br/wp-content/uploads/sites/29/2025/04/CodigoDeEtica_2025_Digital.pdf

---
