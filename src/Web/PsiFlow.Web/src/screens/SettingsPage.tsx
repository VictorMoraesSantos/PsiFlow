import { Bell, Clock, Mail, Save, ShieldCheck, UserRound } from 'lucide-react';
import { useState } from 'react';
import { Button } from '../components/Button';
import { Section } from '../components/Section';
import { useToast } from '../components/Toast';
import { api, isLocalFallbackStatus } from '../services/api';

type ProfileForm = {
  fullName: string;
  email: string;
  phone: string;
  crp: string;
  specialty: string;
};

type PreferencesForm = {
  timezone: string;
  appointmentReminder: string;
  sessionStartWindow: string;
  emailNotifications: boolean;
  clinicalSummaryEmail: boolean;
  requireMfa: boolean;
};

const defaultProfile: ProfileForm = {
  fullName: 'Dra. Vitoria Almeida',
  email: 'vitoria@clinicapsi.com',
  phone: '(11) 98888-0000',
  crp: '06/123456',
  specialty: 'Psicologia clinica',
};

const defaultPreferences: PreferencesForm = {
  timezone: 'America/Sao_Paulo',
  appointmentReminder: '24h',
  sessionStartWindow: '15min',
  emailNotifications: true,
  clinicalSummaryEmail: false,
  requireMfa: true,
};

