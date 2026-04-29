/**
 * JWT tokenin dekoodaus ja validointi
 */

interface JwtPayload {
  exp?: number;
  iat?: number;
  [key: string]: unknown;
}

/**
 * Dekoodaa JWT tokenin payload-osan
 */
export function decodeToken(token: string): JwtPayload | null {
  try {
    const parts = token.split(".");
    if (parts.length !== 3) {
      console.error("Invalid token format");
      return null;
    }

    const decoded = JSON.parse(atob(parts[1]));
    return decoded as JwtPayload;
  } catch (error) {
    console.error("Failed to decode token:", error);
    return null;
  }
}

/**
 * Tarkistaa onko token vanhentunut
 */
export function isTokenExpired(token: string): boolean {
  const payload = decodeToken(token);
  if (!payload || !payload.exp) {
    return true;
  }

  const expirationTime = payload.exp * 1000;
  const currentTime = Date.now();

  return currentTime >= expirationTime - 60 * 1000;
}

/**
 * Hae tokenin vanhenemisaika
 */
export function getTokenExpirationTime(token: string): Date | null {
  const payload = decodeToken(token);
  if (!payload || !payload.exp) {
    return null;
  }

  return new Date(payload.exp * 1000);
}