import { useEffect, useState, type Dispatch, type SetStateAction } from 'react';
import { fallbackData } from '../data/fallbackData';
import { loadDashboardData } from '../services/api';
import type { DashboardData } from '../types';

type DashboardState = {
  data: DashboardData;
  isLoading: boolean;
  setData: Dispatch<SetStateAction<DashboardData>>;
  reload: () => Promise<void>;
};

export function useDashboardData(): DashboardState {
  const [data, setData] = useState<DashboardData>(fallbackData);
  const [isLoading, setIsLoading] = useState(true);

  async function reload() {
    setIsLoading(true);
    const nextData = await loadDashboardData();
    setData(nextData);
    setIsLoading(false);
  }

  useEffect(() => {
    let isCurrent = true;

    loadDashboardData().then((data) => {
      if (!isCurrent) return;
      setData(data);
      setIsLoading(false);
    });

    return () => {
      isCurrent = false;
    };
  }, []);

  return { data, isLoading, setData, reload };
}
