import type { APIRoute } from "astro";

export const GET: APIRoute = async ({ params }) => {
  const contestId = params.id;

  // Validate the data - you'll probably want to do more than this
  if (!contestId) {
    return new Response(
      JSON.stringify({
        message: "Missing required fields",
      }),
      { status: 400 }
    );
  }

  const response = await fetch(
    `http://localhost:5212/contest/${contestId}/scores`,
    {
      method: "GET",
      headers: { "Content-Type": "application/json" },
    }
  );

  return response;
};
