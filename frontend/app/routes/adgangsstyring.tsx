import { useEffect, useState } from "react";
import { api } from "~/services/apiService";
import { DataTable } from "~/components/DataTable";
import { UserForm, type User, type UserRequest } from "~/components/forms/UserForm";
import { EmploymentForm, type Employment, type EmploymentRequest } from "~/components/forms/EmploymentForm";

type Tab = "adgange" | "ansaettelser";

type Option = { id: string; name: string };

const yesNo = (b: boolean) => (b ? "Ja" : "Nej");
const yesNoOptions = [{ value: "true", label: "Ja" }, { value: "false", label: "Nej" }];
const formatDate = (iso: string) => iso.split("-").reverse().join("-");

export default function Adgangsstyring() {
  const [tab, setTab] = useState<Tab>("adgange");

  const tabClass = (t: Tab) =>
    `px-4 py-2 rounded-t ${tab === t ? "bg-gray-200 font-semibold" : "hover:bg-gray-100"}`;

  return (
    <div className="space-y-4">
      <h2 className="text-2xl font-bold">Adgangsstyring</h2>
      <div className="flex gap-1 border-b">
        <button className={tabClass("adgange")} onClick={() => setTab("adgange")}>Adgange</button>
        <button className={tabClass("ansaettelser")} onClick={() => setTab("ansaettelser")}>Ansættelser</button>
      </div>
      {tab === "adgange" ? <AdgangeTab /> : <AnsaettelserTab />}
    </div>
  );
}

function AdgangeTab() {
  const [users, setUsers] = useState<User[]>([]);
  const [editing, setEditing] = useState<User | null>(null);
  const [error, setError] = useState("");

  async function load() {
    try {
      setUsers(await api.get<User[]>("/api/users"));
    } catch (e) {
      setError((e as Error).message);
    }
  }
  useEffect(() => { load(); }, []);

  async function save(data: UserRequest) {
    try {
      setError("");
      if (editing) await api.put(`/api/users/${editing.id}`, data);
      else await api.post("/api/users", data);
      setEditing(null);
      await load();
    } catch (e) {
      setError((e as Error).message);
    }
  }

  async function remove(u: User) {
    if (!window.confirm(`Slet brugeren "${u.name}"?`)) return;
    try {
      setError("");
      await api.delete(`/api/users/${u.id}`);
      await load();
    } catch (e) {
      setError((e as Error).message);
    }
  }

  return (
    <div className="space-y-4">
      {error && <div className="bg-red-100 text-red-800 p-3 rounded">{error}</div>}
      <UserForm editing={editing} onSave={save} onCancel={() => setEditing(null)} />
      <DataTable
        rows={users}
        columns={[
          { key: "name", label: "Navn" },
          { key: "email", label: "Email" },
          { key: "isAdmin", label: "Admin", render: (u) => yesNo(u.isAdmin) },
          { key: "isPersonnel", label: "Personale", render: (u) => yesNo(u.isPersonnel) },
        ]}
        filters={[
          { key: "isAdmin", label: "Admin", options: yesNoOptions },
          { key: "isPersonnel", label: "Personale", options: yesNoOptions },
        ]}
        actions={(u) => (
          <>
            <button className="text-blue-600" onClick={() => setEditing(u)}>Rediger</button>
            <button className="text-red-600" onClick={() => remove(u)}>Slet</button>
          </>
        )}
      />
    </div>
  );
}

function AnsaettelserTab() {
  const [employments, setEmployments] = useState<Employment[]>([]);
  const [users, setUsers] = useState<Option[]>([]);
  const [sections, setSections] = useState<Option[]>([]);
  const [groups, setGroups] = useState<Option[]>([]);
  const [editing, setEditing] = useState<Employment | null>(null);
  const [error, setError] = useState("");

  async function load() {
    try {
      const [emp, usr, sec, grp] = await Promise.all([
        api.get<Employment[]>("/api/employments"),
        api.get<User[]>("/api/users"),
        api.get<Option[]>("/api/sections"),
        api.get<Option[]>("/api/staffgroups"),
      ]);
      setEmployments(emp);
      setUsers(usr.filter((u) => u.isPersonnel));   // only personnel can be employed (E3)
      setSections(sec);
      setGroups(grp);
    } catch (e) {
      setError((e as Error).message);
    }
  }
  useEffect(() => { load(); }, []);

  async function save(data: EmploymentRequest) {
    try {
      setError("");
      if (editing) await api.put(`/api/employments/${editing.id}`, data);
      else await api.post("/api/employments", data);
      setEditing(null);
      await load();
    } catch (e) {
      setError((e as Error).message);
    }
  }

  async function remove(emp: Employment) {
    if (!window.confirm(`Slet ansættelsen for "${emp.userName}"?`)) return;
    try {
      setError("");
      await api.delete(`/api/employments/${emp.id}`);
      await load();
    } catch (e) {
      setError((e as Error).message);
    }
  }

  const toOptions = (list: Option[]) => list.map((o) => ({ value: o.name, label: o.name }));

  return (
    <div className="space-y-4">
      {error && <div className="bg-red-100 text-red-800 p-3 rounded">{error}</div>}
      <EmploymentForm editing={editing} users={users} sections={sections} groups={groups}
                      onSave={save} onCancel={() => setEditing(null)} />
      <DataTable
        rows={employments}
        columns={[
          { key: "userName", label: "Bruger" },
          { key: "sectionName", label: "Afsnit" },
          { key: "staffGroupName", label: "Personalegruppe" },
          { key: "hoursPerWeek", label: "Timer/uge" },
          { key: "startDate", label: "Startdato", render: (e) => formatDate(e.startDate) },
          { key: "endDate", label: "Slutdato", render: (e) => formatDate(e.endDate) },
        ]}
        filters={[
          { key: "sectionName", label: "Afsnit", options: toOptions(sections) },
          { key: "staffGroupName", label: "Personalegruppe", options: toOptions(groups) },
        ]}
        actions={(emp) => (
          <>
            <button className="text-blue-600" onClick={() => setEditing(emp)}>Rediger</button>
            <button className="text-red-600" onClick={() => remove(emp)}>Slet</button>
          </>
        )}
      />
    </div>
  );
}