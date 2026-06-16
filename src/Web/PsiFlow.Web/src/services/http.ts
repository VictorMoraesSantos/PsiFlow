const tokenKey = 'psiflow.accessToken';
const refreshTokenKey = 'psiflow.refreshToken';
const demoModeKey = 'psiflow.demoMode';
export const localFallbackEvent = 'psiflow:local-fallback';
export const sessionExpiredEvent = 'psiflow:session-expired';

export class ApiError extends Error {
  readonly status: number;

  constructor(message: string, status: number) {
    super(message);
    this.name = 'ApiError';
    this.status = status;
  }
}

export function isBackendUnreachable(error: unknown): boolean {
  if (!(error instanceof ApiError)) return false;
  return error.status === 0 || error.status === 502 || error.status === 503 || error.status === 504;
}

export function isLocalFallbackStatus(error: unknown): boolean {
  return isBackendUnreachable(error);
}

export function announceLocalFallback(status?: number): void {
  if (typeof window === 'undefined') return;
  window.dispatchEvent(new CustomEvent(localFallbackEvent, { detail: { status } }));
}

export function announceSessionExpired(status?: number): void {
  if (typeof window === 'undefined') return;
  window.dispatchEvent(new CustomEvent(sessionExpiredEvent, { detail: { status } }));
}

export function getAccessToken(): string | null {
  if (typeof window === 'undefined') return null;
  return localStorage.getItem(tokenKey);
}

export function getRefreshToken(): string | null {
  if (typeof window === 'undefined') return null;
  return localStorage.getItem(refreshTokenKey);
}

export function setSessionTokens(accessToken: string, refreshToken?: string): void {
  if (typeof window === 'undefined') return;
  localStorage.setItem(tokenKey, accessToken);
  if (refreshToken) localStorage.setItem(refreshTokenKey, refreshToken);
}

export function clearSessionTokens(): void {
  if (typeof window === 'undefined') return;
  localStorage.removeItem(tokenKey);
  localStorage.removeItem(refreshTokenKey);
}

export function setDemoMode(enabled: boolean): void {
  if (typeof window === 'undefined') return;
  if (enabled) localStorage.setItem(demoModeKey, '1');
  else localStorage.removeItem(demoModeKey);
}

export function isDemoMode(): boolean {
  if (typeof window === 'undefined') return false;
  return localStorage.getItem(demoModeKey) === '1';
}

type RequestOptions = {
  query?: Record<string, string | number | boolean | null | undefined>;
  signal?: AbortSignal;
  skipAuth?: boolean;
};

const buildUrl = (path: string, query?: RequestOptions['query']): string => {
  if (!path.startsWith('/')) throw new Error(`API path must start with '/': ${path}`);
  const search = new URLSearchParams();
  if (query) {
    for (const [key, value] of Object.entries(query)) {
      if (value === null || value === undefined || value === '') continue;
      search.set(key, String(value));
    }
  }
  const tail = search.toString();
  return tail ? `${path}?${tail}` : path;
};

const safeParse = async (response: Response): Promise<unknown> => {
  const text = await response.text();
  if (!text) return null;
  try {
    return JSON.parse(text);
  } catch {
    return text;
  }
};

const extractErrorMessage = (payload: unknown, fallback: string): string => {
  if (!payload) return fallback;
  if (typeof payload === 'string') return payload;
  if (typeof payload !== 'object') return fallback;
  const record = payload as Record<string, unknown>;
  return (record.error as string) ?? (record.detail as string) ?? (record.title as string) ?? (record.message as string) ?? fallback;
};

const unwrap = (payload: unknown): unknown => {
  if (Array.isArray(payload)) return payload;
  if (payload && typeof payload === 'object') {
    const record = payload as Record<string, unknown>;
    if (Array.isArray(record.items)) return record.items;
    if (record.value && (Array.isArray(record.value) || typeof record.value === 'object')) return record.value;
    if (record.data && typeof record.data === 'object') return record.data;
  }
  return payload;
};

let refreshInFlight: Promise<string | null> | null = null;

async function refreshAccessToken(): Promise<string | null> {
  if (typeof window === 'undefined') return null;
  if (refreshInFlight) return refreshInFlight;

  const refreshToken = getRefreshToken();
  if (!refreshToken) return null;

  refreshInFlight = (async () => {
    try {
      const response = await fetch('/api/auth/v1/auth/refresh', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json', Accept: 'application/json' },
        body: JSON.stringify({ refreshToken }),
        cache: 'no-store',
        credentials: 'same-origin',
      });
      if (!response.ok) return null;
      const payload = (await safeParse(response)) as Record<string, unknown> | null;
      const newAccess =
        (payload?.accessToken as string | undefined) ??
        (payload?.access_token as string | undefined) ??
        (payload?.token as string | undefined);
      const newRefresh =
        (payload?.refreshToken as string | undefined) ?? (payload?.refresh_token as string | undefined);
      if (!newAccess) return null;
      setSessionTokens(newAccess, newRefresh ?? refreshToken);
      return newAccess;
    } catch {
      return null;
    } finally {
      refreshInFlight = null;
    }
  })();

  return refreshInFlight;
}

async function performFetch(
  method: string,
  path: string,
  body: unknown,
  options: RequestOptions,
  tokenOverride?: string | null,
): Promise<Response> {
  const headers = new Headers({ Accept: 'application/json' });
  const token = options.skipAuth ? null : tokenOverride !== undefined ? tokenOverride : getAccessToken();
  if (token) headers.set('Authorization', `Bearer ${token}`);
  if (body !== undefined) headers.set('Content-Type', 'application/json');

  const init: RequestInit = {
    method,
    headers,
    cache: 'no-store',
    credentials: 'same-origin',
  };
  if (body !== undefined) init.body = JSON.stringify(body);
  if (options.signal) init.signal = options.signal;

  return fetch(buildUrl(path, options.query), init);
}

export async function request<T = unknown>(method: string, path: string, body?: unknown, options: RequestOptions = {}): Promise<T> {
  let response: Response;
  try {
    response = await performFetch(method, path, body, options);
  } catch (networkError) {
    announceLocalFallback(0);
    throw new ApiError(networkError instanceof Error ? networkError.message : 'Falha de rede.', 0);
  }

  if (response.status === 401 && !options.skipAuth && getAccessToken()) {
    const newToken = await refreshAccessToken();
    if (newToken) {
      try {
        response = await performFetch(method, path, body, options, newToken);
      } catch (networkError) {
        announceLocalFallback(0);
        throw new ApiError(networkError instanceof Error ? networkError.message : 'Falha de rede.', 0);
      }
    } else {
      clearSessionTokens();
      announceSessionExpired(401);
    }
  }

  if (response.status === 204) return undefined as T;

  const payload = await safeParse(response);

  if (!response.ok) {
    if (response.status >= 500 || response.status === 0) announceLocalFallback(response.status);
    throw new ApiError(extractErrorMessage(payload, `Resposta ${response.status} do servidor.`), response.status);
  }

  return unwrap(payload) as T;
}

export const api = {
  get: <T = unknown>(path: string, options?: RequestOptions) => request<T>('GET', path, undefined, options),
  post: <T = unknown>(path: string, body?: unknown, options?: RequestOptions) => request<T>('POST', path, body, options),
  put: <T = unknown>(path: string, body?: unknown, options?: RequestOptions) => request<T>('PUT', path, body, options),
  patch: <T = unknown>(path: string, body?: unknown, options?: RequestOptions) => request<T>('PATCH', path, body, options),
  delete: <T = unknown>(path: string, options?: RequestOptions) => request<T>('DELETE', path, undefined, options),
};
