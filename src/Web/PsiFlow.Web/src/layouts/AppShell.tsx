import { Bell, CalendarDays, FileText, HeartPulse, Home, LogOut, MessageSquareText, PanelLeft, Settings, UserRound, Video } from 'lucide-react';
import { useEffect, useRef, useState } from 'react';
import { useFocusTrap } from '../hooks/useFocusTrap';

const navItems = [
  { id: 'dashboard', label: 'Visao geral', icon: Home },
  { id: 'patients', label: 'Pacientes', icon: UserRound },
  { id: 'agenda', label: 'Agenda', icon: CalendarDays },
  { id: 'sessions', label: 'Sessoes', icon: HeartPulse },
  { id: 'records', label: 'Prontuarios', icon: FileText },
  { id: 'notifications', label: 'Notificacoes', icon: MessageSquareText },
  { id: 'online', label: 'Atendimento online', icon: Video },
  { id: 'settings', label: 'Perfil e configuracoes', icon: Settings },
] as const;

export type PageId = (typeof navItems)[number]['id'];

type AppShellProps = {
  currentPage: PageId;
  onNavigate: (page: PageId) => void;
  onLogout: () => void;
  children: React.ReactNode;
};

function formatTopbarDate(date: Date): string {
  return date.toLocaleDateString('pt-BR', {
    weekday: 'long',
    day: '2-digit',
    month: 'long',
  }).replace(/^./, (char) => char.toUpperCase());
}

export function AppShell({ currentPage, onNavigate, onLogout, children }: AppShellProps) {
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

  function navigate(page: PageId) {
    onNavigate(page);
    setIsNavOpen(false);
    requestAnimationFrame(() => menuButtonRef.current?.focus());
  }

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
            const isActive = currentPage === item.id;
            return (
              <button
                key={item.id}
                className={isActive ? 'nav-item nav-item--active' : 'nav-item'}
                type="button"
                aria-current={isActive ? 'page' : undefined}
                onClick={() => navigate(item.id)}
              >
                <Icon aria-hidden="true" size={18} />
                <span>{item.label}</span>
              </button>
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
            <button className="profile-chip" type="button" aria-label="Abrir perfil e configuracoes" onClick={() => navigate('settings')}>
              <span aria-hidden="true">DV</span>
              <strong>Dra. Vitoria</strong>
            </button>
            <button className="icon-button" type="button" aria-label="Sair da conta" onClick={onLogout}>
              <LogOut aria-hidden="true" size={19} />
            </button>
          </div>
        </header>
        <main id="main-content" tabIndex={-1}>
          {children}
        </main>
      </div>
    </div>
  );
}
