import { QueryClient } from "@tanstack/vue-query";
import { atom } from "nanostores";

export const queryClient = atom(new QueryClient());
