import type { APIRoute } from "astro";

export const ALL: APIRoute = ({ params, request }) => {
  console.log("ASDDSDSAADSDSA");
  return new Response(
    JSON.stringify({
      message: "This was a GET!",
    })
  );
};
