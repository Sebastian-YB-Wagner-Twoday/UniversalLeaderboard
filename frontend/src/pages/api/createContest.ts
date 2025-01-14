import type { APIRoute } from "astro";

export const POST: APIRoute = async ({ cookies, request }) => {
  const data = await request.formData();
  const name = data.get("name");
  const description = data.get("description");
  const rankingType = data.get("rankingType");
  const rankingOrder = data.get("rankingOrder");
  const scoreType = data.get("scoreType");

  // Validate the data - you'll probably want to do more than this
  if (!name || !description || !rankingType || !rankingOrder || !scoreType) {
    return new Response(
      JSON.stringify({
        message: "Missing required fields",
      }),
      { status: 400 }
    );
  }

  const body = JSON.stringify({
    name: name,
    description: description,
    rankingType: rankingType,
    rankingOrder: rankingOrder,
    scoreType: scoreType,
  });

  const response = await fetch("http://localhost:5212/contest", {
    method: "POST",
    body: body,
    headers: {
      "Content-Type": "application/json",
      Authorization: "Bearer " + cookies.get("session")?.value || "",
    },
  });

  return response;
};
