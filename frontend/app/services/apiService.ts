// One place for all calls to the backend.
// On error the backend sends ProblemDetails { title, status, detail } – we throw `detail` (Danish text).

async function request<T>(method: string, url: string, body?: unknown): Promise<T> {
  const response = await fetch(url, {
    method,
    headers: body ? { "Content-Type": "application/json" } : undefined,
    body: body ? JSON.stringify(body) : undefined,
  });

  if (!response.ok) {
    let message = "Der opstod en fejl.";
    try {
      const problem = await response.json();
      if (problem?.detail) message = problem.detail;
    } catch {
      // response had no JSON body – keep the default message
    }
    throw new Error(message);
  }

  // DELETE returns 204 No Content → nothing to parse
  if (response.status === 204) return undefined as T;
  return (await response.json()) as T;
}

export const api = {
  get: <T>(url: string) => request<T>("GET", url),
  post: <T>(url: string, body: unknown) => request<T>("POST", url, body),
  put: <T>(url: string, body: unknown) => request<T>("PUT", url, body),
  delete: (url: string) => request<void>("DELETE", url),
};