export function SettingsPage() {
  const { notify } = useToast();
  const [profile, setProfile] = useState(defaultProfile);
  const [preferences, setPreferences] = useState(defaultPreferences);
  const [isSavingProfile, setIsSavingProfile] = useState(false);
  const [isSavingPreferences, setIsSavingPreferences] = useState(false);

  async function saveProfile(event: React.FormEvent) {
    event.preventDefault();
    setIsSavingProfile(true);
    try {
      await api.put('auth', '/v1/users/me', profile);
      notify('Perfil atualizado.');
    } catch (error) {
      if (isLocalFallbackStatus(error)) {
        notify('Perfil salvo localmente.', 'info');
        return;
      }
      notify(error instanceof Error ? error.message : 'Nao foi possivel salvar o perfil.', 'danger');
    } finally {
      setIsSavingProfile(false);
    }
  }

  async function savePreferences(event: React.FormEvent) {
    event.preventDefault();
    setIsSavingPreferences(true);
    try {
      await api.put('auth', '/v1/users/me/settings', preferences);
      notify('Configuracoes atualizadas.');
    } catch (error) {
      if (isLocalFallbackStatus(error)) {
        notify('Configuracoes salvas localmente.', 'info');
        return;
      }
      notify(error instanceof Error ? error.message : 'Nao foi possivel salvar as configuracoes.', 'danger');
    } finally {
      setIsSavingPreferences(false);
    }
  }

  return (
    <div className="settings-page page-stack">
      <Section title="Perfil e configuracoes" description="Atualize seus dados profissionais e preferencias de uso do workspace.">
        <div className="settings-hero" aria-label="Resumo do perfil logado">
          <div className="settings-avatar" aria-hidden="true">DV</div>
          <div>
            <strong>{profile.fullName}</strong>
            <span>{profile.specialty} · CRP {profile.crp}</span>
          </div>
          <div className="settings-hero__status"><ShieldCheck size={17} aria-hidden="true" /> Verificacao em duas etapas ativa</div>
        </div>
      </Section>

      <div className="settings-grid">
        <Section title="Perfil profissional" description="Dados usados para identificar voce no workspace e em rotinas operacionais.">
          <form className="resource-form settings-form" onSubmit={saveProfile}>
            <label className="form-field">
              <span>Nome completo</span>
              <div className="input-with-icon"><UserRound aria-hidden="true" size={17} /><input value={profile.fullName} onChange={(event) => setProfile({ ...profile, fullName: event.target.value })} required /></div>
            </label>
            <label className="form-field">
              <span>Email</span>
              <div className="input-with-icon"><Mail aria-hidden="true" size={17} /><input type="email" value={profile.email} onChange={(event) => setProfile({ ...profile, email: event.target.value })} required /></div>
            </label>
            <div className="form-grid">
              <label className="form-field">
                <span>Telefone</span>
                <input type="tel" value={profile.phone} onChange={(event) => setProfile({ ...profile, phone: event.target.value })} />
              </label>
              <label className="form-field">
                <span>CRP</span>
                <input value={profile.crp} onChange={(event) => setProfile({ ...profile, crp: event.target.value })} required />
              </label>
            </div>
            <label className="form-field">
              <span>Especialidade</span>
              <input value={profile.specialty} onChange={(event) => setProfile({ ...profile, specialty: event.target.value })} />
            </label>
            <div className="form-actions">
              <Button type="submit" disabled={isSavingProfile}><Save size={16} aria-hidden="true" />{isSavingProfile ? 'Salvando perfil...' : 'Salvar perfil'}</Button>
            </div>
          </form>
        </Section>

        <Section title="Configuracoes" description="Preferencias que controlam avisos, agenda e seguranca da sua conta.">
          <form className="resource-form settings-form" onSubmit={savePreferences}>
            <label className="form-field">
              <span>Fuso horario</span>
              <div className="input-with-icon"><Clock aria-hidden="true" size={17} /><select value={preferences.timezone} onChange={(event) => setPreferences({ ...preferences, timezone: event.target.value })}>
                <option value="America/Sao_Paulo">America/Sao_Paulo</option>
                <option value="America/Manaus">America/Manaus</option>
                <option value="America/Recife">America/Recife</option>
              </select></div>
            </label>
            <div className="form-grid">
              <label className="form-field">
                <span>Lembrete de consulta</span>
                <select value={preferences.appointmentReminder} onChange={(event) => setPreferences({ ...preferences, appointmentReminder: event.target.value })}>
                  <option value="2h">2 horas antes</option>
                  <option value="24h">24 horas antes</option>
                  <option value="48h">48 horas antes</option>
                </select>
              </label>
              <label className="form-field">
                <span>Abertura da sala online</span>
                <select value={preferences.sessionStartWindow} onChange={(event) => setPreferences({ ...preferences, sessionStartWindow: event.target.value })}>
                  <option value="5min">5 minutos antes</option>
                  <option value="15min">15 minutos antes</option>
                  <option value="30min">30 minutos antes</option>
                </select>
              </label>
            </div>
            <div className="settings-toggles" aria-label="Preferencias de notificacao e seguranca">
              <ToggleRow icon={<Bell size={17} aria-hidden="true" />} label="Receber notificacoes por email" description="Avisos operacionais sobre agenda, convites e alteracoes." checked={preferences.emailNotifications} onChange={(checked) => setPreferences({ ...preferences, emailNotifications: checked })} />
              <ToggleRow icon={<Mail size={17} aria-hidden="true" />} label="Resumo clinico por email" description="Enviar apenas um resumo operacional, sem notas de prontuario." checked={preferences.clinicalSummaryEmail} onChange={(checked) => setPreferences({ ...preferences, clinicalSummaryEmail: checked })} />
              <ToggleRow icon={<ShieldCheck size={17} aria-hidden="true" />} label="Exigir verificacao em duas etapas" description="Mantem uma etapa adicional no acesso quando a conta exigir maior protecao." checked={preferences.requireMfa} onChange={(checked) => setPreferences({ ...preferences, requireMfa: checked })} />
            </div>
            <div className="form-actions">
              <Button type="submit" disabled={isSavingPreferences}><Save size={16} aria-hidden="true" />{isSavingPreferences ? 'Salvando configuracoes...' : 'Salvar configuracoes'}</Button>
            </div>
          </form>
        </Section>
      </div>
    </div>
  );
}

function ToggleRow({ icon, label, description, checked, onChange }: { icon: React.ReactNode; label: string; description: string; checked: boolean; onChange: (checked: boolean) => void }) {
  return (
    <label className="settings-toggle">
      <span className="settings-toggle__icon">{icon}</span>
      <span className="settings-toggle__copy"><strong>{label}</strong><small>{description}</small></span>
      <input type="checkbox" checked={checked} onChange={(event) => onChange(event.target.checked)} />
    </label>
  );
}
