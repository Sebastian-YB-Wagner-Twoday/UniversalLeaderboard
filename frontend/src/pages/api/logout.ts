import { post } from "@/lib/api/http";
import type { APIRoute } from "astro";

export const POST: APIRoute = async ({ cookies, locals }) => {
  const body = "logout";

  locals.session = null;
  locals.user = null;

  const token = cookies.get("session")?.value;

  cookies.delete("session", { secure: true, path: "/" });

  cookies.delete("refresh", { secure: true, path: "/" });

  await post("http://localhost:5212/logout", body, token);

  const okResponse = new Response(null, { status: 200 });

  return okResponse;
};
