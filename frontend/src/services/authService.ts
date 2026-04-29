import { apiRequest } from "./apiClient";
import { isTokenExpired } from "@/utils/tokenUtils";
import type { LoginRequest, LoginResponse } from "@/types/auth";

const TOKEN_KEY = "hotelLakeviewToken";
const ROLE_KEY = "hotelLakeviewRole";
const USERNAME_KEY = "hotelLakeviewUsername";

export async function login(request: LoginRequest): Promise<LoginResponse> {
  return apiRequest<LoginResponse>("/Auth/login", {
    method: "POST",
    body: JSON.stringify(request),
  });
}

export function saveAuth(data: LoginResponse) {
  localStorage.setItem(TOKEN_KEY, data.token);
  localStorage.setItem(ROLE_KEY, data.role);
  localStorage.setItem(USERNAME_KEY, data.username);
}

export function logout() {
  localStorage.removeItem(TOKEN_KEY);
  localStorage.removeItem(ROLE_KEY);
  localStorage.removeItem(USERNAME_KEY);
}

export function getToken() {
  return localStorage.getItem(TOKEN_KEY);
}

export function getRole() {
  return localStorage.getItem(ROLE_KEY);
}

export function isLoggedIn() {
  const token = getToken();
  if (!token) {
    return false;
  }

  // Tarkista onko token vanhentunut
  if (isTokenExpired(token)) {
    logout();
    return false;
  }

  return true;
}

export function isManagement() {
  return getRole() === "Management";
}

/**
 * Tarkista onko käyttäjä kirjautunut ja token voimassa
 */
export function isValidSession(): boolean {
  return isLoggedIn();
}