import Link from 'next/link';

export default function AppNotFound() {
  return (
    <div className="route-not-found" role="status">
      <h2>Pagina nao encontrada</h2>
      <p>O endereco acessado nao corresponde a nenhuma area do workspace.</p>
      <Link className="button" href="/dashboard">Voltar para a visao geral</Link>
    </div>
  );
}
