'use client';

import { createContext, useCallback, useContext, useEffect, useMemo, useState, type Dispatch, type SetStateAction } from 'react';
import { fallbackData } from '../data/fallbackData';
import { login as authLogin, logoutServer, type LoginResult } from '../services/auth';
import {
  clearSessionTokens,
  getAccessToken,
  isDemoMode as readDemoMode,
  localFallbackEvent,
  sessionExpiredEvent,
  setDemoMode as persistDemoMode,
} from '../services/http';
import { loadDashboardData } from '../services/api';
import type { DashboardData } from '../types';

type AppContextValue = {
  data: DashboardData;
  isLoading: boolean;
  isAuthenticated: boolean;
  isHydrating: boolean;
  isLocalMode: boolean;
  setData: Dispatch<SetStateAction<DashboardData>>;
  dismissLocalMode: () => void;
  login: (email: string, password: string) => Promise<LoginResult>;
  enterDemoMode: () => void;
  logout: () => void;
};

const AppContext = createContext<AppContextValue | null>(null);

const readToken = (): string | null => {
  try {
    return getAccessToken();
  } catch {
    return null;
  }
};

export function AppProvider({ children }: { children: React.ReactNode }) {
  const [data, setData] = useState<DashboardData>(fallbackData);
  const [isLoading, setIsLoading] = useState(false);
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [isHydrating, setIsHydrating] = useState(true);
  const [isLocalMode, setIsLocalMode] = useState(false);

  useEffect(() => {
    const demo = readDemoMode();
    setIsLocalMode(demo);
    setIsAuthenticated(demo || Boolean(readToken()));
    setIsHydrating(false);
  }, []);

  useEffect(() => {
    const onLocalFallback = () => setIsLocalMode(true);
    const onSessionExpired = () => {
      clearSessionTokens();
      persistDemoMode(false);
      setIsAuthenticated(false);
      setIsLocalMode(false);
      setData(fallbackData);
      if (typeof window !== 'undefined' && window.location.pathname !== '/login') {
        window.location.assign('/login');
      }
    };
    window.addEventListener(localFallbackEvent, onLocalFallback);
    window.addEventListener(sessionExpiredEvent, onSessionExpired);
    return () => {
      window.removeEventListener(localFallbackEvent, onLocalFallback);
      window.removeEventListener(sessionExpiredEvent, onSessionExpired);
    };
  }, []);

  useEffect(() => {
    if (!isAuthenticated) {
      setData(fallbackData);
      setIsLoading(false);
      return;
    }
    if (isLocalMode && !readToken()) {
      setData(fallbackData);
      setIsLoading(false);
      return;
    }
    let isCurrent = true;
    setIsLoading(true);
    loadDashboardData()
      .then((loaded) => {
        if (!isCurrent) return;
        setData(loaded);
        setIsLoading(false);
      })
      .catch(() => {
        if (!isCurrent) return;
        setIsLoading(false);
      });
    return () => {
      isCurrent = false;
    };
  }, [isAuthenticated, isLocalMode]);

  const login = useCallback(async (email: string, password: string): Promise<LoginResult> => {
    const result = await authLogin({ email, password });
    if (result.kind === 'authenticated') {
      persistDemoMode(false);
      setIsLocalMode(false);
      setIsAuthenticated(Boolean(readToken()));
    }
    return result;
  }, []);

  const enterDemoMode = useCallback(() => {
    clearSessionTokens();
    persistDemoMode(true);
    setIsAuthenticated(true);
    setIsLocalMode(true);
    setData(fallbackData);
  }, []);

  const logout = useCallback(() => {
    if (readToken()) {
      logoutServer().catch(() => {
        // ignora falha de logout: tokens serao limpos localmente
      });
    }
    clearSessionTokens();
    persistDemoMode(false);
    setIsAuthenticated(false);
    setIsLocalMode(false);
    setData(fallbackData);
  }, []);

  const dismissLocalMode = useCallback(() => setIsLocalMode(false), []);

  const value = useMemo<AppContextValue>(
    () => ({
      data,
      isLoading,
      isAuthenticated,
      isHydrating,
      isLocalMode,
      setData,
      dismissLocalMode,
      login,
      enterDemoMode,
      logout,
    }),
    [data, isLoading, isAuthenticated, isHydrating, isLocalMode, dismissLocalMode, login, enterDemoMode, logout],
  );

  return <AppContext.Provider value={value}>{children}</AppContext.Provider>;
}

export function useApp() {
  const context = useContext(AppContext);
  if (!context) throw new Error('useApp must be used within AppProvider');
  return context;
}
