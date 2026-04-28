import { apiRequest } from "./apiClient";
import { RoomImage } from "@/types/roomImage";

export async function getRoomImages(roomId: number): Promise<RoomImage[]> {
  return apiRequest<RoomImage[]>(`/Rooms/${roomId}/images`);
}

export async function uploadRoomImage(
  roomId: number,
  file: File,
  sortOrder: number,
  isPrimary: boolean
): Promise<RoomImage> {
  const formData = new FormData();

  formData.append("file", file);
  formData.append("sortOrder", sortOrder.toString());
  formData.append("isPrimary", isPrimary.toString());

  return apiRequest<RoomImage>(`/Rooms/${roomId}/images`, {
    method: "POST",
    body: formData,
    headers: {},
  });
}

export async function deleteRoomImage(imageId: number): Promise<void> {
  return apiRequest<void>(`/Rooms/images/${imageId}`, {
    method: "DELETE",
  });
}