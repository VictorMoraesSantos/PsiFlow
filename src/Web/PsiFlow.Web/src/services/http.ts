const tokenKey = 'psiflow.accessToken';
const refreshTokenKey = 'psiflow.refreshToken';
export const localFallbackEvent = 'psiflow:local-fallback';

export class ApiError extends Error {
  readonly status: number;

  constructor(message: string, status: number) {
    super(message);
    this.name = 'ApiError';
    this.status = status;
  }
}

export function isLocalFallbackStatus(error: unknown): boolean {
  return error instanceof ApiError && (error.status === 401 || error.status === 405);
}

export function announceLocalFallback(status?: number): void {
  if (typeof window === 'undefined') return;
  window.dispatchEvent(new CustomEvent(localFallbackEvent, { detail: { status } }));
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

type RequestOptions = {
  query?: Record<string, string | number | boolean | null | undefined>;
  signal?: AbortSignal;
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
    if (Array.isArray(record.value)) return record.value;
    if (record.data && typeof record.data === 'object') return record.data;
  }
  return payload;
};

export async function request<T = unknown>(method: string, path: string, body?: unknown, options: RequestOptions = {}): Promise<T> {
  const headers = new Headers({ Accept: 'application/json' });
  const token = getAccessToken();
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

  const response = await fetch(buildUrl(path, options.query), init);
  if (response.status === 204) return undefined as T;

  const payload = await safeParse(response);

  if (!response.ok) {
    if (response.status === 401 || response.status === 405) announceLocalFallback(response.status);
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
