'use client';

import { AlertTriangle, CalendarCheck, CheckCircle2, ClipboardCheck, Clock3, FileText, ShieldCheck, UsersRound, Video } from 'lucide-react';
import { useRouter } from 'next/navigation';
import { Badge } from '../components/Badge';
import { Button } from '../components/Button';
import { MetricTile } from '../components/MetricTile';
import { MonthCalendar } from '../components/MonthCalendar';
import { Section } from '../components/Section';
import { useApp } from '../state/AppContext';
import type { Appointment, StatusTone } from '../types';

export function DashboardPage() {
  const { data, isLoading } = useApp();
  const router = useRouter();
  const navigate = (path: string) => router.push(path);
  const nextAppointment = data.appointments.find((appointment) => appointment.status !== 'Cancelada') ?? data.appointments[0];
  const nextSession = data.sessions.find((session) => session.status !== 'Finalizada') ?? data.sessions[0];
  const pendingRecords = data.records.filter((record) => record.status !== 'Assinado').length;
  const confirmedAppointments = data.appointments.filter((appointment) => appointment.status === 'Confirmada').length;
  const pendingNotifications = data.templates.filter((template) => template.status === 'Pausado').length;
  const scheduleConflicts = data.appointments.filter((appointment) => appointment.status === 'Pendente').length;
  const hasAppointments = data.appointments.length > 0;
  const hasOperationalData = hasAppointments || data.patients.length > 0 || data.records.length > 0 || data.sessions.length > 0;
  const primaryAction = nextAppointment?.kind === 'Online' ? 'Abrir sala online' : nextAppointment ? 'Preparar atendimento' : 'Criar consulta';
  const secondaryAction = pendingRecords > 0 ? 'Revisar prontuarios' : 'Ver agenda';
  const attentionItems = [
    {
      id: 'records',
      icon: ClipboardCheck,
      title: pendingRecords > 0 ? `${pendingRecords} prontuario${pendingRecords > 1 ? 's' : ''} para revisar` : 'Prontuarios em dia',
      detail: pendingRecords > 0 ? 'Finalize assinatura ou revisao antes do fim do dia.' : 'Nenhuma evolucao clinica pendente agora.',
      tone: pendingRecords > 0 ? 'warning' : 'success',
      badge: pendingRecords > 0 ? 'Acao hoje' : 'OK',
    },
    {
      id: 'agenda',
      icon: AlertTriangle,
      title: scheduleConflicts > 0 ? `${scheduleConflicts} consulta${scheduleConflicts > 1 ? 's' : ''} sem confirmacao` : 'Agenda confirmada',
      detail: scheduleConflicts > 0 ? 'Confira horario, modalidade e confirmacao do paciente.' : 'Os atendimentos listados nao exigem ajuste imediato.',
      tone: scheduleConflicts > 0 ? 'warning' : 'success',
      badge: scheduleConflicts > 0 ? 'Conferir' : 'OK',
    },
    {
      id: 'notifications',
      icon: ShieldCheck,
      title: pendingNotifications > 0 ? `${pendingNotifications} modelo${pendingNotifications > 1 ? 's' : ''} pausado` : 'Comunicacao ativa',
      detail: pendingNotifications > 0 ? 'Revise lembretes pausados antes de depender de envio automatico.' : 'Modelos essenciais estao ativos para a operacao.',
      tone: pendingNotifications > 0 ? 'info' : 'success',
      badge: pendingNotifications > 0 ? 'Revisar' : 'OK',
    },
  ] as const;

  return (
    <div className="page-stack">
      <section className="hero-panel" aria-label="Resumo do dia">
        <div className="hero-panel__content">
          <span className="context-label">Atendimento de hoje</span>
          <h2>{nextAppointment ? `Proxima sessao as ${nextAppointment.time}` : 'Dia sem consultas marcadas'}</h2>
          <p>{nextAppointment ? 'Veja o que exige preparo agora, sem expor notas clinicas na visao geral.' : 'Use este espaco para criar agenda, revisar pendencias e manter o contexto clinico organizado.'}</p>
          <div className="hero-panel__actions">
            <Button type="button" onClick={() => navigate(nextAppointment?.kind === 'Online' ? '/online' : '/sessions')}>{primaryAction}</Button>
            <Button type="button" variant="secondary" onClick={() => navigate(pendingRecords > 0 ? '/records' : '/agenda')}>{secondaryAction}</Button>
          </div>
        </div>
        <div className="care-card dashboard-next" aria-label="Proximo atendimento">
          <span className="care-card__label">Proximo atendimento</span>
          <strong className="care-card__name">{nextAppointment?.patientName ?? 'Sem paciente selecionado'}</strong>
          <p className="care-card__detail">{nextAppointment ? `${nextAppointment.kind} · ${nextAppointment.status}` : 'Nenhuma sessao planejada para hoje'}</p>
          <div className="dashboard-next__meta">
            <Badge tone={getAppointmentTone(nextAppointment)}>{nextAppointment?.status ?? 'Livre'}</Badge>
            {nextAppointment?.kind === 'Online' ? <Badge tone="info">Sala online</Badge> : null}
          </div>
        </div>
      </section>

      <div className="metric-grid" aria-label="Indicadores operacionais">
        <button type="button" className="metric-button" onClick={() => navigate('/agenda')}><MetricTile label="Consultas confirmadas" value={isLoading ? '...' : String(confirmedAppointments)} detail="Agenda de hoje" tone="accent" icon={<CalendarCheck size={24} />} /></button>
        <button type="button" className="metric-button" onClick={() => navigate('/records')}><MetricTile label="Prontuarios pendentes" value={isLoading ? '...' : String(pendingRecords)} detail="Precisam de revisao ou assinatura" icon={<FileText size={24} />} /></button>
        <button type="button" className="metric-button" onClick={() => navigate('/patients')}><MetricTile label="Pacientes ativos" value={isLoading ? '...' : String(data.patients.length)} detail="Contextos disponiveis no workspace" icon={<UsersRound size={24} />} /></button>
      </div>

      {isLoading ? <DashboardLoadingState /> : null}
      {!isLoading && !hasOperationalData ? <DashboardEmptyState /> : null}

      <div className="two-column">
        <Section title="Agenda de hoje" description="Ordem de atendimento, modalidade e confirmacao.">
          <div className="timeline-list">
            {hasAppointments ? data.appointments.map((appointment) => (
              <article className="timeline-item timeline-item--interactive" key={appointment.id}>
                <CalendarCheck aria-hidden="true" size={18} />
                <div>
                  <strong>{appointment.time} · {appointment.patientName}</strong>
                  <span>{appointment.kind} · dados clinicos no prontuario</span>
                </div>
                <Badge tone={getAppointmentTone(appointment)}>{appointment.status}</Badge>
              </article>
            )) : (
              <div className="empty-state empty-state--compact">
                <CalendarCheck aria-hidden="true" size={28} />
                <h3>Nenhuma consulta hoje</h3>
                <p>Crie um atendimento ou use o dia para fechar prontuarios e retornos pendentes.</p>
              </div>
            )}
          </div>
        </Section>

        <Section title="Calendario do mes" description="Visao rapida dos dias com compromissos. A agenda completa continua na pagina Agenda.">
          <MonthCalendar appointments={data.appointments} compact />
        </Section>

        <Section title="Atalhos do workspace" description="Continue uma tarefa sem perder o contexto do dia.">
          <div className="dashboard-actions-grid">
            <Button type="button" variant="secondary" onClick={() => navigate('/agenda')}>Abrir agenda</Button>
            <Button type="button" variant="secondary" onClick={() => navigate('/patients')}>Cadastrar paciente</Button>
            <Button type="button" variant="secondary" onClick={() => navigate('/records')}>Revisar prontuarios</Button>
            <Button type="button" variant="secondary" onClick={() => navigate('/notifications')}>Ver notificacoes</Button>
          </div>
        </Section>

        <Section title="Precisa de atencao" description="Pendencias sem detalhes clinicos sensiveis.">
          <div className="attention-list">
            {attentionItems.map((item) => {
              const Icon = item.icon;
              return (
                <article className="attention-item" key={item.id}>
                  <Icon aria-hidden="true" size={18} />
                  <div>
                    <strong>{item.title}</strong>
                    <span>{item.detail}</span>
                  </div>
                  <Badge tone={item.tone as StatusTone}>{item.badge}</Badge>
                </article>
              );
            })}
          </div>
        </Section>
      </div>

      <div className="two-column two-column--reverse-weight">
        <Section title="Continuidade clinica" description="Retome contexto sem abrir notas sensiveis no resumo.">
          <div className="continuity-list">
            {data.sessions.slice(0, 3).map((session) => (
              <article className="continuity-item" key={session.id}>
                <div>
                  <strong>{session.patientName}</strong>
                  <span>{session.status} · {session.modality ?? session.room}</span>
                </div>
                <Badge tone={session.status === 'Finalizada' ? 'success' : 'info'}>{session.payment}</Badge>
              </article>
            ))}
            {data.sessions.length === 0 ? (
              <div className="empty-state empty-state--compact">
                <Clock3 aria-hidden="true" size={28} />
                <h3>Sem sessoes recentes</h3>
                <p>Quando houver atendimentos, os retornos de contexto aparecem aqui.</p>
              </div>
            ) : null}
          </div>
        </Section>

        <Section title="Sala e seguranca" description="Preparos rapidos para atendimento online.">
          <div className="readiness-card">
            {nextSession?.room?.toLowerCase().includes('online') || nextAppointment?.kind === 'Online' ? <Video aria-hidden="true" size={22} /> : <CheckCircle2 aria-hidden="true" size={22} />}
            <div>
              <strong>{nextAppointment?.kind === 'Online' ? 'Sala online pronta para conferencia' : 'Atendimento presencial sem sala online'}</strong>
              <p>Revise presenca, horario e registro depois da sessao. Conteudo clinico permanece no prontuario.</p>
            </div>
          </div>
        </Section>
      </div>
    </div>
  );
}

function getAppointmentTone(appointment?: Appointment): StatusTone {
  if (!appointment) return 'neutral';
  if (appointment.status === 'Confirmada') return 'success';
  if (appointment.status === 'Cancelada') return 'danger';
  return 'warning';
}

function DashboardLoadingState() {
  return (
    <section className="dashboard-state" aria-label="Carregando dashboard">
      <div className="dashboard-skeleton" />
      <div className="dashboard-skeleton dashboard-skeleton--wide" />
      <span>Carregando agenda e pendencias...</span>
    </section>
  );
}

function DashboardEmptyState() {
  return (
    <section className="empty-state dashboard-empty" aria-label="Dashboard vazio">
      <CalendarCheck aria-hidden="true" size={34} />
      <h3>Configure a primeira agenda</h3>
      <p>Cadastre pacientes e consultas para que o Dashboard mostre proximos atendimentos, retornos e prontuarios pendentes.</p>
      <Button>Criar consulta</Button>
    </section>
  );
}
