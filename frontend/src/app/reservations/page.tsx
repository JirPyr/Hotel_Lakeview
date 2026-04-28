"use client";

import BackToHomeLink from "@/components/BackToHomeLink";
import {
  cancelReservation,
  getReservations,
  updateReservation,
} from "@/services/reservationService";
import { getRooms } from "@/services/roomService";
import { getCustomers } from "@/services/customerService";
import { Reservation } from "@/types/reservation";
import { Room } from "@/types/room";
import { Customer } from "@/types/customer";
import { useEffect, useState } from "react";
import AuthGuard from "@/components/AuthGuard";

const PAGE_SIZE = 10;
const LOOKUP_PAGE_SIZE = 100;

function getReservationStatusName(status: number | string): string {
  if (status === 1 || status === "Pending") return "Odottaa";
  if (status === 2 || status === "Confirmed") return "Vahvistettu";
  if (status === 3 || status === "Cancelled") return "Peruttu";
  return "Tuntematon";
}

function isCancelled(status: number | string): boolean {
  return status === 3 || status === "Cancelled";
}

function formatDate(date: string): string {
  return new Date(date).toLocaleDateString("fi-FI");
}

function formatPrice(price: number): string {
  return new Intl.NumberFormat("fi-FI", {
    style: "currency",
    currency: "EUR",
  }).format(price);
}

