// src/middleware.ts
import {
  validateSessionToken,
  setSessionTokenCookie,
  deleteSessionTokenCookie,
} from "./lib/server/session.ts";
import { defineMiddleware } from "astro:middleware";

export const onRequest = defineMiddleware(async (context, next) => {
  const token = context.cookies.get("session")?.value ?? null;

  const refreshToken = context.cookies.get("refresh")?.value ?? null;

  if (token === null || refreshToken == null) {
    context.locals.session = null;
    return next();
  }

  let session = await validateSessionToken(token, refreshToken);

  if (session !== null) {
    let today = new Date();
    today.setMinutes(today.getMinutes() + session.expiresIn);

    setSessionTokenCookie(context, token, session.expiresAt);
  } else {
    deleteSessionTokenCookie(context);
  }

  context.locals.session = session;
  return next();
});
