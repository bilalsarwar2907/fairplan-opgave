import { useEffect, useState } from "react";
import { api } from "~/services/apiService";
import { DataTable } from "~/components/DataTable";
import { PeriodForm, type Period, type PeriodRequest } from "~/components/forms/PeriodForm";

// "2026-12-01" → "01-12-2026"
const formatDate = (iso: string) => iso.split("-").reverse().join("-");

export default function Perioder() {
  const [periods, setPeriods] = useState<Period[]>([]);
  const [editing, setEditing] = useState<Period | null>(null);
  const [error, setError] = useState("");

  async function load() {
    setPeriods(await api.get<Period[]>("/api/periods"));
  }
  useEffect(() => { load(); }, []);

  async function save(data: PeriodRequest) {
    try {
      setError("");
      if (editing) await api.put(`/api/periods/${editing.id}`, data);
      else await api.post("/api/periods", data);
      setEditing(null);
      await load();
    } catch (e) {
      setError((e as Error).message);           // Danish message from the backend
    }
  }

  async function remove(p: Period) {
    if (!window.confirm(`Slet perioden "${p.name}"?`)) return;
    try {
      setError("");
      await api.delete(`/api/periods/${p.id}`);
      await load();
    } catch (e) {
      setError((e as Error).message);
    }
  }

  return (
    <div className="space-y-4">
      <h2 className="text-2xl font-bold">Perioder</h2>

      {error && <div className="bg-red-100 text-red-800 p-3 rounded">{error}</div>}

      <PeriodForm editing={editing} onSave={save} onCancel={() => setEditing(null)} />

      <DataTable
        rows={periods}
        columns={[
          { key: "name", label: "Navn" },
          { key: "startDate", label: "Startdato", render: (p) => formatDate(p.startDate) },
          { key: "endDate", label: "Slutdato", render: (p) => formatDate(p.endDate) },
        ]}
        actions={(p) => (
          <>
            <button className="text-blue-600" onClick={() => setEditing(p)}>Rediger</button>
            <button className="text-red-600" onClick={() => remove(p)}>Slet</button>
          </>
        )}
      />
    </div>
  );
}