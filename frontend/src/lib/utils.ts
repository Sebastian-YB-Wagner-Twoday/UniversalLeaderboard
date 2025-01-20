import { type ClassValue, clsx } from "clsx";
import { twMerge } from "tailwind-merge";

/** Unfortunately needed for proper use of ShadCN-vue */
export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}
