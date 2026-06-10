import { LockKeyhole, Mail } from 'lucide-react';
import { useState } from 'react';
import { Button } from '../components/Button';
import { login } from '../services/api';

type LoginPageProps = {
  onAuthenticated: () => void;
};

export function LoginPage({ onAuthenticated }: LoginPageProps) {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

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
          <p>Use sua conta para acessar pacientes, agenda, sessoes e prontuarios.</p>
        </div>
        <form className="resource-form" onSubmit={submit}>
          <label className="form-field">
            <span>Email</span>
            <div className="input-with-icon"><Mail aria-hidden="true" size={17} /><input type="email" value={email} autoComplete="email" onChange={(event) => setEmail(event.target.value)} required /></div>
          </label>
          <label className="form-field">
            <span>Senha</span>
            <div className="input-with-icon"><LockKeyhole aria-hidden="true" size={17} /><input type="password" value={password} autoComplete="current-password" onChange={(event) => setPassword(event.target.value)} required /></div>
          </label>
          {error ? <p className="inline-error" role="alert">{error}</p> : null}
          <Button type="submit" disabled={isSubmitting}>{isSubmitting ? 'Entrando...' : 'Entrar'}</Button>
        </form>
        <p className="auth-card__hint">Sem backend ativo, as telas continuam navegaveis com dados locais, mas mutacoes reais exigem login.</p>
      </section>
    </main>
  );
}
