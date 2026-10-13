import { useEffect, useState } from "react";

export type User = { id: string; name: string; email: string; isAdmin: boolean; isPersonnel: boolean };
export type UserRequest = { name: string; email: string; isAdmin: boolean; isPersonnel: boolean };

type Props = {
  editing: User | null;
  onSave: (data: UserRequest) => Promise<void>;
  onCancel: () => void;
};

const empty: UserRequest = { name: "", email: "", isAdmin: false, isPersonnel: false };

export function UserForm({ editing, onSave, onCancel }: Props) {
  const [form, setForm] = useState<UserRequest>(empty);

  useEffect(() => {
    setForm(editing
      ? { name: editing.name, email: editing.email, isAdmin: editing.isAdmin, isPersonnel: editing.isPersonnel }
      : empty);
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
      <label className="flex flex-col">Email
        <input className="border rounded px-2 py-1" value={form.email}
               onChange={(e) => setForm({ ...form, email: e.target.value })} />
      </label>
      <label className="flex items-center gap-2 py-1">
        <input type="checkbox" checked={form.isAdmin}
               onChange={(e) => setForm({ ...form, isAdmin: e.target.checked })} />
        Admin
      </label>
      <label className="flex items-center gap-2 py-1">
        <input type="checkbox" checked={form.isPersonnel}
               onChange={(e) => setForm({ ...form, isPersonnel: e.target.checked })} />
        Personale
      </label>
      <button className="bg-blue-600 text-white rounded px-4 py-1">
        {editing ? "Gem ændringer" : "Opret bruger"}
      </button>
      {editing && <button type="button" className="px-4 py-1" onClick={onCancel}>Annuller</button>}
    </form>
  );
}