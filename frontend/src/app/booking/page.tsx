"use client";

import BackToHomeLink from "@/components/BackToHomeLink";
import { useState } from "react";
import {
  createReservation,
  getAvailableRooms,
} from "@/services/reservationService";
import { createCustomer, searchCustomers } from "@/services/customerService";
import { AvailableRoom, Reservation } from "@/types/reservation";
import { Customer } from "@/types/customer";
import AuthGuard from "@/components/AuthGuard";

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
      return "Tuntematon";
  }
}

function getErrorMessage(error: unknown, fallbackMessage: string): string {
  if (error instanceof Error) {
    return error.message;
  }

  return fallbackMessage;
}

export default function BookingPage() {
  const [checkInDate, setCheckInDate] = useState("");
  const [checkOutDate, setCheckOutDate] = useState("");
  const [guestCount, setGuestCount] = useState(1);
  const [notes, setNotes] = useState("");

  const [customerMode, setCustomerMode] = useState<"existing" | "new">(
    "existing"
  );

  const [customerSearchTerm, setCustomerSearchTerm] = useState("");
  const [customerSearchResults, setCustomerSearchResults] = useState<Customer[]>(
    []
  );
  const [selectedCustomer, setSelectedCustomer] = useState<Customer | null>(
    null
  );
  const [searchingCustomers, setSearchingCustomers] = useState(false);

  const [newCustomerFullName, setNewCustomerFullName] = useState("");
  const [newCustomerEmail, setNewCustomerEmail] = useState("");
  const [newCustomerPhoneNumber, setNewCustomerPhoneNumber] = useState("");
  const [newCustomerNotes, setNewCustomerNotes] = useState("");
  const [creatingCustomer, setCreatingCustomer] = useState(false);

  const [availableRooms, setAvailableRooms] = useState<AvailableRoom[]>([]);
  const [selectedRoomId, setSelectedRoomId] = useState<number | null>(null);
  const [createdReservation, setCreatedReservation] =
    useState<Reservation | null>(null);

  const [loadingRooms, setLoadingRooms] = useState(false);
  const [creatingReservation, setCreatingReservation] = useState(false);
  const [error, setError] = useState("");
  const [successToast, setSuccessToast] = useState("");

  function showSuccessToast(message: string) {
    setSuccessToast(message);

    setTimeout(() => {
      setSuccessToast("");
    }, 4000);
  }

  function resetSelectedCustomer() {
    setSelectedCustomer(null);
    setCustomerSearchResults([]);
    setCustomerSearchTerm("");
  }

  async function handleSearchCustomers() {
    if (!customerSearchTerm.trim()) {
      setError("Kirjoita asiakkaan nimi, sähköposti tai puhelinnumero.");
      return;
    }

    setError("");
    setSearchingCustomers(true);
    setSelectedCustomer(null);

    try {
      const result = await searchCustomers(customerSearchTerm, 1, 10);

      setCustomerSearchResults(result.items);

      if (result.items.length === 0) {
        setError("Asiakasta ei löytynyt. Voit luoda uuden asiakkaan.");
      }
    } catch (error) {
      console.error("Failed to search customers", error);
      setError(getErrorMessage(error, "Asiakkaiden hakeminen epäonnistui."));
    } finally {
      setSearchingCustomers(false);
    }
  }

  async function handleCreateCustomer() {
    if (!newCustomerFullName.trim()) {
      setError("Uuden asiakkaan nimi on pakollinen.");
      return;
    }

    setCreatingCustomer(true);
    setError("");

    try {
      const customer = await createCustomer({
        fullName: newCustomerFullName.trim(),
        email: newCustomerEmail.trim() || null,
        phoneNumber: newCustomerPhoneNumber.trim() || null,
        notes: newCustomerNotes.trim() || null,
      });

      setSelectedCustomer(customer);
      setCustomerSearchResults([]);
      setCustomerSearchTerm(customer.fullName);
      setCustomerMode("existing");

      setNewCustomerFullName("");
      setNewCustomerEmail("");
      setNewCustomerPhoneNumber("");
      setNewCustomerNotes("");

      showSuccessToast(`Asiakas ${customer.fullName} luotiin onnistuneesti.`);
    } catch (error) {
      console.error("Failed to create customer", error);
      setError(getErrorMessage(error, "Asiakkaan luominen epäonnistui."));
    } finally {
      setCreatingCustomer(false);
    }
  }

  async function handleSearchRooms(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();

    setError("");
    setCreatedReservation(null);
    setSelectedRoomId(null);
    setLoadingRooms(true);

    try {
      const rooms = await getAvailableRooms(
        checkInDate,
        checkOutDate,
        guestCount
      );

      setAvailableRooms(rooms);

      if (rooms.length === 0) {
        setError(
          "Valitulle aikavälille ja henkilömäärälle ei löytynyt vapaita huoneita."
        );
      }
    } catch (error) {
      console.error("Failed to fetch available rooms", error);
      setError(
        getErrorMessage(error, "Vapaiden huoneiden hakeminen epäonnistui.")
      );
    } finally {
      setLoadingRooms(false);
    }
  }

  async function handleCreateReservation() {
    if (!selectedCustomer) {
      setError("Valitse asiakas ennen varauksen luomista.");
      return;
    }

    if (!selectedRoomId) {
      setError("Valitse ensin huone.");
      return;
    }

    setError("");
    setCreatingReservation(true);

    try {
      const reservation = await createReservation({
        customerId: selectedCustomer.id,
        roomId: selectedRoomId,
        checkInDate,
        checkOutDate,
        guestCount,
        notes: notes.trim() || null,
      });

      setCreatedReservation(reservation);

      showSuccessToast(
        `Varaus luotu onnistuneesti! Varaus ID: ${reservation.id}`
      );
    } catch (error) {
      console.error("Failed to create reservation", error);
      setError(getErrorMessage(error, "Varauksen luominen epäonnistui."));
    } finally {
      setCreatingReservation(false);
    }
  }

  return (
    <AuthGuard >
    <main className="min-h-screen bg-slate-100 p-6">
      <div className="mx-auto max-w-6xl">
        <BackToHomeLink />

        <div className="mb-8">
          <h1 className="text-3xl font-bold text-slate-900">Tee varaus</h1>
          <p className="mt-2 text-slate-600">
            Valitse asiakas, päivämäärät, vapaa huone ja luo varaus.
          </p>
        </div>

        {successToast && (
          <div className="fixed right-6 top-6 z-50 rounded-2xl border border-emerald-200 bg-emerald-50 px-5 py-4 text-emerald-800 shadow-lg">
            <p className="font-semibold">Toiminto onnistui</p>
            <p className="mt-1 text-sm">{successToast}</p>
          </div>
        )}

        <form
          onSubmit={handleSearchRooms}
          className="mb-8 rounded-2xl bg-white p-6 shadow-sm"
        >
          <div className="grid gap-4 md:grid-cols-4">
            <div>
              <label className="mb-2 block text-sm font-medium text-slate-700">
                Check-in
              </label>
              <input
                type="date"
                required
                value={checkInDate}
                onChange={(event) => setCheckInDate(event.target.value)}
                className="w-full rounded-lg border border-slate-300 p-3 text-slate-900"
              />
            </div>

            <div>
              <label className="mb-2 block text-sm font-medium text-slate-700">
                Check-out
              </label>
              <input
                type="date"
                required
                value={checkOutDate}
                onChange={(event) => setCheckOutDate(event.target.value)}
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
                required
                value={guestCount}
                onChange={(event) => setGuestCount(Number(event.target.value))}
                className="w-full rounded-lg border border-slate-300 p-3 text-slate-900"
              />
            </div>

            <div className="flex items-end">
              <button
                type="submit"
                disabled={loadingRooms}
                className="w-full rounded-lg bg-slate-900 p-3 font-semibold text-white hover:bg-slate-800 disabled:bg-slate-400"
              >
                {loadingRooms ? "Haetaan..." : "Hae vapaat huoneet"}
              </button>
            </div>
          </div>
        </form>

        {error && (
          <div className="mb-6 rounded-2xl border border-red-300 bg-red-50 p-4 shadow-sm">
            <p className="font-semibold text-red-800">⚠️ Toiminto epäonnistui</p>
            <p className="mt-1 text-sm text-red-700">{error}</p>
          </div>
        )}

        <div className="grid gap-6 lg:grid-cols-[2fr_1fr]">
          <section>
            <h2 className="mb-4 text-xl font-bold text-slate-900">
              Vapaat huoneet
            </h2>

            {availableRooms.length === 0 ? (
              <div className="rounded-2xl bg-white p-6 text-slate-600 shadow-sm">
                Ei haettuja huoneita vielä.
              </div>
            ) : (
              <div className="grid gap-4 md:grid-cols-2">
                {availableRooms.map((room) => (
                  <button
                    key={room.id}
                    type="button"
                    onClick={() => setSelectedRoomId(room.id)}
                    className={`rounded-2xl border bg-white p-5 text-left shadow-sm transition hover:-translate-y-1 hover:shadow-md ${
                      selectedRoomId === room.id
                        ? "border-slate-900 ring-2 ring-slate-900"
                        : "border-transparent"
                    }`}
                  >
                    <div className="mb-3 flex justify-between gap-4">
                      <div>
                        <h3 className="text-lg font-bold text-slate-900">
                          Huone {room.roomNumber}
                        </h3>
                        <p className="text-sm text-slate-500">
                          {getRoomTypeName(room.roomType)}
                        </p>
                      </div>

                      <p className="font-bold text-slate-900">
                        {room.basePricePerNight} € / yö
                      </p>
                    </div>

                    <p className="mb-3 text-sm text-slate-600">
                      {room.description ?? "Ei kuvausta."}
                    </p>

                    <p className="text-sm font-semibold text-slate-700">
                      Max {room.maxGuests} henkilöä
                    </p>
                  </button>
                ))}
              </div>
            )}
          </section>

          <aside className="rounded-2xl bg-white p-6 shadow-sm">
            <h2 className="mb-4 text-xl font-bold text-slate-900">
              Varauksen tiedot
            </h2>

            <div className="mb-4 grid grid-cols-2 gap-2 rounded-xl bg-slate-100 p-1">
              <button
                type="button"
                onClick={() => {
                  setCustomerMode("existing");
                  setError("");
                }}
                className={`rounded-lg px-3 py-2 text-sm font-semibold ${
                  customerMode === "existing"
                    ? "bg-white text-slate-900 shadow-sm"
                    : "text-slate-600"
                }`}
              >
                Vanha asiakas
              </button>

              <button
                type="button"
                onClick={() => {
                  setCustomerMode("new");
                  resetSelectedCustomer();
                  setError("");
                }}
                className={`rounded-lg px-3 py-2 text-sm font-semibold ${
                  customerMode === "new"
                    ? "bg-white text-slate-900 shadow-sm"
                    : "text-slate-600"
                }`}
              >
                Uusi asiakas
              </button>
            </div>

            {customerMode === "existing" && (
              <>
                <label className="mb-2 block text-sm font-medium text-slate-700">
                  Hae asiakas
                </label>

                <div className="mb-4 flex gap-2">
                  <input
                    type="text"
                    value={customerSearchTerm}
                    onChange={(event) =>
                      setCustomerSearchTerm(event.target.value)
                    }
                    className="w-full rounded-lg border border-slate-300 p-3 text-slate-900"
                    placeholder="Nimi, sähköposti tai puhelin"
                  />

                  <button
                    type="button"
                    onClick={handleSearchCustomers}
                    disabled={searchingCustomers}
                    className="rounded-lg bg-slate-900 px-4 font-semibold text-white hover:bg-slate-800 disabled:bg-slate-400"
                  >
                    {searchingCustomers ? "..." : "Hae"}
                  </button>
                </div>

                {selectedCustomer && (
                  <div className="mb-4 rounded-xl border border-emerald-200 bg-emerald-50 p-4 text-sm text-emerald-900">
                    <div className="flex items-start justify-between gap-3">
                      <div>
                        <p className="font-bold">{selectedCustomer.fullName}</p>
                        <p>{selectedCustomer.email ?? "Ei sähköpostia"}</p>
                        <p>
                          {selectedCustomer.phoneNumber ??
                            "Ei puhelinnumeroa"}
                        </p>
                      </div>

                      <button
                        type="button"
                        onClick={resetSelectedCustomer}
                        className="text-xs font-semibold text-emerald-900 underline"
                      >
                        Vaihda
                      </button>
                    </div>
                  </div>
                )}

                {!selectedCustomer && customerSearchResults.length > 0 && (
                  <div className="mb-4 grid gap-2">
                    {customerSearchResults.map((customer) => (
                      <button
                        key={customer.id}
                        type="button"
                        onClick={() => {
                          setSelectedCustomer(customer);
                          setCustomerSearchResults([]);
                          setCustomerSearchTerm(customer.fullName);
                        }}
                        className="rounded-xl border border-slate-200 bg-slate-50 p-3 text-left text-sm hover:border-slate-900"
                      >
                        <p className="font-bold text-slate-900">
                          {customer.fullName}
                        </p>
                        <p className="text-slate-600">
                          {customer.email ?? "Ei sähköpostia"}
                        </p>
                        <p className="text-slate-600">
                          {customer.phoneNumber ?? "Ei puhelinnumeroa"}
                        </p>
                      </button>
                    ))}
                  </div>
                )}
              </>
            )}

            {customerMode === "new" && (
              <div className="mb-4 rounded-xl border border-slate-200 bg-slate-50 p-4">
                <label className="mb-2 block text-sm font-medium text-slate-700">
                  Asiakkaan nimi
                </label>
                <input
                  type="text"
                  value={newCustomerFullName}
                  onChange={(event) =>
                    setNewCustomerFullName(event.target.value)
                  }
                  className="mb-3 w-full rounded-lg border border-slate-300 p-3 text-slate-900"
                  placeholder="Esim. Liisa Järvinen"
                />

                <label className="mb-2 block text-sm font-medium text-slate-700">
                  Sähköposti
                </label>
                <input
                  type="email"
                  value={newCustomerEmail}
                  onChange={(event) => setNewCustomerEmail(event.target.value)}
                  className="mb-3 w-full rounded-lg border border-slate-300 p-3 text-slate-900"
                  placeholder="liisa@example.com"
                />

                <label className="mb-2 block text-sm font-medium text-slate-700">
                  Puhelinnumero
                </label>
                <input
                  type="text"
                  value={newCustomerPhoneNumber}
                  onChange={(event) =>
                    setNewCustomerPhoneNumber(event.target.value)
                  }
                  className="mb-3 w-full rounded-lg border border-slate-300 p-3 text-slate-900"
                  placeholder="0401234567"
                />

                <label className="mb-2 block text-sm font-medium text-slate-700">
                  Asiakkaan lisätiedot
                </label>
                <textarea
                  value={newCustomerNotes}
                  onChange={(event) => setNewCustomerNotes(event.target.value)}
                  className="mb-4 min-h-20 w-full rounded-lg border border-slate-300 p-3 text-slate-900"
                  placeholder="Esim. allergiat tai erityistoiveet"
                />

                <button
                  type="button"
                  onClick={handleCreateCustomer}
                  disabled={creatingCustomer}
                  className="w-full rounded-lg bg-slate-900 p-3 font-semibold text-white hover:bg-slate-800 disabled:bg-slate-400"
                >
                  {creatingCustomer ? "Luodaan asiakasta..." : "Luo asiakas"}
                </button>
              </div>
            )}

            <label className="mb-2 block text-sm font-medium text-slate-700">
              Varauksen lisätiedot
            </label>
            <textarea
              value={notes}
              onChange={(event) => setNotes(event.target.value)}
              className="mb-4 min-h-28 w-full rounded-lg border border-slate-300 p-3 text-slate-900"
              placeholder="Esim. myöhäinen saapuminen"
            />

            <div className="mb-4 rounded-xl bg-slate-50 p-4 text-sm text-slate-600">
              <p>
                Asiakas:{" "}
                <span className="font-semibold text-slate-900">
                  {selectedCustomer?.fullName ?? "Ei valittu"}
                </span>
              </p>
              <p>
                Huone:{" "}
                <span className="font-semibold text-slate-900">
                  {selectedRoomId ?? "Ei valittu"}
                </span>
              </p>
              <p>
                Aika:{" "}
                <span className="font-semibold text-slate-900">
                  {checkInDate || "?"} → {checkOutDate || "?"}
                </span>
              </p>
              <p>
                Henkilöt:{" "}
                <span className="font-semibold text-slate-900">
                  {guestCount}
                </span>
              </p>
            </div>

            <button
              type="button"
              onClick={handleCreateReservation}
              disabled={
                !selectedCustomer || !selectedRoomId || creatingReservation
              }
              className="w-full rounded-lg bg-emerald-700 p-3 font-semibold text-white hover:bg-emerald-800 disabled:bg-slate-300"
            >
              {creatingReservation ? "Luodaan..." : "Luo varaus"}
            </button>

            {createdReservation && (
              <div className="mt-6 rounded-xl bg-emerald-50 p-4 text-emerald-800">
                <p className="font-bold">Varaus luotu!</p>
                <p>Varaus ID: {createdReservation.id}</p>
                <p>Hinta: {createdReservation.totalPrice} €</p>
              </div>
            )}
          </aside>
        </div>
      </div>
    </main>
    </AuthGuard>
  );
}