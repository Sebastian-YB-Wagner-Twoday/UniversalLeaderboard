import type { ScoreEntry } from "@/model/ScoreEntry.model";
import { ScoreType } from "@/model/ScoreType.model";
import type { ColumnDef } from "@tanstack/vue-table";
import { h } from "vue";

export const columns = (scoreType: ScoreType): ColumnDef<ScoreEntry>[] => [
  {
    accessorKey: "userName",
    header: () => h("div", { class: "text-right" }, "Contestant"),
    cell: ({ row }) => {
      const userName: string = row.getValue("userName");

      return h("div", { class: "text-right font-medium" }, userName);
    },
  },
  {
    accessorKey: "score",
    header: () => h("div", { class: "text-right" }, "Score"),
    cell: ({ row }) => {
      if (scoreType === ScoreType.Float) {
        const score = Number.parseFloat(row.getValue("score"));
        const formatted = new Intl.NumberFormat("en", {
          maximumFractionDigits: 3,
        }).format(score);

        return h("div", { class: "text-right font-medium" }, formatted);
      } else if (scoreType === ScoreType.Integer) {
        const score = Number.parseInt(row.getValue("score"));
        const formatted = new Intl.NumberFormat("en").format(score);

        return h("div", { class: "text-right font-medium" }, formatted);
      }
    },
  },
];
