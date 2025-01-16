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

  const body = JSON.stringify({
    score: score,
    contestId: contestId,
  });

  const response = await fetch("http://localhost:5212/contest/submitScore", {
    method: "POST",
    body: body,
    headers: {
      "Content-Type": "application/json",
      Authorization: "Bearer " + cookies.get("session")?.value || "",
    },
  });

  return response;
};
