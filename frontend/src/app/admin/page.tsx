"use client";
import AuthGuard from "@/components/AuthGuard";
import { useEffect, useState } from "react";
import BackToHomeLink from "@/components/BackToHomeLink";
import {
  getReservationSummary,
  getOccupancyReport,
  getRevenueReport,
  getPopularRoomTypesReport,
} from "@/services/analyticsService";
import { createUser, deactivateUser, getUsers } from "@/services/userService";

import type {
  ReservationSummaryReport,
  OccupancyReport,
  RevenueReport,
  PopularRoomTypesReport,
} from "@/types/analytics";
import type { User, UserRole } from "@/types/user";

function formatDate(date: Date): string {
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, "0");
  const day = String(date.getDate()).padStart(2, "0");

  return `${year}-${month}-${day}T00:00:00Z`;
}

function formatCurrency(value: number): string {
  return new Intl.NumberFormat("fi-FI", {
    style: "currency",
    currency: "EUR",
  }).format(value);
}

function getRoomTypeName(roomType: number): string {
  switch (roomType) {
    case 1:
      return "Economy";
    case 2:
      return "Standard";
    case 3:
      return "Superior";
    case 4:
      return "Junior Suite";
    case 5:
      return "Suite";
    default:
      return "Unknown";
  }
}

