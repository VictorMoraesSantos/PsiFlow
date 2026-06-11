import { KeyRound, LockKeyhole, Mail, ShieldCheck, UserPlus } from 'lucide-react';
import { useState } from 'react';
import { Button } from '../components/Button';
import { api, isLocalFallbackStatus, login } from '../services/api';

type LoginPageProps = {
  onAuthenticated: () => void;
};

type AuthMode = 'login' | 'register' | 'recover' | 'mfa';

export function LoginPage({ onAuthenticated }: LoginPageProps) {
  const [mode, setMode] = useState<AuthMode>('login');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [message, setMessage] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [registerEmail, setRegisterEmail] = useState('');
  const [registerName, setRegisterName] = useState('');
  const [registerPhone, setRegisterPhone] = useState('');
  const [registerCrp, setRegisterCrp] = useState('');
  const [registerPassword, setRegisterPassword] = useState('');
  const [registerRole, setRegisterRole] = useState<'psychologist' | 'patient'>('psychologist');
  const [mfaCode, setMfaCode] = useState('');
  const [isMfaSubmitting, setIsMfaSubmitting] = useState(false);

  const passwordRequirements = [
    { label: '10 caracteres ou mais', isMet: registerPassword.length >= 10 },
    { label: 'uma letra maiuscula', isMet: /[A-Z]/.test(registerPassword) },
    { label: 'uma letra minuscula', isMet: /[a-z]/.test(registerPassword) },
    { label: 'um numero', isMet: /\d/.test(registerPassword) },
    { label: 'um simbolo', isMet: /[^A-Za-z0-9]/.test(registerPassword) },
  ];
  const isRegisterPasswordValid = passwordRequirements.every((requirement) => requirement.isMet);

  function changeMode(nextMode: AuthMode) {
    setMode(nextMode);
    setError(null);
    setMessage(null);
  }

  async function submit(event: React.FormEvent) {
    event.preventDefault();
    setError(null);
    setIsSubmitting(true);
    try {
      await login(email, password);
      onAuthenticated();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Nao foi possivel entrar.');
    } finally {
      setIsSubmitting(false);
    }
  }

  async function register(event: React.FormEvent) {
    event.preventDefault();
    setError(null);
    setMessage(null);
    if (!isRegisterPasswordValid) {
      setError('A senha precisa cumprir todos os requisitos antes de criar a conta.');
      return;
    }
    if (registerRole === 'psychologist' && !registerCrp.trim()) {
      setError('Informe o CRP para criar uma conta de psicologa.');
      return;
    }
    setIsSubmitting(true);
    try {
      await api.post('auth', '/v1/auth/register', { role: registerRole, fullName: registerName, email: registerEmail, password: registerPassword, confirmPassword: registerPassword, phone: registerPhone.trim() || null, crp: registerRole === 'psychologist' ? registerCrp.trim() : null, acceptedTermsVersion: '2026-06-01', acceptedPrivacyVersion: '2026-06-01' });
      setMessage(registerRole === 'psychologist' ? 'Conta de psicologa criada. Entre com seu email e senha.' : 'Conta de paciente criada. Entre com seu email e senha.');
      setEmail(registerEmail);
      setPassword('');
      setMode('login');
    } catch (err) {
      if (isLocalFallbackStatus(err)) {
        setMessage(registerRole === 'psychologist' ? 'Cadastro de psicologa registrado localmente.' : 'Cadastro de paciente registrado localmente.');
        return;
      }
      setError(err instanceof Error ? err.message : 'Nao foi possivel criar a conta.');
    } finally {
      setIsSubmitting(false);
    }
  }

  async function forgotPassword(event: React.FormEvent) {
    event.preventDefault();
    setError(null);
    setMessage(null);
    setIsSubmitting(true);
    try {
      await api.post('auth', '/v1/auth/forgot-password', { email });
      setMessage('Se o email existir, o fluxo de recuperacao foi iniciado.');
    } catch (err) {
      if (isLocalFallbackStatus(err)) {
        setMessage('Recuperacao registrada localmente.');
        return;
      }
      setError(err instanceof Error ? err.message : 'Nao foi possivel iniciar recuperacao.');
    } finally {
      setIsSubmitting(false);
    }
  }

  async function setupMfa() {
    setError(null);
    setMessage(null);
    setIsMfaSubmitting(true);
    try {
      const result = await api.post<{ secret: string; qrCodeUri: string }>('auth', '/v1/auth/mfa/setup');
      setMessage(`Verificacao em duas etapas preparada. Use este codigo no autenticador: ${result.secret}`);
    } catch (err) {
      if (isLocalFallbackStatus(err)) {
        setMessage('Preparacao registrada localmente. Entre na conta antes de ativar a verificacao em duas etapas no backend.');
        return;
      }
      setError(err instanceof Error ? err.message : 'Nao foi possivel preparar a verificacao em duas etapas.');
    } finally {
      setIsMfaSubmitting(false);
    }
  }

  async function verifyMfa(event: React.FormEvent) {
    event.preventDefault();
    setError(null);
    setMessage(null);
    try {
      await api.post('auth', '/v1/auth/mfa/verify', { code: mfaCode });
      setMessage('Verificacao em duas etapas ativada para esta conta.');
    } catch (err) {
      if (isLocalFallbackStatus(err)) {
        setMessage('Verificacao em duas etapas registrada localmente.');
        return;
      }
      setError(err instanceof Error ? err.message : 'Nao foi possivel verificar o codigo.');
    }
  }

  return (
    <main className="auth-page">
      <section className="auth-card" aria-labelledby="login-title">
        <div className="brand brand--auth">
          <div className="brand__mark" aria-hidden="true">P</div>
          <div>
            <strong>PsiFlow</strong>
            <span>Workspace clinico</span>
          </div>
        </div>
        <div>
          <h1 id="login-title">Entrar no workspace</h1>
          <p>Acesse pacientes, agenda, sessoes e prontuarios em um ambiente protegido.</p>
        </div>

        <div className="auth-tabs" role="tablist" aria-label="Fluxos de autenticacao">
          <button id="auth-tab-login" type="button" role="tab" aria-selected={mode === 'login'} aria-controls="auth-panel-login" tabIndex={mode === 'login' ? 0 : -1} onClick={() => changeMode('login')}>Entrar</button>
          <button id="auth-tab-register" type="button" role="tab" aria-selected={mode === 'register'} aria-controls="auth-panel-register" tabIndex={mode === 'register' ? 0 : -1} onClick={() => changeMode('register')}>Criar conta</button>
          <button id="auth-tab-recover" type="button" role="tab" aria-selected={mode === 'recover'} aria-controls="auth-panel-recover" tabIndex={mode === 'recover' ? 0 : -1} onClick={() => changeMode('recover')}>Recuperar senha</button>
        </div>

        {error ? <p className="inline-error" role="alert">{error}</p> : null}
        {message ? <p className="inline-success" role="status">{message}</p> : null}

        {mode === 'login' ? (
          <form id="auth-panel-login" role="tabpanel" aria-labelledby="auth-tab-login" className="resource-form auth-form" onSubmit={submit}>
            <label className="form-field">
              <span>Email</span>
              <div className="input-with-icon"><Mail aria-hidden="true" size={17} /><input type="email" value={email} autoComplete="email" onChange={(event) => setEmail(event.target.value)} required /></div>
            </label>
            <label className="form-field">
              <span>Senha</span>
              <div className="input-with-icon"><LockKeyhole aria-hidden="true" size={17} /><input type="password" value={password} autoComplete="current-password" onChange={(event) => setPassword(event.target.value)} required /></div>
            </label>
            <Button type="submit" disabled={isSubmitting}>{isSubmitting ? 'Entrando...' : 'Entrar no workspace'}</Button>
            <button className="auth-link" type="button" onClick={() => changeMode('mfa')}>Ativar verificacao em duas etapas</button>
          </form>
        ) : null}

        {mode === 'register' ? (
          <form id="auth-panel-register" role="tabpanel" aria-labelledby="auth-tab-register" className="resource-form auth-form" onSubmit={register}>
            <div className="auth-choice" aria-label="Tipo de conta">
              <button type="button" aria-pressed={registerRole === 'psychologist'} onClick={() => setRegisterRole('psychologist')}>Psicologa</button>
              <button type="button" aria-pressed={registerRole === 'patient'} onClick={() => setRegisterRole('patient')}>Paciente</button>
            </div>
            <label className="form-field">
              <span>Nome completo</span>
              <div className="input-with-icon"><UserPlus aria-hidden="true" size={17} /><input value={registerName} autoComplete="name" onChange={(event) => setRegisterName(event.target.value)} placeholder="Dra. Ana Souza" required /></div>
            </label>
            <label className="form-field">
              <span>Email</span>
              <div className="input-with-icon"><Mail aria-hidden="true" size={17} /><input type="email" value={registerEmail} autoComplete="email" onChange={(event) => setRegisterEmail(event.target.value)} placeholder="novo@email.com" required /></div>
            </label>
            <label className="form-field">
              <span>Telefone</span>
              <input type="tel" value={registerPhone} autoComplete="tel" onChange={(event) => setRegisterPhone(event.target.value)} placeholder="(11) 99999-9999" />
            </label>
            {registerRole === 'psychologist' ? (
              <label className="form-field">
                <span>CRP</span>
                <input value={registerCrp} onChange={(event) => setRegisterCrp(event.target.value)} placeholder="06/123456" required />
              </label>
            ) : null}
            <label className="form-field">
              <span>Senha</span>
              <div className="input-with-icon"><LockKeyhole aria-hidden="true" size={17} /><input type="password" value={registerPassword} autoComplete="new-password" onChange={(event) => setRegisterPassword(event.target.value)} placeholder="Crie uma senha segura" required minLength={10} aria-describedby="password-rules" /></div>
            </label>
            <ul id="password-rules" className="auth-checklist" aria-label="Requisitos da senha">
              {passwordRequirements.map((requirement) => (
                <li key={requirement.label} data-met={requirement.isMet}>{requirement.label}</li>
              ))}
            </ul>
            <Button type="submit" disabled={isSubmitting}>{isSubmitting ? 'Criando conta...' : 'Criar conta'}</Button>
            <p className="auth-card__hint">Ao criar a conta, voce aceita os termos e a politica de privacidade vigentes.</p>
          </form>
        ) : null}

        {mode === 'recover' ? (
          <form id="auth-panel-recover" role="tabpanel" aria-labelledby="auth-tab-recover" className="resource-form auth-form" onSubmit={forgotPassword}>
            <label className="form-field">
              <span>Email da conta</span>
              <div className="input-with-icon"><KeyRound aria-hidden="true" size={17} /><input type="email" value={email} autoComplete="email" onChange={(event) => setEmail(event.target.value)} placeholder="voce@clinica.com" required /></div>
            </label>
            <Button type="submit" disabled={isSubmitting}>{isSubmitting ? 'Enviando...' : 'Enviar recuperacao de senha'}</Button>
          </form>
        ) : null}

        {mode === 'mfa' ? (
          <form className="resource-form auth-form" onSubmit={verifyMfa}>
            <p className="auth-card__hint">A verificacao em duas etapas adiciona um codigo do aplicativo autenticador ao login. Entre na conta antes de ativar este recurso.</p>
            <Button type="button" variant="secondary" onClick={setupMfa} disabled={isMfaSubmitting}>{isMfaSubmitting ? 'Preparando...' : 'Preparar verificacao'}</Button>
            <label className="form-field">
              <span>Codigo do autenticador</span>
              <div className="input-with-icon"><ShieldCheck aria-hidden="true" size={17} /><input inputMode="numeric" autoComplete="one-time-code" value={mfaCode} onChange={(event) => setMfaCode(event.target.value)} placeholder="000000" required /></div>
            </label>
            <Button type="submit">Verificar codigo</Button>
            <button className="auth-link" type="button" onClick={() => changeMode('login')}>Voltar para login</button>
          </form>
        ) : null}
      </section>
    </main>
  );
}
