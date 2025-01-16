// src/env.d.ts

/// <reference types="astro/client" />

declare namespace App {
  // Note: 'import {} from ""' syntax does not work in .d.ts files.
  interface Locals {
    session: Session | null;
    user: import("./model/LeaderBoardUser.model").LeaderBoardUser | null;
  }
}
