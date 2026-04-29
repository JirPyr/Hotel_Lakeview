import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import {
  getAvailableRooms,
  createReservation,
  cancelReservation,
  updateReservation,
} from "../services/reservationService";

describe("reservationService", () => {
  const originalBaseUrl = process.env.NEXT_PUBLIC_API_BASE_URL;

  beforeEach(() => {
    process.env.NEXT_PUBLIC_API_BASE_URL = "http://localhost:3000/api";
    global.fetch = vi.fn(() =>
      Promise.resolve(
        new Response(JSON.stringify({ id: 1 }), { status: 200 })
      ) as Promise<Response>
    ) as unknown as typeof fetch;
  });

  afterEach(() => {
    process.env.NEXT_PUBLIC_API_BASE_URL = originalBaseUrl;
    vi.restoreAllMocks();
  });

  it("builds available rooms query parameters", async () => {
    await getAvailableRooms("2026-05-01", "2026-05-05", 2);

    expect(fetch).toHaveBeenCalledTimes(1);
    expect(fetch).toHaveBeenCalledWith(
      "http://localhost:5000/api/Rooms/available?checkInDate=2026-05-01&checkOutDate=2026-05-05&guestCount=2",
      {
        method: undefined,
        headers: { "Content-Type": "application/json" },
      }
    );
  });

  it("creates a reservation with POST body", async () => {
    await createReservation({
      customerId: 1,
      roomId: 2,
      checkInDate: "2026-05-01",
      checkOutDate: "2026-05-05",
      guestCount: 2,
      notes: "Test",
    });

    expect(fetch).toHaveBeenCalledWith("http://localhost:5000/api/reservations", {
      method: "POST",
      body: JSON.stringify({
        customerId: 1,
        roomId: 2,
        checkInDate: "2026-05-01",
        checkOutDate: "2026-05-05",
        guestCount: 2,
        notes: "Test",
      }),
      headers: { "Content-Type": "application/json" },
    });
  });

  it("deletes a reservation", async () => {
    await cancelReservation(5);

    expect(fetch).toHaveBeenCalledWith("http://localhost:5000/api/Reservations/5", {
      method: "DELETE",
      headers: { "Content-Type": "application/json" },
    });
  });

  it("updates a reservation with PUT body", async () => {
    await updateReservation(7, {
      roomId: 2,
      checkInDate: "2026-05-10",
      checkOutDate: "2026-05-15",
      guestCount: 3,
      notes: "Updated",
    });

    expect(fetch).toHaveBeenCalledWith("http://localhost:5000/api/Reservations/7", {
      method: "PUT",
      body: JSON.stringify({
        roomId: 2,
        checkInDate: "2026-05-10",
        checkOutDate: "2026-05-15",
        guestCount: 3,
        notes: "Updated",
      }),
      headers: { "Content-Type": "application/json" },
    });
  });
});
