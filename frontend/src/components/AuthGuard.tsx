"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { getRole, isLoggedIn, logout } from "@/services/authService";
import { isTokenExpired } from "@/utils/tokenUtils";

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
      const token = localStorage.getItem("hotelLakeviewToken");

      // Tarkista token vanheneminen
      if (token && isTokenExpired(token)) {
        console.warn("Token expired, logging out");
        logout();
        setStatus("redirecting");
        router.replace("/login");
        return;
      }

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
      ? "Ei oikeuksia tälle sivulle"
      : "Tarkistetaan oikeuksia...";

  return (
    <div className="flex items-center justify-center min-h-screen">
      <div className="text-center">
        <h1 className="text-2xl font-bold mb-4">{message}</h1>
        {status === "redirecting" && (
          <p className="text-gray-600">Ohjataan kirjautumissivulle...</p>
        )}
      </div>
    </div>
  );
}