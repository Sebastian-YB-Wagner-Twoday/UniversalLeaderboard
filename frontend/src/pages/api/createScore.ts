import { post } from "@/lib/api/http";
import type { APIRoute } from "astro";

export const POST: APIRoute = async ({ cookies, request }) => {
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

  const body = {
    score: score,
    contestId: contestId,
  };

  const response = await post(
    "http://localhost:5212/contest/submitScore",
    body,
    cookies.get("session")?.value
  );

  return response;
};
