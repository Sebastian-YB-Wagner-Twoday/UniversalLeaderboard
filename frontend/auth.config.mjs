import GitHub from "@auth/core/providers/github";
import { defineConfig } from "auth-astro";

export default defineConfig({
  providers: [
    GitHub({
      name: "GitHub",
      clientId: import.meta.env.AUTH_GITHUB_ID,
      clientSecret: import.meta.env.AUTH_GITHUB_SECRET,

      redirectProxyUrl: "http://localhost:4321/api/auth/setup ",
    }),
  ],
  callbacks: {
    async session({ session }) {
      const body = JSON.stringify({
        userName: session.user.name,
        email: session.user.email,
      });

      const response = await fetch("http://localhost:5212/users", {
        method: "POST",
        body: body,
        headers: { "Content-Type": "application/json" },
      });

      const userInfo = await response
        .json()
        .catch((e) => console.log("there was an error: ", e));

      if (session?.user) {
        session.user.id = userInfo.id;
      }
      return session;
    },
  },
});
