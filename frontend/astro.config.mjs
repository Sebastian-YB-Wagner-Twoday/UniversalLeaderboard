// @ts-check
import { defineConfig } from "astro/config";

import vue from "@astrojs/vue";

import node from "@astrojs/node";

import tailwind from "@astrojs/tailwind";

// https://astro.build/config
export default defineConfig({
  integrations: [
    vue({ appEntrypoint: "/src/pages/_app" }),
    tailwind({
      applyBaseStyles: false,
    }),
  ],

  adapter: node({
    mode: "standalone",
  }),
  output: "server",
});
