"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { login, saveAuth } from "@/services/authService";

export default function LoginPage() {
  const router = useRouter();

  const [username, setUsername] = useState("Admin");
  const [password, setPassword] = useState("Admin1");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();

    try {
      setLoading(true);
      setError("");

      const result = await login({
        username,
        password,
      });

      saveAuth(result);
      router.replace("/");
    } catch (err) {
      setError(
        err instanceof Error ? err.message : "Kirjautuminen epäonnistui."
      );
    } finally {
      setLoading(false);
    }
  }

  return (
    <main className="flex min-h-screen items-center justify-center bg-gradient-to-br from-slate-950 via-slate-900 to-[#6b4f2a] px-6 py-8">
      <section className="w-full max-w-md rounded-3xl bg-white p-8 shadow-2xl">
        <p className="text-sm font-semibold uppercase tracking-wide text-[#8a6a3f]">
          Hotel Lakeview
        </p>

        <h1 className="mt-3 text-3xl font-bold text-slate-900">
          Kirjaudu sisään
        </h1>

        <p className="mt-2 text-slate-600">
          Kirjaudu sisään käyttääksesi varausjärjestelmää.
        </p>

        <form onSubmit={handleSubmit} className="mt-6 space-y-4">
          <div>
            <label className="text-sm font-semibold text-slate-700">
              Käyttäjänimi
            </label>
            <input
              value={username}
              onChange={(event) => setUsername(event.target.value)}
              className="mt-1 w-full rounded-xl border border-slate-300 px-4 py-3 text-slate-900 outline-none focus:border-slate-900"
              required
            />
          </div>

          <div>
            <label className="text-sm font-semibold text-slate-700">
              Salasana
            </label>
            <input
              value={password}
              onChange={(event) => setPassword(event.target.value)}
              type="password"
              className="mt-1 w-full rounded-xl border border-slate-300 px-4 py-3 text-slate-900 outline-none focus:border-slate-900"
              required
            />
          </div>

          {error && (
            <div className="rounded-xl bg-red-50 p-3 text-sm text-red-700">
              {error}
            </div>
          )}

          <button
            type="submit"
            disabled={loading}
            className="w-full rounded-xl bg-slate-900 px-4 py-3 font-semibold text-white transition hover:bg-slate-700 disabled:cursor-not-allowed disabled:bg-slate-400"
          >
            {loading ? "Kirjaudutaan..." : "Kirjaudu"}
          </button>
        </form>
      </section>
    </main>
  );
}