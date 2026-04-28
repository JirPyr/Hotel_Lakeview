"use client";

import { useRouter } from "next/navigation";
import { logout } from "@/services/authService";

export default function LogoutButton() {
  const router = useRouter();

  function handleLogout() {
    logout();
    router.replace("/login");
  }

  return (
    <button
      onClick={handleLogout}
      className="rounded-full border border-red-400/30 bg-red-500/10 px-4 py-2 text-sm font-semibold text-red-300 transition hover:bg-red-500/20"
    >
      Kirjaudu ulos
    </button>
  );
}