import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { getRooms } from "../services/roomService";

describe("roomService", () => {
  const originalBaseUrl = process.env.NEXT_PUBLIC_API_BASE_URL;

  beforeEach(() => {
    process.env.NEXT_PUBLIC_API_BASE_URL = "http://localhost:3000/api";
    global.fetch = vi.fn(() =>
      Promise.resolve(
        new Response(JSON.stringify({ items: [], total: 0 }), { status: 200 })
      ) as Promise<Response>
    ) as unknown as typeof fetch;
  });

  afterEach(() => {
    process.env.NEXT_PUBLIC_API_BASE_URL = originalBaseUrl;
    vi.restoreAllMocks();
  });

  it("fetches rooms with paging parameters", async () => {
    const response = await getRooms(2, 10);

    expect(response).toEqual({ items: [], total: 0 });
    expect(fetch).toHaveBeenCalledTimes(1);
    expect(fetch).toHaveBeenCalledWith(
      "http://localhost:5000/api/rooms?page=2&pageSize=10",
      {
        method: undefined,
        headers: { "Content-Type": "application/json" },
      }
    );
  });
});
