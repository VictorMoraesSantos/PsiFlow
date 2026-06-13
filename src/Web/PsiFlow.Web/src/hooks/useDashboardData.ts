'use client';

import { useCallback, useEffect, useState, type Dispatch, type SetStateAction } from 'react';
import { fallbackData } from '../data/fallbackData';
import { loadDashboardData } from '../services/api';
import type { DashboardData } from '../types';

type DashboardState = {
  data: DashboardData;
  isLoading: boolean;
  setData: Dispatch<SetStateAction<DashboardData>>;
  reload: () => Promise<void>;
};

export function useDashboardData(enabled: boolean): DashboardState {
  const [data, setData] = useState<DashboardData>(fallbackData);
  const [isLoading, setIsLoading] = useState(enabled);

  const reload = useCallback(async () => {
    setIsLoading(true);
    try {
      const nextData = await loadDashboardData();
      setData(nextData);
    } finally {
      setIsLoading(false);
    }
  }, []);

  useEffect(() => {
    if (!enabled) {
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
  }, [enabled]);

  return { data, isLoading, setData, reload };
}
