import { useMemo, useState } from "react";
import type { ReactNode } from "react";

export type Column<T> = {
  key: keyof T & string;              // which field to show / sort / search
  label: string;                      // header text (Danish)
  render?: (row: T) => ReactNode;     // optional custom cell, e.g. colour swatch
};

export type Filter<T> = {
  key: keyof T & string;              // which field to filter on
  label: string;                      // dropdown label
  options: { value: string; label: string }[];
};

type Props<T> = {
  rows: T[];
  columns: Column<T>[];
  filters?: Filter<T>[];
  actions?: (row: T) => ReactNode;    // edit/delete buttons per row
};

export function DataTable<T>({ rows, columns, filters = [], actions }: Props<T>) {
  const [search, setSearch] = useState("");
  const [sortKey, setSortKey] = useState<string | null>(null);
  const [sortAsc, setSortAsc] = useState(true);
  const [filterValues, setFilterValues] = useState<Record<string, string>>({});

  const visibleRows = useMemo(() => {
    let result = rows;

    // 1. Filters: keep rows whose field equals the chosen value
    for (const f of filters) {
      const chosen = filterValues[f.key];
      if (chosen) result = result.filter((r) => String(r[f.key]) === chosen);
    }

    // 2. Search: text appears in any column
    const term = search.trim().toLowerCase();
    if (term) {
      result = result.filter((r) =>
        columns.some((c) => String(r[c.key] ?? "").toLowerCase().includes(term))
      );
    }

    // 3. Sort: by clicked column
    if (sortKey) {
      result = [...result].sort((a, b) => {
        const av = String((a as any)[sortKey] ?? "");
        const bv = String((b as any)[sortKey] ?? "");
        const cmp = av.localeCompare(bv, "da", { numeric: true });
        return sortAsc ? cmp : -cmp;
      });
    }
    return result;
  }, [rows, columns, filters, filterValues, search, sortKey, sortAsc]);

  function clickHeader(key: string) {
    if (sortKey === key) setSortAsc(!sortAsc);
    else { setSortKey(key); setSortAsc(true); }
  }

  return (
    <div className="space-y-3">
      <div className="flex flex-wrap gap-3">
        <input
          className="border rounded px-3 py-1"
          placeholder="Søg…"
          value={search}
          onChange={(e) => setSearch(e.target.value)}
        />
        {filters.map((f) => (
          <select
            key={f.key}
            className="border rounded px-3 py-1"
            value={filterValues[f.key] ?? ""}
            onChange={(e) => setFilterValues({ ...filterValues, [f.key]: e.target.value })}
          >
            <option value="">{f.label}: alle</option>
            {f.options.map((o) => (
              <option key={o.value} value={o.value}>{o.label}</option>
            ))}
          </select>
        ))}
      </div>

      <table className="w-full border-collapse">
        <thead>
          <tr className="bg-gray-100 text-left">
            {columns.map((c) => (
              <th key={c.key} className="p-2 cursor-pointer select-none" onClick={() => clickHeader(c.key)}>
                {c.label} {sortKey === c.key ? (sortAsc ? "▲" : "▼") : ""}
              </th>
            ))}
            {actions && <th className="p-2" />}
          </tr>
        </thead>
        <tbody>
          {visibleRows.map((row, i) => (
            <tr key={i} className="border-t">
              {columns.map((c) => (
                <td key={c.key} className="p-2">
                  {c.render ? c.render(row) : String(row[c.key] ?? "")}
                </td>
              ))}
              {actions && <td className="p-2 space-x-2 text-right">{actions(row)}</td>}
            </tr>
          ))}
          {visibleRows.length === 0 && (
            <tr><td className="p-2 text-gray-500" colSpan={columns.length + 1}>Ingen rækker.</td></tr>
          )}
        </tbody>
      </table>
    </div>
  );
}