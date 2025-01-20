import type { LeaderBoardUser } from "@/model/user/LeaderBoardUser.model";
import type { APIContext } from "astro";
import { get, post } from "../api/http";

export async function validateSessionToken(
  token: string,
  refreshToken: string
): Promise<any> {
  const session = await post(
    "http://localhost:5212/refresh",
    { refreshToken },
    token
  );

  return session
    .json()
    .catch((e) => console.log("Session cant be refreshed: ", e));
}

export async function getLoggedInUser(token: string): Promise<LeaderBoardUser> {
  const user = await get("http://localhost:5212/user", token);

  return user.json().catch((e) => console.log("User info can't be found: ", e));
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
