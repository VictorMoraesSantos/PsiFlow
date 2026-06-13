import Link from 'next/link';

export default function NotFound() {
  return (
    <main className="auth-page">
      <section className="auth-surface" aria-labelledby="not-found-title">
        <div className="auth-card">
          <h1 id="not-found-title">Pagina nao encontrada</h1>
          <p>O endereco acessado nao corresponde a nenhuma area do workspace.</p>
          <Link className="button" href="/dashboard">Voltar para a visao geral</Link>
        </div>
      </section>
    </main>
  );
}
