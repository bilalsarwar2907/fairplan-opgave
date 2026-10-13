import { useEffect, useState } from "react";

export type Section = { id: string; name: string; color: string };
export type SectionRequest = { name: string; color: string };

type Props = {
  editing: Section | null;
  onSave: (data: SectionRequest) => Promise<void>;
  onCancel: () => void;
};

const empty: SectionRequest = { name: "", color: "#3B82F6" };

export function SectionForm({ editing, onSave, onCancel }: Props) {
  const [form, setForm] = useState<SectionRequest>(empty);

  useEffect(() => {
    setForm(editing ? { name: editing.name, color: editing.color } : empty);
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
      <label className="flex flex-col">Farve
        {/* colour picker always gives #rrggbb, so it matches rule A1 */}
        <input type="color" className="h-9 w-16 border rounded" value={form.color}
               onChange={(e) => setForm({ ...form, color: e.target.value })} />
      </label>
      <button className="bg-blue-600 text-white rounded px-4 py-1">
        {editing ? "Gem ændringer" : "Opret afsnit"}
      </button>
      {editing && <button type="button" className="px-4 py-1" onClick={onCancel}>Annuller</button>}
    </form>
  );
}