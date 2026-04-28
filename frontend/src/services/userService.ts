import { apiRequest } from "./apiClient";
import type { PagedResult } from "@/types/room";
import type { CreateUserRequest, User } from "@/types/user";

export async function getUsers(
  page: number = 1,
  pageSize: number = 10
): Promise<PagedResult<User>> {
  return apiRequest<PagedResult<User>>(
    `/Users?page=${page}&pageSize=${pageSize}`
  );
}

export async function createUser(request: CreateUserRequest): Promise<void> {
  return apiRequest<void>("/Users", {
    method: "POST",
    body: JSON.stringify(request),
  });
}

export async function deactivateUser(id: number): Promise<void> {
  return apiRequest<void>(`/Users/${id}/deactivate`, {
    method: "PATCH",
  });
}