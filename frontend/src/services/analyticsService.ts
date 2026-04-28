import { apiRequest } from "./apiClient";
import type {
  OccupancyReport,
  PopularRoomTypesReport,
  ReservationSummaryReport,
  RevenueReport,
} from "@/types/analytics";

export async function getReservationSummary(
  startDate: string,
  endDate: string
): Promise<ReservationSummaryReport> {
  const params = new URLSearchParams({
    startDate,
    endDate,
  });

  return apiRequest<ReservationSummaryReport>(
    `/Analytics/reservation-summary?${params.toString()}`
  );
}

export async function getOccupancyReport(
  startDate: string,
  endDate: string
): Promise<OccupancyReport> {
  const params = new URLSearchParams({
    startDate,
    endDate,
  });

  return apiRequest<OccupancyReport>(
    `/Analytics/occupancy?${params.toString()}`
  );
}

export async function getRevenueReport(
  startDate: string,
  endDate: string
): Promise<RevenueReport> {
  const params = new URLSearchParams({
    startDate,
    endDate,
  });

  return apiRequest<RevenueReport>(
    `/Analytics/revenue?${params.toString()}`
  );
}

export async function getPopularRoomTypesReport(
  startDate: string,
  endDate: string
): Promise<PopularRoomTypesReport> {
  const params = new URLSearchParams({
    startDate,
    endDate,
  });

  return apiRequest<PopularRoomTypesReport>(
    `/Analytics/popular-room-types?${params.toString()}`
  );
}