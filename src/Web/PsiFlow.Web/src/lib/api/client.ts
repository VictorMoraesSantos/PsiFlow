import 'server-only';

import { backendByPath, backendConfig, type BackendService } from './config';

export class UpstreamError extends Error {
  constructor(message: string, public readonly status: number, public readonly upstream: BackendService) {
    super(message);
    this.name = 'UpstreamError';
  }
}

export class UpstreamTimeoutError extends Error {
  constructor(public readonly upstream: BackendService) {
    super(`Upstream ${upstream} timed out.`);
    this.name = 'UpstreamTimeoutError';
  }
}

export type ProxyOptions = {
  request: Request;
  service: BackendService;
  searchParams?: Record<string, string | number | boolean | null | undefined>;
};

const passthroughRequestHeaders = (request: Request): HeadersInit => {
  const headers = new Headers();
  const allowed = ['accept', 'accept-language', 'authorization', 'content-type', 'if-match', 'if-none-match', 'x-request-id'];
  for (const name of allowed) {
    const value = request.headers.get(name);
    if (value) headers.set(name, value);
  }
  return headers;
};

const buildUpstreamUrl = (service: BackendService, path: string[], searchParams?: ProxyOptions['searchParams']): string => {
  const base = backendConfig[service].replace(/\/$/, '');
  const tail = path.map((segment) => encodeURIComponent(segment)).join('/');
  const url = new URL(`${base}/${tail}`);
  if (searchParams) {
    for (const [key, value] of Object.entries(searchParams)) {
      if (value === null || value === undefined || value === '') continue;
      url.searchParams.set(key, String(value));
    }
  }
  return url.toString();
};

const extractErrorMessage = (payload: unknown, fallback: string): string => {
  if (!payload) return fallback;
  if (typeof payload === 'string') return payload;
  if (typeof payload !== 'object') return fallback;
  const record = payload as Record<string, unknown>;
  return (record.error as string) ?? (record.detail as string) ?? (record.title as string) ?? (record.message as string) ?? fallback;
};

const safeJson = async (response: Response): Promise<unknown> => {
  const text = await response.text();
  if (!text) return null;
  try {
    return JSON.parse(text);
  } catch {
    return text;
  }
};

export async function proxyRequest({ request, service, searchParams }: ProxyOptions): Promise<Response> {
  const url = new URL(request.url);
  const tailSegments = url.pathname.replace(/^\/api\/[^/]+\/?/, '').split('/').filter(Boolean);

  const upstreamUrl = buildUpstreamUrl(service, tailSegments, searchParams);
  const init: RequestInit = {
    method: request.method,
    headers: passthroughRequestHeaders(request),
    redirect: 'manual',
    cache: 'no-store',
  };

  if (request.method !== 'GET' && request.method !== 'HEAD') {
    init.body = await request.arrayBuffer();
  }

  const controller = new AbortController();
  const timeout = setTimeout(() => controller.abort(), backendConfig.requestTimeoutMs);
  init.signal = controller.signal;

  try {
    const upstream = await fetch(upstreamUrl, init);
    clearTimeout(timeout);

    const payload = await safeJson(upstream);
    const responseHeaders = new Headers();
    const passthrough = ['content-type', 'cache-control', 'etag', 'x-request-id'];
    for (const name of passthrough) {
      const value = upstream.headers.get(name);
      if (value) responseHeaders.set(name, value);
    }

    if (upstream.status === 204) {
      return new Response(null, { status: 204, headers: responseHeaders });
    }

    if (!upstream.ok) {
      const message = extractErrorMessage(payload, `Upstream ${service} responded with ${upstream.status}.`);
      return Response.json({ error: message, upstream: service, status: upstream.status }, { status: upstream.status, headers: responseHeaders });
    }

    return new Response(JSON.stringify(payload), { status: upstream.status, headers: responseHeaders });
  } catch (error) {
    clearTimeout(timeout);
    if (error instanceof DOMException && error.name === 'AbortError') {
      return Response.json({ error: 'Upstream timeout.', upstream: service, status: 504 }, { status: 504 });
    }
    return Response.json({ error: error instanceof Error ? error.message : 'Upstream connection failed.', upstream: service, status: 502 }, { status: 502 });
  }
}

export function resolveService(pathname: string): BackendService | null {
  for (const prefix of Object.keys(backendByPath).sort((a, b) => b.length - a.length)) {
    if (pathname === prefix || pathname.startsWith(`${prefix}/`)) {
      return backendByPath[prefix];
    }
  }
  return null;
}
