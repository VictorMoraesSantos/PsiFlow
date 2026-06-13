'use client';

import { useEffect } from 'react';

export default function AppError({ error, reset }: { error: Error & { digest?: string }; reset: () => void }) {
  useEffect(() => {
    console.error('[app] route error', error);
  }, [error]);

  return (
    <div className="route-error" role="alert">
      <h2>Algo interrompeu o carregamento desta area.</h2>
      <p>Tente recarregar. Se o problema persistir, verifique sua conexao e tente novamente.</p>
      <button type="button" className="button" onClick={reset}>Tentar novamente</button>
    </div>
  );
}
