import { useEffect, useState } from "react";
import { getContests, syncContests, getStats } from "../api";
import type { Contest, ContestStats } from "../types";

const PLATFORMS = ["All", "Codeforces", "LeetCode"];
const PAGE_SIZE = 20;

const statusLabel = (s: number) => ["Upcoming", "Ongoing", "Finished"][s] ?? "Unknown";
const statusColor = (s: number) =>
  ["bg-blue-100 text-blue-800", "bg-green-100 text-green-800", "bg-gray-100 text-gray-600"][s] ?? "";

export default function ContestsPage() {
  const [contests, setContests] = useState<Contest[]>([]);
  const [platform, setPlatform] = useState("All");
  const [search, setSearch] = useState("");
  const [debouncedSearch, setDebouncedSearch] = useState("");
  const [page, setPage] = useState(1);
  const [total, setTotal] = useState(0);
  const [loading, setLoading] = useState(false);
  const [syncing, setSyncing] = useState(false);
  const [stats, setStats] = useState<ContestStats | null>(null);

  const totalPages = Math.ceil(total / PAGE_SIZE);

  useEffect(() => {
    const timer = setTimeout(() => setDebouncedSearch(search), 300);
    return () => clearTimeout(timer);
  }, [search]);

  const load = async (p: string, s: string, pg: number) => {
    setLoading(true);
    const data = await getContests(p === "All" ? undefined : p, s || undefined, pg, PAGE_SIZE);
    setContests(data.items);
    setTotal(data.total);
    setLoading(false);
  };

  useEffect(() => {
    load(platform, debouncedSearch, page);
  }, [platform, debouncedSearch, page]);

  useEffect(() => {
    getStats().then(setStats);
  }, []);

  const handlePlatform = (p: string) => {
    setPlatform(p);
    setSearch("");
    setPage(1);
  };

  const handleSearch = (s: string) => {
    setSearch(s);
    setPage(1);
  };

  const handleSync = async () => {
    setSyncing(true);
    await syncContests();
    await load(platform, debouncedSearch, page);
    const updated = await getStats();
    setStats(updated);
    setSyncing(false);
  };

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-bold text-gray-900">Contests</h1>
        <button
          onClick={handleSync}
          disabled={syncing}
          className="px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 disabled:opacity-50 text-sm font-medium"
        >
          {syncing ? "Syncing..." : "Sync Now"}
        </button>
      </div>

      {stats && (
        <div className="grid grid-cols-2 sm:grid-cols-4 gap-3 mb-6">
          {[
            { label: "Total", value: stats.total },
            { label: "Upcoming", value: stats.upcoming },
            { label: "Ongoing", value: stats.ongoing },
            { label: "Finished", value: stats.finished },
          ].map(({ label, value }) => (
            <div key={label} className="bg-white border border-gray-200 rounded-xl p-4 text-center">
              <p className="text-2xl font-bold text-indigo-600">{value}</p>
              <p className="text-xs text-gray-500 mt-1">{label}</p>
            </div>
          ))}
        </div>
      )}

      <div className="flex flex-col sm:flex-row gap-3 mb-6">
        <div className="flex gap-2 flex-wrap">
          {PLATFORMS.map((p) => (
            <button
              key={p}
              onClick={() => handlePlatform(p)}
              className={`px-3 py-1 rounded-full text-sm font-medium border transition-colors ${
                platform === p
                  ? "bg-indigo-600 text-white border-indigo-600"
                  : "bg-white text-gray-600 border-gray-300 hover:border-indigo-400"
              }`}
            >
              {p}
            </button>
          ))}
        </div>
        <input
          type="text"
          placeholder="Search contests..."
          value={search}
          onChange={(e) => handleSearch(e.target.value)}
          className="flex-1 px-3 py-1.5 text-sm border border-gray-300 rounded-lg focus:outline-none focus:border-indigo-400"
        />
      </div>

      {loading ? (
        <p className="text-gray-500">Loading...</p>
      ) : contests.length === 0 ? (
        <p className="text-gray-500 italic">No contests found. Try syncing or adjusting filters.</p>
      ) : (
        <>
          <div className="grid gap-4">
            {contests.map((c) => (
              <div key={c.id} className="bg-white rounded-xl border border-gray-200 p-5">
                <div className="flex items-center gap-2 mb-1">
                  <span className={`text-xs font-medium px-2 py-0.5 rounded-full ${statusColor(c.status)}`}>
                    {statusLabel(c.status)}
                  </span>
                  <span className="text-xs text-gray-400 font-medium">{c.platform}</span>
                </div>
                <a href={c.url} target="_blank" rel="noreferrer" className="text-gray-900 font-semibold hover:text-indigo-600">
                  {c.name}
                </a>
                <p className="text-sm text-gray-500 mt-1">
                  {new Date(c.startTime).toLocaleString()} → {new Date(c.endTime).toLocaleString()}
                </p>
              </div>
            ))}
          </div>

          <div className="flex items-center justify-between mt-6">
            <p className="text-sm text-gray-500">{total} contests found</p>
            <div className="flex items-center gap-2">
              <button
                onClick={() => setPage((p) => p - 1)}
                disabled={page === 1}
                className="px-3 py-1.5 text-sm border border-gray-300 rounded-lg disabled:opacity-40 hover:bg-gray-50"
              >
                ← Prev
              </button>
              <span className="text-sm text-gray-600">Page {page} of {totalPages}</span>
              <button
                onClick={() => setPage((p) => p + 1)}
                disabled={page >= totalPages}
                className="px-3 py-1.5 text-sm border border-gray-300 rounded-lg disabled:opacity-40 hover:bg-gray-50"
              >
                Next →
              </button>
            </div>
          </div>
        </>
      )}
    </div>
  );
}
