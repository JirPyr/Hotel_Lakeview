import { apiRequest } from "./apiClient";
import { PagedResult, Room } from "@/types/room";

export async function getRooms(
  page: number,
  pageSize: number
): Promise<PagedResult<Room>> {
  return apiRequest<PagedResult<Room>>(
    `/rooms?page=${page}&pageSize=${pageSize}`
  );
}