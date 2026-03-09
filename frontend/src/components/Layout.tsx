import { NavLink, Outlet } from "react-router-dom";

export default function Layout() {
  return (
    <div className="min-h-screen bg-gray-50">
      <nav className="bg-white border-b border-gray-200 px-6 py-4 flex items-center gap-8">
        <span className="text-xl font-bold text-indigo-600 tracking-tight">CodePodium</span>
        <NavLink
          to="/"
          end
          className={({ isActive }) =>
            `text-sm font-medium transition-colors ${
              isActive ? "text-indigo-600" : "text-gray-500 hover:text-gray-900"
            }`
          }
        >
          Contests
        </NavLink>
      </nav>
      <main className="max-w-4xl mx-auto px-6 py-8">
        <Outlet />
      </main>
    </div>
  );
}
