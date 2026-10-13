import { useEffect, useState } from "react";

export type Period = { id: string; name: string; startDate: string; endDate: string };
export type PeriodRequest = { name: string; startDate: string; endDate: string };

type Props = {
  editing: Period | null;                       // null = create new, otherwise edit this one
  onSave: (data: PeriodRequest) => Promise<void>;
  onCancel: () => void;
};

const empty: PeriodRequest = { name: "", startDate: "", endDate: "" };

export function PeriodForm({ editing, onSave, onCancel }: Props) {
  const [form, setForm] = useState<PeriodRequest>(empty);

  // When "Rediger" is clicked, fill the form with that period
  useEffect(() => {
    setForm(editing ? { name: editing.name, startDate: editing.startDate, endDate: editing.endDate } : empty);
  }, [editing]);

  async function submit(e: React.FormEvent) {
    e.preventDefault();
    await onSave(form);
  }

  return (
    <form onSubmit={submit} className="flex flex-wrap items-end gap-3 bg-gray-50 p-4 rounded">
      <label className="flex flex-col">Navn
        <input className="border rounded px-2 py-1" value={form.name}
               onChange={(e) => setForm({ ...form, name: e.target.value })} />
      </label>
      <label className="flex flex-col">Startdato
        <input type="date" className="border rounded px-2 py-1" value={form.startDate}
               onChange={(e) => setForm({ ...form, startDate: e.target.value })} />
      </label>
      <label className="flex flex-col">Slutdato
        <input type="date" className="border rounded px-2 py-1" value={form.endDate}
               onChange={(e) => setForm({ ...form, endDate: e.target.value })} />
      </label>
      <button className="bg-blue-600 text-white rounded px-4 py-1">
        {editing ? "Gem ændringer" : "Opret periode"}
      </button>
      {editing && <button type="button" className="px-4 py-1" onClick={onCancel}>Annuller</button>}
    </form>
  );
}