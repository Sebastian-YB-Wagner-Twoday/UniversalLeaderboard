// @ts-check
import { defineConfig } from "astro/config";

import vue from "@astrojs/vue";

import auth from "auth-astro";

import node from "@astrojs/node";

// https://astro.build/config
export default defineConfig({
  integrations: [auth(), vue()],

  adapter: node({
    mode: "standalone",
  }),
  output: "server",
});
