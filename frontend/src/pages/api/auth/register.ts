import { post } from "@/lib/api/http";
import type { APIRoute } from "astro";

export const POST: APIRoute = async ({ cookies, request, locals }) => {
  const data = await request.json();
  const email = data.email;
  const password = data.password;

  // Validate the data - you'll probably want to do more than this
  if (!email || !password) {
    return new Response(
      JSON.stringify({
        message: "Missing required fields",
      }),
      { status: 400 }
    );
  }

  const body = {
    email: email,
    password: password,
  };

  const response = post("http://localhost:5212/register", body);

  return response;
};
