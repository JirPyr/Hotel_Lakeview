import { apiRequest } from "./apiClient";
import type {
  AvailableRoom,
  CreateReservationRequest,
  Reservation,
  UpdateReservationRequest,
} from "@/types/reservation";
import type { PagedResult } from "@/types/room";

export async function getAvailableRooms(
  checkInDate: string,
  checkOutDate: string,
  guestCount: number
): Promise<AvailableRoom[]> {
  const params = new URLSearchParams({
    checkInDate,
    checkOutDate,
    guestCount: guestCount.toString(),
  });

  return apiRequest<AvailableRoom[]>(
    `/Rooms/available?${params.toString()}`
  );
}

export async function createReservation(
  request: CreateReservationRequest
): Promise<Reservation> {
  return apiRequest<Reservation>("/reservations", {
    method: "POST",
    body: JSON.stringify(request),
  });
}

export async function getReservations(
  page: number,
  pageSize: number,
  searchTerm: string = ""
): Promise<PagedResult<Reservation>> {
  const params = new URLSearchParams({
    page: page.toString(),
    pageSize: pageSize.toString(),
  });

  if (searchTerm.trim()) {
    params.set("searchTerm", searchTerm.trim());
  }

  return apiRequest<PagedResult<Reservation>>(
    `/reservations?${params.toString()}`
  );
}

export async function cancelReservation(id: number): Promise<void> {
  return apiRequest<void>(`/Reservations/${id}`, {
    method: "DELETE",
  });
}

export async function updateReservation(
  id: number,
  request: UpdateReservationRequest
): Promise<Reservation> {
  return apiRequest<Reservation>(`/Reservations/${id}`, {
    method: "PUT",
    body: JSON.stringify(request),
  });
}