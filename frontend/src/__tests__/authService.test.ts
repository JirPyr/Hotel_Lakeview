import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import {
  login,
  saveAuth,
  logout,
  getToken,
  getRole,
  isLoggedIn,
  isManagement,
} from "../services/authService";
import * as tokenUtils from "../utils/tokenUtils";

describe("authService", () => {
  const originalBaseUrl = process.env.NEXT_PUBLIC_API_BASE_URL;

  beforeEach(() => {
    process.env.NEXT_PUBLIC_API_BASE_URL = "http://localhost:5000/api";
    localStorage.clear();
    
    // Mock isTokenExpired to return false by default (token valid)
    vi.spyOn(tokenUtils, "isTokenExpired").mockReturnValue(false);
    
    global.fetch = vi.fn(() =>
      Promise.resolve(
        new Response(
          JSON.stringify({
            token: "token",
            role: "Management",
            username: "admin",
          }),
          { status: 200 }
        )
      ) as Promise<Response>
    ) as unknown as typeof fetch;
  });

  afterEach(() => {
    process.env.NEXT_PUBLIC_API_BASE_URL = originalBaseUrl;
    vi.restoreAllMocks();
  });

  it("sends login request and returns data", async () => {
    const response = await login({ username: "admin", password: "secret" });

    expect(response).toEqual({
      token: "token",
      role: "Management",
      username: "admin",
    });

    expect(fetch).toHaveBeenCalledTimes(1);
    expect(fetch).toHaveBeenCalledWith("http://localhost:5000/api/Auth/login", {
      method: "POST",
      body: JSON.stringify({ username: "admin", password: "secret" }),
      headers: { "Content-Type": "application/json" },
    });
  });

  it("stores and clears auth data", () => {
    saveAuth({ token: "token", role: "Management", username: "admin" });

    expect(getToken()).toBe("token");
    expect(getRole()).toBe("Management");
    expect(isLoggedIn()).toBe(true);
    expect(isManagement()).toBe(true);

    logout();

    expect(getToken()).toBeNull();
    expect(getRole()).toBeNull();
    expect(isLoggedIn()).toBe(false);
    expect(isManagement()).toBe(false);
  });

  it("returns false when token is expired", () => {
    saveAuth({ token: "token", role: "Management", username: "admin" });
    vi.spyOn(tokenUtils, "isTokenExpired").mockReturnValue(true);

    expect(isLoggedIn()).toBe(false);
    expect(getToken()).toBeNull(); // Token poistettu
  });
});