export default function AdminPage() {
  const [summary, setSummary] = useState<ReservationSummaryReport | null>(null);
  const [occupancy, setOccupancy] = useState<OccupancyReport | null>(null);
  const [revenue, setRevenue] = useState<RevenueReport | null>(null);
  const [popular, setPopular] = useState<PopularRoomTypesReport | null>(null);
  const [users, setUsers] = useState<User[]>([]);

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [isRevenueOpen, setIsRevenueOpen] = useState(false);
  const [isPopularOpen, setIsPopularOpen] = useState(false);
  const [isOccupancyOpen, setIsOccupancyOpen] = useState(false);
  const [isSummaryOpen, setIsSummaryOpen] = useState(false);
  const [isUsersOpen, setIsUsersOpen] = useState(false);

  const [newUsername, setNewUsername] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [newRole, setNewRole] = useState<UserRole>("Receptionist");

  const [userMessage, setUserMessage] = useState<string | null>(null);
  const [userError, setUserError] = useState<string | null>(null);
  const [isUserActionLoading, setIsUserActionLoading] = useState(false);

  async function loadUsers() {
    const usersResult = await getUsers(1, 20);
    setUsers(usersResult.items);
  }

  useEffect(() => {
    async function loadData() {
      try {
        setLoading(true);
        setError(null);

        const now = new Date();

        const start = new Date(now.getFullYear(), 2, 1);
        const end = new Date(now.getFullYear(), 5, 1);

        const startDate = formatDate(start);
        const endDate = formatDate(end);

        const [summaryRes, occupancyRes, revenueRes, popularRes, usersRes] =
          await Promise.all([
            getReservationSummary(startDate, endDate),
            getOccupancyReport(startDate, endDate),
            getRevenueReport(startDate, endDate),
            getPopularRoomTypesReport(startDate, endDate),
            getUsers(1, 20),
          ]);

        setSummary(summaryRes);
        setOccupancy(occupancyRes);
        setRevenue(revenueRes);
        setPopular(popularRes);
        setUsers(usersRes.items);
      } catch (err) {
        setError(
          err instanceof Error
            ? err.message
            : "Admin-datan lataaminen epäonnistui."
        );
      } finally {
        setLoading(false);
      }
    }

    loadData();
  }, []);

  async function handleCreateUser(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();

    if (newPassword.length < 6) {
      setUserError("Salasanan pitää olla vähintään 6 merkkiä.");
      setUserMessage(null);
      return;
    }

    try {
      setIsUserActionLoading(true);
      setUserError(null);
      setUserMessage(null);

      await createUser({
        username: newUsername.trim(),
        password: newPassword,
        role: newRole,
      });

      await loadUsers();

      setNewUsername("");
      setNewPassword("");
      setNewRole("Receptionist");
      setUserMessage("Käyttäjä lisättiin onnistuneesti.");
    } catch (err) {
      setUserError(
        err instanceof Error ? err.message : "Käyttäjän lisääminen epäonnistui."
      );
    } finally {
      setIsUserActionLoading(false);
    }
  }

  async function handleDeactivateUser(id: number) {
    try {
      setIsUserActionLoading(true);
      setUserError(null);
      setUserMessage(null);

      await deactivateUser(id);
      await loadUsers();

      setUserMessage("Käyttäjä poistettiin käytöstä.");
    } catch (err) {
      setUserError(
        err instanceof Error ? err.message : "Käyttäjän poisto epäonnistui."
      );
    } finally {
      setIsUserActionLoading(false);
    }
  }

  if (loading) {
    return (
      <main className="min-h-screen bg-slate-900 p-6 text-white">
        Ladataan admin-näkymää...
      </main>
    );
  }

  if (error) {
    return (
      <main className="min-h-screen bg-slate-900 p-6 text-white">
        <BackToHomeLink />
        <div className="rounded-xl border border-red-400/30 bg-red-500/10 p-4 text-red-100">
          {error}
        </div>
      </main>
    );
  }

  const mostPopularRoomType = popular?.roomTypes[0];

  return (
    <AuthGuard requireManagement>
    <main className="min-h-screen bg-slate-900 px-6 py-8 text-white">
      <div className="mx-auto max-w-6xl space-y-8">
        <BackToHomeLink />

        <header>
          <h1 className="text-3xl font-bold">Admin</h1>
          <p className="mt-2 text-slate-400">
            Hotellin raportointi ja hallintanäkymä.
          </p>
        </header>

        <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-5">
          <Card
            title="Aktiiviset varaukset"
            value={summary?.activeReservationCount ?? 0}
          />

          <Card
            title="Perutut varaukset"
            value={summary?.cancelledReservationCount ?? 0}
          />

          <Card
            title="Käyttöaste"
            value={`${occupancy?.occupancyRatePercentage ?? 0} %`}
          />

          <Card
            title="Revenue"
            value={formatCurrency(revenue?.totalRevenue ?? 0)}
          />

          <Card
            title="Suosituin huonetyyppi"
            value={
              mostPopularRoomType
                ? getRoomTypeName(mostPopularRoomType.roomType)
                : "-"
            }
          />
        </div>

        <ExpandableSection
          title="Varausten yhteenveto"
          isOpen={isSummaryOpen}
          onToggle={() => setIsSummaryOpen((value) => !value)}
        >
          <div className="grid gap-3 md:grid-cols-2">
            <InfoRow
              label="Aktiiviset varaukset"
              value={summary?.activeReservationCount ?? 0}
            />
            <InfoRow
              label="Perutut varaukset"
              value={summary?.cancelledReservationCount ?? 0}
            />
            <InfoRow
              label="Varaukset yhteensä"
              value={summary?.totalReservationCount ?? 0}
            />
            <InfoRow
              label="Peruutusaste"
              value={`${summary?.cancellationRatePercentage ?? 0} %`}
            />
            <InfoRow
              label="Peruttujen arvo"
              value={formatCurrency(summary?.cancelledRevenueValue ?? 0)}
            />
          </div>
        </ExpandableSection>

        <ExpandableSection
          title="Käyttöasteen tarkemmat tiedot"
          isOpen={isOccupancyOpen}
          onToggle={() => setIsOccupancyOpen((value) => !value)}
        >
          <div className="grid gap-3 md:grid-cols-2">
            <InfoRow
              label="Aktiivisia huoneita"
              value={occupancy?.activeRoomCount ?? 0}
            />
            <InfoRow
              label="Päiviä raportissa"
              value={occupancy?.totalDays ?? 0}
            />
            <InfoRow
              label="Saatavilla huoneyöt"
              value={occupancy?.totalAvailableRoomNights ?? 0}
            />
            <InfoRow
              label="Varatut huoneyöt"
              value={occupancy?.occupiedRoomNights ?? 0}
            />
            <InfoRow
              label="Käyttöaste"
              value={`${occupancy?.occupancyRatePercentage ?? 0} %`}
            />
          </div>
        </ExpandableSection>

        <ExpandableSection
          title="Kuukausittainen revenue"
          isOpen={isRevenueOpen}
          onToggle={() => setIsRevenueOpen((value) => !value)}
        >
          {!revenue?.months.length ? (
            <p className="text-slate-400">Ei revenue-dataa.</p>
          ) : (
            <div className="space-y-3">
              {revenue.months.map((month) => (
                <div
                  key={`${month.year}-${month.month}`}
                  className="flex justify-between rounded-lg bg-slate-800 p-3"
                >
                  <span>
                    {month.month}.{month.year}
                  </span>
                  <span>{formatCurrency(month.revenue)}</span>
                </div>
              ))}
            </div>
          )}
        </ExpandableSection>

        <ExpandableSection
          title="Suosituimmat huonetyypit"
          isOpen={isPopularOpen}
          onToggle={() => setIsPopularOpen((value) => !value)}
        >
          {!popular?.roomTypes.length ? (
            <p className="text-slate-400">Ei huonetyyppidataa.</p>
          ) : (
            <div className="space-y-3">
              {popular.roomTypes.map((room) => (
                <div
                  key={room.roomType}
                  className="flex justify-between rounded-lg bg-slate-800 p-3"
                >
                  <span>{getRoomTypeName(room.roomType)}</span>
                  <span>
                    {room.reservationCount} varausta (
                    {room.percentageOfReservations.toFixed(1)} %)
                  </span>
                </div>
              ))}
            </div>
          )}
        </ExpandableSection>

        <ExpandableSection
          title="Käyttäjien hallinta"
          isOpen={isUsersOpen}
          onToggle={() => setIsUsersOpen((value) => !value)}
        >
          <form
            onSubmit={handleCreateUser}
            className="mb-6 grid gap-3 md:grid-cols-4"
          >
            <input
              value={newUsername}
              onChange={(event) => setNewUsername(event.target.value)}
              placeholder="Käyttäjänimi"
              className="rounded-lg border border-white/10 bg-slate-800 px-4 py-3 text-white outline-none placeholder:text-slate-500 focus:border-cyan-400"
              required
            />

            <input
              value={newPassword}
              onChange={(event) => setNewPassword(event.target.value)}
              placeholder="Salasana, vähintään 6 merkkiä"
              type="password"
              minLength={6}
              className="rounded-lg border border-white/10 bg-slate-800 px-4 py-3 text-white outline-none placeholder:text-slate-500 focus:border-cyan-400"
              required
            />

            <select
              value={newRole}
              onChange={(event) => setNewRole(event.target.value as UserRole)}
              className="rounded-lg border border-white/10 bg-slate-800 px-4 py-3 text-white outline-none focus:border-cyan-400"
            >
              <option value="Receptionist">Receptionist</option>
              <option value="Management">Management</option>
            </select>

            <button
              type="submit"
              disabled={isUserActionLoading}
              className="rounded-lg bg-cyan-500 px-4 py-3 font-semibold text-slate-950 transition hover:bg-cyan-400 disabled:cursor-not-allowed disabled:bg-slate-600 disabled:text-slate-300"
            >
              {isUserActionLoading ? "Tallennetaan..." : "Lisää käyttäjä"}
            </button>
          </form>

          {userError && (
            <div className="mb-4 rounded-lg border border-red-400/30 bg-red-500/10 p-3 text-red-100">
              {userError}
            </div>
          )}

          {userMessage && (
            <div className="mb-4 rounded-lg border border-green-400/30 bg-green-500/10 p-3 text-green-100">
              {userMessage}
            </div>
          )}

          {users.length === 0 ? (
            <p className="text-slate-400">Ei käyttäjiä.</p>
          ) : (
            <div className="space-y-3">
              {users.map((user) => (
                <div
                  key={user.id}
                  className="flex flex-col gap-3 rounded-lg bg-slate-800 p-4 md:flex-row md:items-center md:justify-between"
                >
                  <div>
                    <p className="font-semibold">{user.username}</p>
                    <p className="text-sm text-slate-400">
                      Rooli: {user.role} ·{" "}
                      {user.isActive ? "Aktiivinen" : "Passiivinen"}
                    </p>
                  </div>

                  <button
                    type="button"
                    onClick={() => handleDeactivateUser(user.id)}
                    disabled={!user.isActive || isUserActionLoading}
                    className="rounded-lg bg-red-500 px-4 py-2 font-semibold text-white transition hover:bg-red-400 disabled:cursor-not-allowed disabled:bg-slate-600"
                  >
                    Poista käytöstä
                  </button>
                </div>
              ))}
            </div>
          )}
        </ExpandableSection>
      </div>
    </main>
    </AuthGuard>
  );
}

function Card({ title, value }: { title: string; value: string | number }) {
  return (
    <div className="rounded-xl border border-white/10 bg-white/10 p-4">
      <p className="text-sm text-slate-300">{title}</p>
      <p className="mt-2 text-2xl font-bold">{value}</p>
    </div>
  );
}

function ExpandableSection({
  title,
  isOpen,
  onToggle,
  children,
}: {
  title: string;
  isOpen: boolean;
  onToggle: () => void;
  children: React.ReactNode;
}) {
  return (
    <section className="rounded-xl border border-white/10 bg-white/5">
      <button
        type="button"
        onClick={onToggle}
        className="flex w-full items-center justify-between p-5 text-left"
      >
        <h2 className="text-xl font-semibold">{title}</h2>
        <span className="text-slate-300">{isOpen ? "Sulje" : "Avaa"}</span>
      </button>

      {isOpen && <div className="border-t border-white/10 p-5">{children}</div>}
    </section>
  );
}

function InfoRow({
  label,
  value,
}: {
  label: string;
  value: string | number;
}) {
  return (
    <div className="rounded-lg bg-slate-800 p-3">
      <p className="text-sm text-slate-400">{label}</p>
      <p className="mt-1 font-semibold">{value}</p>
    </div>
  );
}