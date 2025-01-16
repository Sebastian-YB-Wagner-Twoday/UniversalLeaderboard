import type { LeaderBoardUser } from "@/model/LeaderBoardUser.model";
import { atom, map } from "nanostores";

export const isLoggedIn = atom(false);

export const user = map<{ user: LeaderBoardUser | null }>({ user: null });
