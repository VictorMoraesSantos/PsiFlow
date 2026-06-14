'use client';

import { useRouter } from 'next/navigation';
import { useEffect, useState } from 'react';
import { AppShell } from '../../layouts/AppShell';
import { ToastProvider } from '../../components/Toast';
import { getAccessToken } from '../../services/http';
import { useApp } from '../../state/AppContext';

export default function ProtectedLayout({ children }: { children: React.ReactNode }) {
  const { isAuthenticated, isHydrating } = useApp();
  const router = useRouter();
  const [hasToken, setHasToken] = useState<boolean | null>(null);

  useEffect(() => {
    setHasToken(Boolean(getAccessToken()));
  }, []);

  useEffect(() => {
    if (isHydrating) return;
    if (hasToken === false && !isAuthenticated) {
      router.replace('/login');
    }
  }, [isAuthenticated, isHydrating, hasToken, router]);

  if (isHydrating || hasToken === null) {
    return (
      <main className="auth-page" aria-busy="true">
        <p className="auth-card__hint">Carregando workspace...</p>
      </main>
    );
  }

  if (!isAuthenticated && !hasToken) {
    return (
      <main className="auth-page" aria-busy="true">
        <p className="auth-card__hint">Carregando workspace...</p>
      </main>
    );
  }

  return (
    <ToastProvider>
      <AppShell>{children}</AppShell>
    </ToastProvider>
  );
}
