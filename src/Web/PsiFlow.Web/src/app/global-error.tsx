'use client';

export default function GlobalError({ error, reset }: { error: Error & { digest?: string }; reset: () => void }) {
  return (
    <html lang="pt-BR">
      <body>
        <div className="route-error" role="alert">
          <h2>Erro inesperado no workspace.</h2>
          <p>Nao conseguimos concluir a operacao. Tente recarregar a aplicacao.</p>
          <button type="button" className="button" onClick={reset}>Recarregar</button>
        </div>
      </body>
    </html>
  );
}
