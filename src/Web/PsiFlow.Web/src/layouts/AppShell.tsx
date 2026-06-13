'use client';

import { Bell, CalendarDays, FileText, HeartPulse, Home, LogOut, MessageSquareText, PanelLeft, Settings, UserRound, Video } from 'lucide-react';
import Link from 'next/link';
import { usePathname } from 'next/navigation';
import { useEffect, useRef, useState } from 'react';
import { useFocusTrap } from '../hooks/useFocusTrap';
import { useApp } from '../state/AppContext';

const navItems = [
  { id: 'dashboard', href: '/dashboard', label: 'Visao geral', icon: Home },
  { id: 'patients', href: '/patients', label: 'Pacientes', icon: UserRound },
  { id: 'agenda', href: '/agenda', label: 'Agenda', icon: CalendarDays },
  { id: 'sessions', href: '/sessions', label: 'Sessoes', icon: HeartPulse },
  { id: 'records', href: '/records', label: 'Prontuarios', icon: FileText },
  { id: 'notifications', href: '/notifications', label: 'Notificacoes', icon: MessageSquareText },
  { id: 'online', href: '/online', label: 'Atendimento online', icon: Video },
  { id: 'settings', href: '/settings', label: 'Perfil e configuracoes', icon: Settings },
] as const;

export type PageId = (typeof navItems)[number]['id'];

function formatTopbarDate(date: Date): string {
  return date.toLocaleDateString('pt-BR', {
    weekday: 'long',
    day: '2-digit',
    month: 'long',
  }).replace(/^./, (char) => char.toUpperCase());
}

export function AppShell({ children }: { children: React.ReactNode }) {
  const { isLocalMode, dismissLocalMode, logout } = useApp();
  const pathname = usePathname();
  const [isNavOpen, setIsNavOpen] = useState(false);
  const [today] = useState(() => formatTopbarDate(new Date()));
  const sidebarRef = useRef<HTMLElement>(null);
  const menuButtonRef = useRef<HTMLButtonElement>(null);
  useFocusTrap(sidebarRef, isNavOpen, () => setIsNavOpen(false));

  useEffect(() => {
    if (!isNavOpen) return;
    const onResize = () => {
      if (window.innerWidth > 920) setIsNavOpen(false);
    };
    window.addEventListener('resize', onResize);
    return () => window.removeEventListener('resize', onResize);
  }, [isNavOpen]);

  useEffect(() => {
    setIsNavOpen(false);
  }, [pathname]);

  function closeSidebar() {
    setIsNavOpen(false);
    requestAnimationFrame(() => menuButtonRef.current?.focus());
  }

  return (
    <div className="app-shell">
      <a href="#main-content" className="skip-link">Pular para o conteudo principal</a>
      <aside
        id="primary-sidebar"
        ref={sidebarRef}
        className={`sidebar ${isNavOpen ? 'sidebar--open' : ''}`}
        aria-label="Navegacao principal"
      >
        <div className="brand">
          <div className="brand__mark" aria-hidden="true">P</div>
          <div>
            <strong>PsiFlow</strong>
            <span>Clinica integrada</span>
          </div>
        </div>
        <nav aria-label="Secoes do workspace">
          {navItems.map((item) => {
            const Icon = item.icon;
            const isActive = pathname === item.href || pathname?.startsWith(`${item.href}/`);
            return (
              <Link
                key={item.id}
                href={item.href}
                className={isActive ? 'nav-item nav-item--active' : 'nav-item'}
                aria-current={isActive ? 'page' : undefined}
                onClick={() => setIsNavOpen(false)}
              >
                <Icon aria-hidden="true" size={18} />
                <span>{item.label}</span>
              </Link>
            );
          })}
        </nav>
        <div className="sidebar__note">
          <span>Proxima sessao</span>
          <strong>14:30, online</strong>
          <p>Revise o rascunho antes de abrir a sala.</p>
        </div>
        {isNavOpen ? (
          <button className="button sidebar__close" type="button" onClick={closeSidebar}>
            Fechar menu
          </button>
        ) : null}
      </aside>

      <div className="workspace">
        <header className="topbar">
          <button
            ref={menuButtonRef}
            className="icon-button topbar__menu"
            type="button"
            aria-label={isNavOpen ? 'Fechar menu' : 'Abrir menu'}
            aria-expanded={isNavOpen}
            aria-controls="primary-sidebar"
            onClick={() => setIsNavOpen((value) => !value)}
          >
            <PanelLeft aria-hidden="true" size={20} />
          </button>
          <div>
            <span className="topbar__date">{today}</span>
            <h1>Workspace clinico</h1>
          </div>
          <div className="topbar__actions">
            <button className="icon-button" type="button" aria-label="Abrir notificacoes">
              <Bell aria-hidden="true" size={19} />
            </button>
            <Link href="/settings" className="profile-chip" aria-label="Abrir perfil e configuracoes">
              <span aria-hidden="true">DV</span>
              <strong>Dra. Vitoria</strong>
            </Link>
            <button className="icon-button" type="button" aria-label="Sair da conta" onClick={logout}>
              <LogOut aria-hidden="true" size={19} />
            </button>
          </div>
        </header>
        {isLocalMode ? (
          <div className="sync-banner" role="status">
            <strong>Modo local ativo.</strong>
            <span>Algumas alteracoes estao apenas neste navegador porque o backend respondeu sem autenticacao ou sem suporte ao metodo.</span>
            <button type="button" onClick={dismissLocalMode}>Ocultar aviso</button>
          </div>
        ) : null}
        <main id="main-content" tabIndex={-1}>
          {children}
        </main>
      </div>
    </div>
  );
}
