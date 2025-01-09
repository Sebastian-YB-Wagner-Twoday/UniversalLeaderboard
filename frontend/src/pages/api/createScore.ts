import type { APIRoute } from "astro";
import { getSession } from "auth-astro/server";

export const POST: APIRoute = async ({ request }) => {
  const session = await getSession(request);

  const data = await request.json();
  const score = data.score;
  const contestId = data.contestId;

  // Validate the data - you'll probably want to do more than this
  if (!score || !contestId) {
    return new Response(
      JSON.stringify({
        message: "Missing required fields",
      }),
      { status: 400 }
    );
  }

  const body = JSON.stringify({
    UserId: session?.user?.id,
    score: score,
    contestId: contestId,
  });

  const response = await fetch("http://localhost:5212/contest/submitScore", {
    method: "POST",
    body: body,
    headers: { "Content-Type": "application/json" },
  });

  return response;
};
