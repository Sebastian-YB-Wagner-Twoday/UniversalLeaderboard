// src/middleware.ts
import {
  validateSessionToken,
  setSessionTokenCookie,
  deleteSessionTokenCookie,
  getLoggedInUser,
} from "./lib/server/session.ts";
import { user } from "../src/lib/store/userStore.ts";
import { sequence } from "astro:middleware";
import { defineMiddleware } from "astro:middleware";
import type { APIContext } from "astro";
import { isr } from "./lib/services/isr.ts";

const authentication = defineMiddleware(async (context, next) => {
  if (!context.isPrerendered) {
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
  }

  return next();
});

const shouldSkipCache = (req: APIContext) => {
  // Skip the cache if the request is not a GET request.
  if (req.request.method !== "GET") return true;

  return false;
};

const caching = defineMiddleware(async (req, next) => {
  const key = req.url.pathname;
  console.log("[Middleware] onRequest", key);

  let ttl: undefined | number;
  req.locals.cache = (seconds: number = 60) => (ttl = seconds);
  if (shouldSkipCache(req)) return next();
  const cachedResponse = isr.get(key);

  if (cachedResponse) return cachedResponse;
  const response = await next();
  if (ttl !== undefined) isr.set(key, response, ttl);

  return response;
});

export const onRequest = sequence(authentication, caching);
