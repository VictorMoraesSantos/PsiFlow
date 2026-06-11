import { useEffect, useState } from 'react';
import { ToastProvider } from './components/Toast';
import { AppShell, type PageId } from './layouts/AppShell';
import { AgendaPage } from './pages/AgendaPage';
import { DashboardPage } from './pages/DashboardPage';
import { LoginPage } from './pages/LoginPage';
import { NotificationsPage } from './pages/NotificationsPage';
import { OnlineSessionPage } from './pages/OnlineSessionPage';
import { PatientsPage } from './pages/PatientsPage';
import { RecordsPage } from './pages/RecordsPage';
import { SessionsPage } from './pages/SessionsPage';
import { useDashboardData } from './hooks/useDashboardData';
import { clearSessionTokens, getAccessToken, localFallbackEvent } from './services/api';

export function App() {
  const [currentPage, setCurrentPage] = useState<PageId>('dashboard');
  const [isAuthenticated, setIsAuthenticated] = useState(() => Boolean(getAccessToken()));
  const [isLocalMode, setIsLocalMode] = useState(false);
  const { data, isLoading, setData } = useDashboardData(isAuthenticated);

  useEffect(() => {
    const onLocalFallback = () => setIsLocalMode(true);
    window.addEventListener(localFallbackEvent, onLocalFallback);
    return () => window.removeEventListener(localFallbackEvent, onLocalFallback);
  }, []);

  function logout() {
    clearSessionTokens();
    setIsAuthenticated(false);
    setCurrentPage('dashboard');
  }

  if (!isAuthenticated) {
    return <LoginPage onAuthenticated={() => setIsAuthenticated(true)} />;
  }

  return (
      <ToastProvider>
      {isLocalMode ? (
        <div className="sync-banner" role="status">
          <strong>Modo local ativo.</strong>
          <span>Algumas alterações estão apenas neste navegador porque o backend respondeu sem autenticação ou sem suporte ao método.</span>
          <button type="button" onClick={() => setIsLocalMode(false)}>Ocultar aviso</button>
        </div>
      ) : null}
      <AppShell currentPage={currentPage} onNavigate={setCurrentPage} onLogout={logout}>
        {currentPage === 'dashboard' && <DashboardPage data={data} isLoading={isLoading} onNavigate={setCurrentPage} />}
        {currentPage === 'patients' && <PatientsPage data={data} onPatientsChange={(patients) => setData((current) => ({ ...current, patients }))} />}
        {currentPage === 'agenda' && <AgendaPage data={data} onAppointmentsChange={(appointments) => setData((current) => ({ ...current, appointments }))} />}
        {currentPage === 'sessions' && <SessionsPage data={data} onSessionsChange={(sessions) => setData((current) => ({ ...current, sessions }))} />}
        {currentPage === 'records' && <RecordsPage data={data} onRecordsChange={(records) => setData((current) => ({ ...current, records }))} />}
        {currentPage === 'notifications' && <NotificationsPage templates={data.templates} onTemplatesChange={(templates) => setData((current) => ({ ...current, templates }))} />}
        {currentPage === 'online' && <OnlineSessionPage data={data} onVideoRoomsChange={(videoRooms) => setData((current) => ({ ...current, videoRooms }))} />}
      </AppShell>
    </ToastProvider>
  );
}
