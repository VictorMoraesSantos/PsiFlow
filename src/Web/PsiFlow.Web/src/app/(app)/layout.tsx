'use client';

import { useRouter } from 'next/navigation';
import { useEffect, useState } from 'react';
import { AppShell } from '../../layouts/AppShell';
import { ToastProvider } from '../../components/Toast';
import { getAccessToken, isDemoMode } from '../../services/http';
import { useApp } from '../../state/AppContext';

export default function ProtectedLayout({ children }: { children: React.ReactNode }) {
  const { isAuthenticated, isHydrating, isLocalMode } = useApp();
  const router = useRouter();
  const [hasToken, setHasToken] = useState<boolean | null>(null);
  const [hasDemoFlag, setHasDemoFlag] = useState<boolean | null>(null);

  useEffect(() => {
    setHasToken(Boolean(getAccessToken()));
    setHasDemoFlag(isDemoMode());
  }, []);

  useEffect(() => {
    if (isHydrating) return;
    if (hasToken === null || hasDemoFlag === null) return;
    if (!hasToken && !hasDemoFlag && !isAuthenticated && !isLocalMode) {
      router.replace('/login');
    }
  }, [isAuthenticated, isHydrating, hasToken, hasDemoFlag, isLocalMode, router]);

  if (isHydrating || hasToken === null || hasDemoFlag === null) {
    return (
      <main className="auth-page" aria-busy="true">
        <p className="auth-card__hint">Carregando workspace...</p>
      </main>
    );
  }

  if (!isAuthenticated && !hasToken && !hasDemoFlag && !isLocalMode) {
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
