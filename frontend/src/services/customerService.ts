import { apiRequest } from "./apiClient";
import type { Customer, CreateCustomerRequest } from "@/types/customer";
import type { PagedResult } from "@/types/room";

export async function searchCustomers(
  searchTerm: string,
  page: number = 1,
  pageSize: number = 10
): Promise<PagedResult<Customer>> {
  const params = new URLSearchParams({
    searchTerm,
    page: page.toString(),
    pageSize: pageSize.toString(),
  });

  return apiRequest<PagedResult<Customer>>(
    `/customers/search?${params.toString()}`
  );
}

export async function createCustomer(
  request: CreateCustomerRequest
): Promise<Customer> {
  return apiRequest<Customer>("/customers", {
    method: "POST",
    body: JSON.stringify({
      fullName: request.fullName,
      email: request.email ?? "",
      phoneNumber: request.phoneNumber ?? "",
      notes: request.notes ?? "",
    }),
  });
}
export async function getCustomers(
  page: number = 1,
  pageSize: number = 100
): Promise<PagedResult<Customer>> {
  return apiRequest<PagedResult<Customer>>(
    `/customers?page=${page}&pageSize=${pageSize}`
  );
}