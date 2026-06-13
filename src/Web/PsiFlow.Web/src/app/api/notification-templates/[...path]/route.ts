import { proxyRequest } from '../../../../lib/api/client';

export const runtime = 'nodejs';
export const dynamic = 'force-dynamic';

const handler = async (request: Request) => proxyRequest({ request, service: 'notifications' });

export { handler as GET, handler as POST, handler as PUT, handler as PATCH, handler as DELETE, handler as OPTIONS };
