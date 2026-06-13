'use client';

import { useRouter } from 'next/navigation';
import { useEffect } from 'react';
import { AppShell } from '../../layouts/AppShell';
import { ToastProvider } from '../../components/Toast';
import { useApp } from '../../state/AppContext';

export default function ProtectedLayout({ children }: { children: React.ReactNode }) {
  const { isAuthenticated, isHydrating } = useApp();
  const router = useRouter();

  useEffect(() => {
    if (!isHydrating && !isAuthenticated) {
      router.replace('/login');
    }
  }, [isAuthenticated, isHydrating, router]);

  if (isHydrating || !isAuthenticated) {
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
