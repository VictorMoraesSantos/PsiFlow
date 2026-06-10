import { useState } from 'react';
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
import { getAccessToken } from './services/api';

export function App() {
  const [currentPage, setCurrentPage] = useState<PageId>('dashboard');
  const [isAuthenticated, setIsAuthenticated] = useState(() => Boolean(getAccessToken()) || import.meta.env.DEV);
  const { data, isLoading, setData, reload } = useDashboardData();

  if (!isAuthenticated) {
    return <LoginPage onAuthenticated={() => { setIsAuthenticated(true); void reload(); }} />;
  }

  return (
    <ToastProvider>
      <AppShell currentPage={currentPage} onNavigate={setCurrentPage}>
        {currentPage === 'dashboard' && <DashboardPage data={data} isLoading={isLoading} />}
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
