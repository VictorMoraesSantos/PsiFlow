'use client';

import { createContext, useCallback, useContext, useEffect, useMemo, useState, type Dispatch, type SetStateAction } from 'react';
import { fallbackData } from '../data/fallbackData';
import { login as authLogin } from '../services/auth';
import { clearSessionTokens, getAccessToken, localFallbackEvent, setSessionTokens } from '../services/http';
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
  login: (email: string, password: string) => Promise<void>;
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
    setIsAuthenticated(Boolean(readToken()));
    setIsHydrating(false);
  }, []);

  useEffect(() => {
    const onLocalFallback = () => setIsLocalMode(true);
    window.addEventListener(localFallbackEvent, onLocalFallback);
    return () => window.removeEventListener(localFallbackEvent, onLocalFallback);
  }, []);

  useEffect(() => {
    if (!isAuthenticated) {
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
  }, [isAuthenticated]);

  const login = useCallback(async (email: string, password: string) => {
    await authLogin({ email, password });
    setIsAuthenticated(Boolean(readToken()));
  }, []);

  const enterDemoMode = useCallback(() => {
    const demoToken = `demo.${Math.random().toString(36).slice(2, 10)}.${Date.now()}`;
    setSessionTokens(demoToken);
    setIsAuthenticated(true);
    setIsLocalMode(true);
  }, []);

  const logout = useCallback(() => {
    clearSessionTokens();
    setIsAuthenticated(false);
    setData(fallbackData);
    setIsLocalMode(false);
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
