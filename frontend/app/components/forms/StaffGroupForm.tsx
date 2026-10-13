import { useEffect, useState } from "react";

export type StaffGroup = { id: string; name: string };
export type StaffGroupRequest = { name: string };

type Props = {
  editing: StaffGroup | null;
  onSave: (data: StaffGroupRequest) => Promise<void>;
  onCancel: () => void;
};

export function StaffGroupForm({ editing, onSave, onCancel }: Props) {
  const [name, setName] = useState("");

  useEffect(() => {
    setName(editing ? editing.name : "");
  }, [editing]);

  async function submit(e: React.FormEvent) {
    e.preventDefault();
    await onSave({ name });
  }

  return (
    <form onSubmit={submit} className="flex flex-wrap items-end gap-3 bg-gray-50 p-4 rounded">
      <label className="flex flex-col">Navn
        <input className="border rounded px-2 py-1" value={name}
               onChange={(e) => setName(e.target.value)} />
      </label>
      <button className="bg-blue-600 text-white rounded px-4 py-1">
        {editing ? "Gem ændringer" : "Opret personalegruppe"}
      </button>
      {editing && <button type="button" className="px-4 py-1" onClick={onCancel}>Annuller</button>}
    </form>
  );
}