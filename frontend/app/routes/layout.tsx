import { NavLink, Outlet } from "react-router";

const menu = [
  { to: "/perioder", label: "1.1 Perioder" },
  { to: "/organisering", label: "1.2 Organisering" },
  { to: "/adgangsstyring", label: "1.3 Adgangsstyring" },
  { to: "/personale", label: "2.3 Personale" },
];

export default function Layout() {
  return (
    <div className="flex min-h-screen">
      <nav className="w-56 bg-gray-800 text-white p-4 space-y-1">
        <h1 className="text-lg font-bold mb-4">Fairplan</h1>
        {menu.map((m) => (
          <NavLink
            key={m.to}
            to={m.to}
            className={({ isActive }) =>
              `block rounded px-3 py-2 ${isActive ? "bg-gray-600" : "hover:bg-gray-700"}`
            }
          >
            {m.label}
          </NavLink>
        ))}
      </nav>
      <main className="flex-1 p-6">
        <Outlet />
      </main>
    </div>
  );
}