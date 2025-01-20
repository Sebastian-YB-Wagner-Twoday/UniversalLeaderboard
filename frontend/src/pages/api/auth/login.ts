import { post } from "@/lib/api/http";
import type { Session } from "@/model/user/Session.model";
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

  const response = await post("http://localhost:5212/login", body);

  const sessionData: Session = await response
    .json()
    .catch((e) => console.log("Couldn't login: ", e));

  locals.session = sessionData;
  const today = new Date();
  today.setMinutes(today.getMinutes() + (sessionData.expiresIn ?? 0));

  cookies.set("session", sessionData.accessToken ?? "", {
    httpOnly: true,
    sameSite: "lax",
    secure: import.meta.env.PROD,
    expires: today,
    path: "/",
  });

  cookies.set("refresh", sessionData.refreshToken ?? "", {
    httpOnly: true,
    sameSite: "lax",
    secure: import.meta.env.PROD,
    expires: today,
    path: "/",
  });

  const okResponse = new Response(null, { status: 200 });
  return okResponse;
};
