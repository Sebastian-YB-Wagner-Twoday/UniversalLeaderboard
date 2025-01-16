// src/middleware.ts
import {
  validateSessionToken,
  setSessionTokenCookie,
  deleteSessionTokenCookie,
  getLoggedInUser,
} from "./lib/server/session.ts";
import { defineMiddleware } from "astro:middleware";
import { user } from "../src/lib/store/userStore.ts";

export const onRequest = defineMiddleware(async (context, next) => {
  const token = context.cookies.get("session")?.value ?? null;

  const refreshToken = context.cookies.get("refresh")?.value ?? null;

  if (token === null || refreshToken == null) {
    context.locals.session = null;
    user.set({ user: null });
    return next();
  }

  let session = await validateSessionToken(token, refreshToken);

  if (session) {
    let today = new Date();
    today.setMinutes(today.getMinutes() + session.expiresIn);
    setSessionTokenCookie(context, token, session.expiresAt);

    context.locals.user = await getLoggedInUser(token);
    user.set({ user: context.locals.user });
  } else {
    deleteSessionTokenCookie(context);
  }

  context.locals.session = session;
  return next();
});
