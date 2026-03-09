import axios from "axios";
import type { Contest, ContestStats } from "./types";

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL ?? "/api",
});

export const getContests = (platform?: string, search?: string, page = 1, pageSize = 20) =>
  api.get<{ items: Contest[]; total: number; page: number; pageSize: number }>(
    "/contests", {
      params: {
        ...(platform ? { platform } : {}),
        ...(search ? { search } : {}),
        page,
        pageSize,
      },
    }
  ).then((r) => r.data);

export const getUpcomingContests = () =>
  api.get<Contest[]>("/contests/upcoming").then((r) => r.data);

export const syncContests = () =>
  api.post("/contests/sync").then((r) => r.data);

export const getStats = () =>
  api.get<ContestStats>("/contests/stats").then((r) => r.data);
