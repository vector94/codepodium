export interface Contest {
  id: number;
  externalId: string;
  name: string;
  platform: string;
  startTime: string;
  endTime: string;
  url: string;
  status: number;
}

export interface ContestStats {
  total: number;
  upcoming: number;
  ongoing: number;
  finished: number;
  byPlatform: Record<string, number>;
}
