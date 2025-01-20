import { post } from "@/lib/api/http";
import type { APIRoute } from "astro";

export const POST: APIRoute = async ({ cookies, request }) => {
  const data = await request.formData();
  const username = data.get("username");

  // Validate the data - you'll probably want to do more than this
  if (!username) {
    return new Response(
      JSON.stringify({
        message: "Missing required fields",
      }),
      { status: 400 }
    );
  }

  const body = {
    userName: username,
  };

  const response = await post(
    "http://localhost:5212/registerUsername/",
    body,
    cookies.get("session")?.value
  );

  return response;
};
