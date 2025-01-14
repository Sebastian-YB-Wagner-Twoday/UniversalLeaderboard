import type { APIContext } from "astro";

export async function validateSessionToken(
  token: string,
  refreshToken: string
): Promise<any> {
  const session = await fetch("http://localhost:5212/refresh", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Cookie: token,
    },
    body: JSON.stringify({
      refreshToken,
    }),
  });

  return session.json().catch((e) => "");
}

export function setSessionTokenCookie(
  context: APIContext,
  token: string,
  expiresAt: Date
): void {
  context.cookies.set("session", token, {
    httpOnly: true,
    sameSite: "lax",
    secure: import.meta.env.PROD,
    expires: expiresAt,
    path: "/",
  });
}

export function deleteSessionTokenCookie(context: APIContext): void {
  context.cookies.set("session", "", {
    httpOnly: true,
    sameSite: "lax",
    secure: import.meta.env.PROD,
    maxAge: 0,
    path: "/",
  });
}
