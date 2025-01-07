// @ts-check
import { defineConfig } from "astro/config";

import vue from "@astrojs/vue";

import auth from "auth-astro";

import node from "@astrojs/node";

import tailwind from "@astrojs/tailwind";

// https://astro.build/config
export default defineConfig({
  integrations: [
    auth(),
    vue(),
    tailwind({
      applyBaseStyles: false,
    }),
  ],

  adapter: node({
    mode: "standalone",
  }),
  output: "server",
});
