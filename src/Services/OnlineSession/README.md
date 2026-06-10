# OnlineSession

## Responsabilidade
Gerenciar link externo de videochamada por sessão, instruções pré-sessão e registro de clique. **Não** criar sala própria de vídeo no MVP.

## Como rodar
1. `docker compose -f deploy/docker/docker-compose.dev.yml up -d`
2. `dotnet ef database update --project src/Services/OnlineSession/OnlineSession.Infrastructure --startup-project src/Services/OnlineSession/OnlineSession.API`
3. `dotnet run --project src/Services/OnlineSession/OnlineSession.API`

## Endpoints principais
- `PUT/GET /v1/sessions/{sessionId}/video-room`
- `POST /v1/sessions/{sessionId}/video-room/click`
- `GET /v1/sessions/{sessionId}/video-room/clicks`
- `PUT/GET /v1/video-settings/default-link`

## Validações
URL deve ser HTTPS; provedores aceitos: `zoom`, `google_meet`, `external`. Rejeita `javascript:`, `data:`, `file:`, HTTP sem TLS.

## Eventos publicados
VideoRoomLinked, VideoRoomClicked, DefaultVideoLinkUpdated

## Eventos consumidos
SessionCreated, AppointmentScheduled, SessionCanceled
