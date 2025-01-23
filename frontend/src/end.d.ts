// src/env.d.ts

/// <reference types="astro/client" />

declare namespace App {
  // Note: 'import {} from ""' syntax does not work in .d.ts files.
  interface Locals {
    session: Session | null;
    user: import("./model/user/LeaderBoardUser.model").LeaderBoardUser | null;
    // This will allow us to set the cache duration for each page.
    cache(seconds: number): void;
  }
}
