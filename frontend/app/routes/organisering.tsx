import { useEffect, useState } from "react";
import { api } from "~/services/apiService";
import { DataTable } from "~/components/DataTable";
import { SectionForm, type Section, type SectionRequest } from "~/components/forms/SectionForm";
import { StaffGroupForm, type StaffGroup, type StaffGroupRequest } from "~/components/forms/StaffGroupForm";

type Tab = "afsnit" | "grupper";

export default function Organisering() {
  const [tab, setTab] = useState<Tab>("afsnit");

  const tabClass = (t: Tab) =>
    `px-4 py-2 rounded-t ${tab === t ? "bg-gray-200 font-semibold" : "hover:bg-gray-100"}`;

  return (
    <div className="space-y-4">
      <h2 className="text-2xl font-bold">Organisering</h2>
      <div className="flex gap-1 border-b">
        <button className={tabClass("afsnit")} onClick={() => setTab("afsnit")}>Afsnit</button>
        <button className={tabClass("grupper")} onClick={() => setTab("grupper")}>Personalegrupper</button>
      </div>
      {tab === "afsnit" ? <AfsnitTab /> : <GrupperTab />}
    </div>
  );
}

function AfsnitTab() {
  const [sections, setSections] = useState<Section[]>([]);
  const [editing, setEditing] = useState<Section | null>(null);
  const [error, setError] = useState("");

  async function load() {
    try {
      setSections(await api.get<Section[]>("/api/sections"));
    } catch (e) {
      setError((e as Error).message);
    }
  }
  useEffect(() => { load(); }, []);

  async function save(data: SectionRequest) {
    try {
      setError("");
      if (editing) await api.put(`/api/sections/${editing.id}`, data);
      else await api.post("/api/sections", data);
      setEditing(null);
      await load();
    } catch (e) {
      setError((e as Error).message);
    }
  }

  async function remove(s: Section) {
    if (!window.confirm(`Slet afsnittet "${s.name}"?`)) return;
    try {
      setError("");
      await api.delete(`/api/sections/${s.id}`);
      await load();
    } catch (e) {
      setError((e as Error).message);
    }
  }

  return (
    <div className="space-y-4">
      {error && <div className="bg-red-100 text-red-800 p-3 rounded">{error}</div>}
      <SectionForm editing={editing} onSave={save} onCancel={() => setEditing(null)} />
      <DataTable
        rows={sections}
        columns={[
          { key: "name", label: "Navn" },
          {
            key: "color", label: "Farve",
            render: (s) => (
              <span className="inline-flex items-center gap-2">
                <span className="inline-block w-5 h-5 rounded border" style={{ backgroundColor: s.color }} />
                {s.color}
              </span>
            ),
          },
        ]}
        actions={(s) => (
          <>
            <button className="text-blue-600" onClick={() => setEditing(s)}>Rediger</button>
            <button className="text-red-600" onClick={() => remove(s)}>Slet</button>
          </>
        )}
      />
    </div>
  );
}

function GrupperTab() {
  const [groups, setGroups] = useState<StaffGroup[]>([]);
  const [editing, setEditing] = useState<StaffGroup | null>(null);
  const [error, setError] = useState("");

  async function load() {
    try {
      setGroups(await api.get<StaffGroup[]>("/api/staffgroups"));
    } catch (e) {
      setError((e as Error).message);
    }
  }
  useEffect(() => { load(); }, []);

  async function save(data: StaffGroupRequest) {
    try {
      setError("");
      if (editing) await api.put(`/api/staffgroups/${editing.id}`, data);
      else await api.post("/api/staffgroups", data);
      setEditing(null);
      await load();
    } catch (e) {
      setError((e as Error).message);
    }
  }

  async function remove(g: StaffGroup) {
    if (!window.confirm(`Slet personalegruppen "${g.name}"?`)) return;
    try {
      setError("");
      await api.delete(`/api/staffgroups/${g.id}`);
      await load();
    } catch (e) {
      setError((e as Error).message);
    }
  }

  return (
    <div className="space-y-4">
      {error && <div className="bg-red-100 text-red-800 p-3 rounded">{error}</div>}
      <StaffGroupForm editing={editing} onSave={save} onCancel={() => setEditing(null)} />
      <DataTable
        rows={groups}
        columns={[{ key: "name", label: "Navn" }]}
        actions={(g) => (
          <>
            <button className="text-blue-600" onClick={() => setEditing(g)}>Rediger</button>
            <button className="text-red-600" onClick={() => remove(g)}>Slet</button>
          </>
        )}
      />
    </div>
  );
}