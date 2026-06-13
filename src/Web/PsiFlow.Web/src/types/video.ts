export type VideoRoom = {
  id: number;
  tenantId?: number;
  sessionId: number;
  name: string;
  provider: string;
  urlEncrypted?: string;
  urlHash?: string;
  instructions?: string | null;
  createdBy?: number;
  status: 'Ativo' | 'Pausado' | 'Inativo';
};

export type VideoRoomLink = {
  name: string;
  provider: string;
  url: string;
  instructions?: string | null;
};

export type VideoRoomClick = {
  id: number;
  sessionId: number;
  clickedAt: string;
  ipHash?: string;
  userAgent?: string;
};

export type DefaultVideoLink = {
  provider: string;
  url: string;
};
