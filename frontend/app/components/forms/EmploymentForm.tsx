import { useEffect, useState } from "react";

export type Employment = {
  id: string;
  userId: string; userName: string;
  sectionId: string; sectionName: string;
  staffGroupId: string; staffGroupName: string;
  hoursPerWeek: number; startDate: string; endDate: string;
};
export type EmploymentRequest = {
  userId: string; sectionId: string; staffGroupId: string;
  hoursPerWeek: number; startDate: string; endDate: string;
};
type Option = { id: string; name: string };

type Props = {
  editing: Employment | null;
  users: Option[];          // only personnel users
  sections: Option[];
  groups: Option[];
  onSave: (data: EmploymentRequest) => Promise<void>;
  onCancel: () => void;
};

const empty: EmploymentRequest = { userId: "", sectionId: "", staffGroupId: "", hoursPerWeek: 37, startDate: "", endDate: "" };

export function EmploymentForm({ editing, users, sections, groups, onSave, onCancel }: Props) {
  const [form, setForm] = useState<EmploymentRequest>(empty);

  useEffect(() => {
    setForm(editing
      ? { userId: editing.userId, sectionId: editing.sectionId, staffGroupId: editing.staffGroupId,
          hoursPerWeek: editing.hoursPerWeek, startDate: editing.startDate, endDate: editing.endDate }
      : empty);
  }, [editing]);

  async function submit(e: React.FormEvent) {
    e.preventDefault();
    await onSave(form);
  }

  // one small helper so the 3 dropdowns look the same
  const select = (label: string, value: string, options: Option[], onChange: (v: string) => void) => (
    <label className="flex flex-col">{label}
      <select className="border rounded px-2 py-1" value={value} onChange={(e) => onChange(e.target.value)}>
        <option value="">Vælg…</option>
        {options.map((o) => <option key={o.id} value={o.id}>{o.name}</option>)}
      </select>
    </label>
  );

  return (
    <form onSubmit={submit} className="flex flex-wrap items-end gap-3 bg-gray-50 p-4 rounded">
      {select("Bruger", form.userId, users, (v) => setForm({ ...form, userId: v }))}
      {select("Afsnit", form.sectionId, sections, (v) => setForm({ ...form, sectionId: v }))}
      {select("Personalegruppe", form.staffGroupId, groups, (v) => setForm({ ...form, staffGroupId: v }))}
      <label className="flex flex-col">Timer/uge
        <input type="number" className="border rounded px-2 py-1 w-24" value={form.hoursPerWeek}
               onChange={(e) => setForm({ ...form, hoursPerWeek: Number(e.target.value) })} />
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
        {editing ? "Gem ændringer" : "Opret ansættelse"}
      </button>
      {editing && <button type="button" className="px-4 py-1" onClick={onCancel}>Annuller</button>}
    </form>
  );
}