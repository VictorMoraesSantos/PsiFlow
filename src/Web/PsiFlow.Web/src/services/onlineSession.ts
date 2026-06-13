import { api } from './http';
import type { DefaultVideoLink, VideoRoom, VideoRoomClick, VideoRoomLink } from '../types';

export const videoRoomsApi = {
  list: () => api.get<VideoRoom[]>('/api/video-rooms/v1/video-rooms'),
  create: (payload: VideoRoom) => api.post<VideoRoom>('/api/video-rooms/v1/video-rooms', payload),
  update: (id: number, payload: VideoRoom) => api.put<VideoRoom>(`/api/video-rooms/v1/video-rooms/${id}`, payload),
  remove: (id: number) => api.delete<unknown>(`/api/video-rooms/v1/video-rooms/${id}`),
  setSessionLink: (sessionId: number, payload: VideoRoomLink) =>
    api.put<unknown>(`/api/video-rooms/v1/sessions/${sessionId}/video-room`, payload),
  getSessionLink: (sessionId: number) =>
    api.get<VideoRoomLink>(`/api/video-rooms/v1/sessions/${sessionId}/video-room`),
  sessionClicks: (sessionId: number) =>
    api.get<VideoRoomClick[]>(`/api/video-rooms/v1/sessions/${sessionId}/video-room/clicks`),
};

export const videoSettingsApi = {
  getDefaultLink: () => api.get<DefaultVideoLink>('/api/video-rooms/v1/video-settings/default-link'),
  setDefaultLink: (payload: DefaultVideoLink) =>
    api.put<unknown>('/api/video-rooms/v1/video-settings/default-link', payload),
};
