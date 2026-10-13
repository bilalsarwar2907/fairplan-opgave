import { useEffect, useState } from "react";
import { api } from "~/services/apiService";
import { DataTable } from "~/components/DataTable";
import type { Period } from "~/components/forms/PeriodForm";
import type { Employment } from "~/components/forms/EmploymentForm";

const formatDate = (iso: string) => iso.split("-").reverse().join("-");

export default function Personale() {
  const [periods, setPeriods] = useState<Period[]>([]);
  const [periodId, setPeriodId] = useState("");
  const [rows, setRows] = useState<Employment[]>([]);
  const [error, setError] = useState("");

  // 1. Load periods once and pick the first one automatically
  useEffect(() => {
    (async () => {
      try {
        const list = await api.get<Period[]>("/api/periods");
        setPeriods(list);
        if (list.length > 0) setPeriodId(list[0].id);
      } catch (e) {
        setError((e as Error).message);
      }
    })();
  }, []);

  // 2. Every time the chosen period changes → ask the backend who works in it (S1)
  useEffect(() => {
    if (!periodId) return;
    (async () => {
      try {
        setError("");
        setRows(await api.get<Employment[]>(`/api/personnel?periodId=${periodId}`));
      } catch (e) {
        setError((e as Error).message);
      }
    })();
  }, [periodId]);

  // filter options built from the rows that are shown
  const unique = (values: string[]) =>
    [...new Set(values)].map((v) => ({ value: v, label: v }));

  return (
    <div className="space-y-4">
      <h2 className="text-2xl font-bold">Personale</h2>

      {error && <div className="bg-red-100 text-red-800 p-3 rounded">{error}</div>}

      <label className="flex items-center gap-3">
        <span className="font-semibold">Periode</span>
        <select className="border rounded px-3 py-1" value={periodId}
                onChange={(e) => setPeriodId(e.target.value)}>
          {periods.map((p) => (
            <option key={p.id} value={p.id}>
              {p.name} ({formatDate(p.startDate)} – {formatDate(p.endDate)})
            </option>
          ))}
        </select>
      </label>

      <DataTable
        rows={rows}
        columns={[
          { key: "userName", label: "Navn" },
          { key: "sectionName", label: "Afsnit" },
          { key: "staffGroupName", label: "Personalegruppe" },
          { key: "hoursPerWeek", label: "Timer/uge" },
          { key: "startDate", label: "Ansat fra", render: (e) => formatDate(e.startDate) },
          { key: "endDate", label: "Ansat til", render: (e) => formatDate(e.endDate) },
        ]}
        filters={[
          { key: "sectionName", label: "Afsnit", options: unique(rows.map((r) => r.sectionName)) },
          { key: "staffGroupName", label: "Personalegruppe", options: unique(rows.map((r) => r.staffGroupName)) },
        ]}
      />
    </div>
  );
}