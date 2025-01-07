import type { APIRoute } from "astro";
import { getSession } from "auth-astro/server";

export const POST: APIRoute = async ({ request }) => {
  const session = await getSession(request);

  const data = await request.formData();
  const name = data.get("name");
  const description = data.get("description");
  const rankingType = data.get("rankingType");
  const rankingOrder = data.get("rankingOrder");
  // Validate the data - you'll probably want to do more than this
  if (!name || !description || !rankingType || !rankingOrder) {
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
    AdminId: session?.user?.id,
    rankingType: rankingType,
    rankingOrder: rankingOrder,
  });

  const response = await fetch("http://localhost:5212/contest", {
    method: "POST",
    body: body,
    headers: { "Content-Type": "application/json" },
  });

  return response;
};
