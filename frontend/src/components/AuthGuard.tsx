"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { getRole, isLoggedIn } from "@/services/authService";

type AuthGuardProps = {
  children: React.ReactNode;
  requireManagement?: boolean;
};

type GuardStatus = "checking" | "allowed" | "redirecting" | "forbidden";

export default function AuthGuard({
  children,
  requireManagement = false,
}: AuthGuardProps) {
  const router = useRouter();

  const [status, setStatus] = useState<GuardStatus>("checking");

  useEffect(() => {
    const timeoutId = window.setTimeout(() => {
      if (!isLoggedIn()) {
        setStatus("redirecting");
        router.replace("/login");
        return;
      }

      if (requireManagement && getRole() !== "Management") {
        setStatus("forbidden");

        window.setTimeout(() => {
          router.replace("/");
        }, 1000);

        return;
      }

      setStatus("allowed");
    }, 0);

    return () => window.clearTimeout(timeoutId);
  }, [router, requireManagement]);

  if (status === "allowed") {
    return <>{children}</>;
  }

  const message =
    status === "forbidden"
      ? "Ei oikeuksia admin-näkymään."
      : status === "redirecting"
        ? "Ohjataan kirjautumissivulle..."
        : "Tarkistetaan kirjautumista...";

  return (
    <main className="flex min-h-screen items-center justify-center bg-slate-100 p-6">
      <div className="rounded-2xl bg-white p-6 text-center shadow-lg">
        <p className="font-semibold text-slate-700">{message}</p>
      </div>
    </main>
  );
}