export default function ReservationsPage() {
  const [reservations, setReservations] = useState<Reservation[]>([]);
  const [customersById, setCustomersById] = useState<Record<number, Customer>>({});
  const [roomsById, setRoomsById] = useState<Record<number, Room>>({});

  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [totalCount, setTotalCount] = useState(0);

  const [searchTerm, setSearchTerm] = useState("");
  const [submittedSearchTerm, setSubmittedSearchTerm] = useState("");

  const [loading, setLoading] = useState(true);
  const [cancellingId, setCancellingId] = useState<number | null>(null);
  const [error, setError] = useState("");
  const [successToast, setSuccessToast] = useState("");

  const [editingReservation, setEditingReservation] =
  useState<Reservation | null>(null);

const [editCheckInDate, setEditCheckInDate] = useState("");
const [editCheckOutDate, setEditCheckOutDate] = useState("");
const [editGuestCount, setEditGuestCount] = useState(1);
const [editNotes, setEditNotes] = useState("");
const [savingEdit, setSavingEdit] = useState(false);
const [editRoomId, setEditRoomId] = useState<number | null>(null);
const [editError, setEditError] = useState("");
const [editSuccess, setEditSuccess] = useState("");
  useEffect(() => {
    let ignore = false;

    async function fetchData() {
      try {
        setError("");

        const [reservationResult, customerResult, roomResult] =
          await Promise.all([
            getReservations(page, PAGE_SIZE),
            getCustomers(1, LOOKUP_PAGE_SIZE),
            getRooms(1, LOOKUP_PAGE_SIZE),
          ]);

        if (ignore) return;

        setReservations(reservationResult.items);
        setTotalPages(reservationResult.totalPages);
        setTotalCount(reservationResult.totalCount);

        setCustomersById(
          Object.fromEntries(
            customerResult.items.map((customer) => [customer.id, customer])
          )
        );

        setRoomsById(
          Object.fromEntries(roomResult.items.map((room) => [room.id, room]))
        );
      } catch (error) {
        if (ignore) return;

        console.error("Failed to fetch reservations page data", error);

        if (error instanceof Error) {
          setError(error.message);
        } else {
          setError("Varausten lataaminen epäonnistui.");
        }
      } finally {
        if (!ignore) {
          setLoading(false);
        }
      }
    }

    fetchData();

    return () => {
      ignore = true;
    };
  }, [page, submittedSearchTerm]);

  async function refreshReservations() {
    const result = await getReservations(page, PAGE_SIZE);

    setReservations(result.items);
    setTotalPages(result.totalPages);
    setTotalCount(result.totalCount);
  }

  async function handleCancelReservation(reservationId: number) {
    const confirmed = window.confirm(
      "Haluatko varmasti perua tämän varauksen?"
    );

    if (!confirmed) return;

    setCancellingId(reservationId);
    setError("");
    setSuccessToast("");

    try {
      await cancelReservation(reservationId);
      await refreshReservations();

      setSuccessToast(`Varaus #${reservationId} peruttiin onnistuneesti.`);

      setTimeout(() => {
        setSuccessToast("");
      }, 4000);
    } catch (error) {
      console.error("Failed to cancel reservation", error);

      if (error instanceof Error) {
        setError(error.message);
      } else {
        setError("Varauksen peruminen epäonnistui.");
      }
    } finally {
      setCancellingId(null);
    }
  }

  function handleSearch(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setLoading(true);
    setPage(1);
    setSubmittedSearchTerm(searchTerm);
  }

  function clearSearch() {
    setSearchTerm("");
    setSubmittedSearchTerm("");
    setPage(1);
    setLoading(true);
  }

  function goToPreviousPage() {
    setLoading(true);
    setPage((currentPage) => Math.max(1, currentPage - 1));
  }

  function goToNextPage() {
    setLoading(true);
    setPage((currentPage) => Math.min(totalPages, currentPage + 1));
  }
  const normalizedSearchTerm = submittedSearchTerm.trim().toLowerCase();

const filteredReservations = reservations.filter((reservation) => {
  if (!normalizedSearchTerm) {
    return true;
  }

  const customer = customersById[reservation.customerId];
  const room = roomsById[reservation.roomId];

  return (
    reservation.id.toString().includes(normalizedSearchTerm) ||
    reservation.customerId.toString().includes(normalizedSearchTerm) ||
    reservation.roomId.toString().includes(normalizedSearchTerm) ||
    customer?.fullName?.toLowerCase().includes(normalizedSearchTerm) ||
    customer?.email?.toLowerCase().includes(normalizedSearchTerm) ||
    customer?.phoneNumber?.toLowerCase().includes(normalizedSearchTerm) ||
    room?.roomNumber?.toLowerCase().includes(normalizedSearchTerm)
  );
});

function toDateInputValue(date: string): string {
  return date.split("T")[0];
}

function openEditModal(reservation: Reservation) {
  setEditingReservation(reservation);
  setEditRoomId(reservation.roomId);
  setEditCheckInDate(toDateInputValue(reservation.checkInDate));
  setEditCheckOutDate(toDateInputValue(reservation.checkOutDate));
  setEditGuestCount(reservation.guestCount);
  setEditNotes(reservation.notes ?? "");
  setEditError("");
  setEditSuccess("");
}
function closeEditModal() {
  setEditingReservation(null);
  setEditRoomId(null);
  setEditCheckInDate("");
  setEditCheckOutDate("");
  setEditGuestCount(1);
  setEditNotes("");
  setEditError("");
  setEditSuccess("");
}

async function handleUpdateReservation() {
  if (!editingReservation || !editRoomId) {
    return;
  }

  if (!editCheckInDate || !editCheckOutDate) {
    setEditError("Check-in ja check-out ovat pakollisia.");
    return;
  }

  if (editCheckOutDate <= editCheckInDate) {
    setEditError("Check-out päivän täytyy olla check-in päivän jälkeen.");
    return;
  }

  if (editGuestCount < 1) {
    setEditError("Henkilömäärän täytyy olla vähintään 1.");
    return;
  }

  setSavingEdit(true);
  setEditError("");
  setEditSuccess("");

  try {
    await updateReservation(editingReservation.id, {
      id: editingReservation.id,
      roomId: editRoomId,
      checkInDate: `${editCheckInDate}T00:00:00.000Z`,
      checkOutDate: `${editCheckOutDate}T00:00:00.000Z`,
      guestCount: editGuestCount,
      notes: editNotes.trim() || null,
    });

    await refreshReservations();

    setEditSuccess(`Varaus #${editingReservation.id} päivitettiin onnistuneesti.`);

    setSuccessToast(
      `Varaus #${editingReservation.id} päivitettiin onnistuneesti.`
    );

    setTimeout(() => {
      setSuccessToast("");
    }, 4000);

    setTimeout(() => {
      closeEditModal();
    }, 700);
  } catch (error) {
    console.error("Failed to update reservation", error);

    if (error instanceof Error) {
      setEditError(error.message);
    } else {
      setEditError("Varauksen muokkaaminen epäonnistui.");
    }
  } finally {
    setSavingEdit(false);
  }
}


  return (
    <AuthGuard>
    <main className="min-h-screen bg-linear-to-b from-[#edf1f4] via-[#e8edf0] to-[#d8d0c2] p-6">
      <div className="mx-auto max-w-7xl">
        <BackToHomeLink />

        {successToast && (
          <div className="fixed right-6 top-6 z-50 rounded-2xl border border-emerald-200 bg-emerald-50 px-5 py-4 text-emerald-800 shadow-lg">
            <p className="font-semibold">Toiminto onnistui</p>
            <p className="mt-1 text-sm">{successToast}</p>
          </div>
        )}

        <section className="mb-6 rounded-3xl bg-linear-to-br from-[#162033] via-[#243044] to-[#6b4f2a] p-8 text-white shadow-xl">
          <p className="mb-3 text-sm font-semibold uppercase tracking-wide text-[#e2c891]">
            Hotel Lakeview
          </p>

          <h1 className="text-4xl font-bold md:text-5xl">
            Varausten hallinta
          </h1>

          <p className="mt-4 max-w-2xl text-slate-200">
            Hae varauksia asiakkaan nimen, puhelinnumeron, sähköpostin tai muun
            hakutiedon perusteella.
          </p>

          <div className="mt-6 rounded-2xl bg-white/10 px-4 py-3 text-sm text-slate-100">
            Yhteensä {totalCount} varausta
          </div>
        </section>

        <form
          onSubmit={handleSearch}
          className="mb-8 rounded-2xl bg-white p-4 shadow-sm"
        >
          <label className="mb-2 block text-sm font-semibold text-slate-700">
            Hae varauksia
          </label>

          <div className="flex flex-col gap-3 md:flex-row">
            <input
              type="text"
              value={searchTerm}
              onChange={(event) => setSearchTerm(event.target.value)}
              placeholder="Hae nimellä, puhelinnumerolla, sähköpostilla, ID:llä..."
              className="w-full rounded-xl border border-slate-300 p-3 text-slate-900 outline-none focus:border-[#6b4f2a]"
            />

            <button
              type="submit"
              className="rounded-xl bg-[#162033] px-5 py-3 font-semibold text-white hover:bg-[#243044]"
            >
              Hae
            </button>

            <button
              type="button"
              onClick={clearSearch}
              className="rounded-xl bg-slate-200 px-5 py-3 font-semibold text-slate-800 hover:bg-slate-300"
            >
              Tyhjennä
            </button>
          </div>

          {submittedSearchTerm && (
            <p className="mt-3 text-sm text-slate-600">
              Hakutulos sanalla:{" "}
              <span className="font-semibold text-slate-900">
                {submittedSearchTerm}
              </span>
            </p>
          )}
        </form>

        {error && (
          <div className="mb-6 rounded-2xl border border-red-300 bg-red-50 p-4 shadow-sm">
            <p className="font-semibold text-red-800">
              Varausten hallinta epäonnistui
            </p>
            <p className="mt-1 text-sm text-red-700">{error}</p>
          </div>
        )}

        {loading ? (
          <div className="rounded-2xl bg-white p-8 text-slate-600 shadow-sm">
            Ladataan varauksia...
          </div>
        ) : filteredReservations.length === 0 ? (
          <div className="rounded-2xl bg-white p-8 text-slate-600 shadow-sm">
            Varauksia ei löytynyt.
          </div>
        ) : (
          <>
            <section className="grid gap-4">
              {filteredReservations.map((reservation) => {
                const cancelled = isCancelled(reservation.status);
                const customer = customersById[reservation.customerId];
                const room = roomsById[reservation.roomId];

                return (
                  <article
                    key={reservation.id}
                    className="rounded-2xl border border-[#8a6a3f]/20 bg-white p-6 shadow-md"
                  >
                    <div className="flex flex-col gap-5 lg:flex-row lg:items-start lg:justify-between">
                      <div className="flex-1">
                        <div className="flex flex-wrap items-center gap-3">
                          <h2 className="text-xl font-bold text-[#162033]">
                            Varaus #{reservation.id}
                          </h2>

                          <span
                            className={`rounded-full px-3 py-1 text-xs font-bold ${
                              cancelled
                                ? "bg-red-100 text-red-700"
                                : "bg-emerald-100 text-emerald-700"
                            }`}
                          >
                            {getReservationStatusName(reservation.status)}
                          </span>
                        </div>

                        <div className="mt-4 grid gap-4 text-sm text-slate-600 md:grid-cols-2 lg:grid-cols-4">
                          <div className="rounded-xl bg-slate-50 p-3">
                            <p className="font-semibold text-slate-900">
                              Asiakas
                            </p>
                            <p>
                              {customer?.fullName ??
                                `Asiakas ID: ${reservation.customerId}`}
                            </p>
                            <p className="text-xs text-slate-500">
                              {customer?.phoneNumber ?? "Ei puhelinnumeroa"}
                            </p>
                            <p className="text-xs text-slate-500">
                              {customer?.email ?? "Ei sähköpostia"}
                            </p>
                            <p className="mt-1 text-xs text-slate-400">
                              Customer ID: {reservation.customerId}
                            </p>
                          </div>

                          <div className="rounded-xl bg-slate-50 p-3">
                            <p className="font-semibold text-slate-900">
                              Huone
                            </p>
                            <p>
                              {room?.roomNumber
                                ? `Huone ${room.roomNumber}`
                                : `Huone ID: ${reservation.roomId}`}
                            </p>
                            <p className="text-xs text-slate-500">
                              Room ID: {reservation.roomId}
                            </p>
                          </div>

                          <div className="rounded-xl bg-slate-50 p-3">
                            <p className="font-semibold text-slate-900">
                              Ajankohta
                            </p>
                            <p>Check-in: {formatDate(reservation.checkInDate)}</p>
                            <p>
                              Check-out: {formatDate(reservation.checkOutDate)}
                            </p>
                            <p className="text-xs text-slate-500">
                              Henkilöt: {reservation.guestCount}
                            </p>
                          </div>

                          <div className="rounded-xl bg-slate-50 p-3">
                            <p className="font-semibold text-slate-900">
                              Hinta
                            </p>
                            <p className="text-lg font-bold text-slate-900">
                              {formatPrice(reservation.totalPrice)}
                            </p>
                          </div>
                        </div>

                        {reservation.notes && (
                          <p className="mt-4 rounded-xl bg-slate-50 p-3 text-sm text-slate-600">
                            {reservation.notes}
                          </p>
                        )}
                      </div>

                      <div className="flex flex-col gap-2 sm:flex-row lg:flex-col">
                        <button
                            type="button"
                            onClick={() => openEditModal(reservation)}
                            disabled={cancelled}
                            className="rounded-xl bg-[#162033] px-5 py-3 font-semibold text-white hover:bg-[#243044] disabled:cursor-not-allowed disabled:bg-slate-300"
                            >
                            Muokkaa varausta
                            </button>

                        <button
                          type="button"
                          onClick={() =>
                            handleCancelReservation(reservation.id)
                          }
                          disabled={
                            cancelled || cancellingId === reservation.id
                          }
                          className="rounded-xl bg-red-600 px-5 py-3 font-semibold text-white hover:bg-red-700 disabled:cursor-not-allowed disabled:bg-slate-300"
                        >
                          {cancelled
                            ? "Peruttu"
                            : cancellingId === reservation.id
                              ? "Perutaan..."
                              : "Peru varaus"}
                        </button>
                      </div>
                    </div>
                  </article>
                );
              })}
            </section>

            <div className="mt-8 flex items-center justify-between rounded-2xl bg-white p-4 shadow-sm">
              <button
                onClick={goToPreviousPage}
                disabled={page <= 1}
                className="rounded-lg bg-slate-900 px-4 py-2 font-semibold text-white disabled:cursor-not-allowed disabled:bg-slate-300"
              >
                Edellinen
              </button>

              <p className="text-sm text-slate-600">
                Sivu {page} / {totalPages}
              </p>

              <button
                onClick={goToNextPage}
                disabled={page >= totalPages}
                className="rounded-lg bg-slate-900 px-4 py-2 font-semibold text-white disabled:cursor-not-allowed disabled:bg-slate-300"
              >
                Seuraava
              </button>
            </div>
          </>
        )}
      </div>
      {editingReservation && (
  <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4">
    <div className="w-full max-w-xl rounded-3xl bg-white p-6 shadow-2xl">
      <div className="mb-5 flex items-start justify-between gap-4">
        <div>
          <p className="text-sm font-semibold uppercase tracking-wide text-[#6b4f2a]">
            Hotel Lakeview
          </p>
          <h2 className="mt-1 text-2xl font-bold text-[#162033]">
            Muokkaa varausta #{editingReservation.id}
          </h2>
          <p className="mt-2 text-sm text-slate-600">
            Päivitä päivämäärät, henkilömäärä tai lisätiedot.
          </p>
        </div>

        <button
          type="button"
          onClick={closeEditModal}
          className="rounded-full bg-slate-100 px-3 py-1 text-sm font-bold text-slate-700 hover:bg-slate-200"
        >
          ✕
        </button>
      </div>

            <div className="mb-4 rounded-2xl bg-slate-50 p-4 text-sm text-slate-600">
            {editError && (
        <div className="mb-4 rounded-2xl border border-red-300 bg-red-50 p-4 text-sm text-red-800">
            <p className="font-semibold">Muokkaus epäonnistui</p>
            <p className="mt-1">{editError}</p>
        </div>
        )}

        {editSuccess && (
        <div className="mb-4 rounded-2xl border border-emerald-200 bg-emerald-50 p-4 text-sm text-emerald-800">
            <p className="font-semibold">Muokkaus onnistui</p>
            <p className="mt-1">{editSuccess}</p>
        </div>
        )} <p>
          Varaus ID:{" "}
          <span className="font-semibold text-slate-900">
            {editingReservation.id}
          </span>
        </p>
        <p>
          Huone ID:{" "}
          <span className="font-semibold text-slate-900">
            {editingReservation.roomId}
          </span>
        </p>
        <p>
          Asiakas ID:{" "}
          <span className="font-semibold text-slate-900">
            {editingReservation.customerId}
          </span>
        </p>
      </div>
            <div className="mb-4">
  <label className="mb-2 block text-sm font-medium text-slate-700">
    Huone
  </label>

  <select
    value={editRoomId ?? ""}
    onChange={(event) => setEditRoomId(Number(event.target.value))}
    className="w-full rounded-lg border border-slate-300 p-3 text-slate-900"
  >
    <option value="" disabled>
      Valitse huone
    </option>

    {Object.values(roomsById).map((room) => (
      <option key={room.id} value={room.id}>
        Huone {room.roomNumber} · {room.maxGuests} hlö ·{" "}
        {room.basePricePerNight} € / yö
      </option>
    ))}
  </select>

  <p className="mt-2 text-xs text-slate-500">
    Backend tarkistaa tallennuksessa, onko huone vapaa valitulle aikavälille.
  </p>
</div>
      <div className="grid gap-4 md:grid-cols-2">
        <div>
          <label className="mb-2 block text-sm font-medium text-slate-700">
            Check-in
          </label>
          <input
            type="date"
            value={editCheckInDate}
            onChange={(event) => setEditCheckInDate(event.target.value)}
            className="w-full rounded-lg border border-slate-300 p-3 text-slate-900"
          />
        </div>

        <div>
          <label className="mb-2 block text-sm font-medium text-slate-700">
            Check-out
          </label>
          <input
            type="date"
            value={editCheckOutDate}
            onChange={(event) => setEditCheckOutDate(event.target.value)}
            className="w-full rounded-lg border border-slate-300 p-3 text-slate-900"
          />
        </div>

        <div>
          <label className="mb-2 block text-sm font-medium text-slate-700">
            Henkilömäärä
          </label>
          <input
            type="number"
            min={1}
            value={editGuestCount}
            onChange={(event) => setEditGuestCount(Number(event.target.value))}
            className="w-full rounded-lg border border-slate-300 p-3 text-slate-900"
          />
        </div>
      </div>

      <label className="mb-2 mt-4 block text-sm font-medium text-slate-700">
        Lisätiedot
      </label>
      <textarea
        value={editNotes}
        onChange={(event) => setEditNotes(event.target.value)}
        className="min-h-28 w-full rounded-lg border border-slate-300 p-3 text-slate-900"
        placeholder="Esim. myöhäinen saapuminen"
      />

      <div className="mt-6 flex flex-col gap-3 sm:flex-row sm:justify-end">
        <button
          type="button"
          onClick={closeEditModal}
          className="rounded-xl bg-slate-200 px-5 py-3 font-semibold text-slate-800 hover:bg-slate-300"
        >
          Peruuta
        </button>

        <button
          type="button"
          onClick={handleUpdateReservation}
          disabled={savingEdit}
          className="rounded-xl bg-emerald-700 px-5 py-3 font-semibold text-white hover:bg-emerald-800 disabled:bg-slate-300"
        >
          {savingEdit ? "Tallennetaan..." : "Tallenna muutokset"}
        </button>
      </div>
    </div>
  </div>
)}
    </main>
        </AuthGuard>
  );
}