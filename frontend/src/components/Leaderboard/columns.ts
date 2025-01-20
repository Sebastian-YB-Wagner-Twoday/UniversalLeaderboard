import type { ScoreEntry } from "@/model/scores/ScoreEntry.model";
import { ScoreType } from "@/model/scores/ScoreType.model";
import type { ColumnDef } from "@tanstack/vue-table";
import { h } from "vue";

export const columns = (scoreType: ScoreType): ColumnDef<ScoreEntry>[] => {
  let columnDef: ColumnDef<ScoreEntry>[] = [
    {
      accessorKey: "userName",
      header: () => h("div", { class: "text-right" }, "Contestant"),
      cell: ({ row }) => {
        const userName: string = row.getValue("userName");

        return h("div", { class: "text-right font-medium" }, userName);
      },
    },
  ];

  if (scoreType === ScoreType.Float) {
    columnDef = columnDef.concat({
      accessorKey: "score",
      header: () => h("div", { class: "text-right" }, "Score"),
      cell: ({ row }) => {
        const score = Number.parseFloat(row.getValue("score"));
        const formatted = new Intl.NumberFormat("en", {
          maximumFractionDigits: 3,
        }).format(score);
        return h("div", { class: "text-right font-medium" }, formatted);
      },
    });
  } else if (scoreType === ScoreType.Integer) {
    columnDef = columnDef.concat({
      accessorKey: "score",
      header: () => h("div", { class: "text-right" }, "Score"),
      cell: ({ row }) => {
        const score = Number.parseInt(row.getValue("score"));
        const formatted = new Intl.NumberFormat("en").format(score);

        return h("div", { class: "text-right font-medium" }, formatted);
      },
    });
  }

  return columnDef;
};
