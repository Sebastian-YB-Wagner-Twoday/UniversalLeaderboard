<script setup lang="ts" generic="TValue">
import { columns } from "./columns";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import type { Contest } from "@/model/Contest.model";
import type { ScoreEntry } from "@/model/ScoreEntry.model";
import type { LeaderBoardUser } from "@/model/LeaderBoardUser.model";
import { FlexRender, getCoreRowModel, useVueTable } from "@tanstack/vue-table";
import ScoreForm from "./ScoreForm.vue";

import { useQuery } from "@tanstack/vue-query";
import { get } from "@/lib/api/http";

const props = defineProps<{
  contest: Contest;
  user: LeaderBoardUser | null;
}>();

const fetchScores = async (): Promise<ScoreEntry[]> => {
  const response = await get(
    `${window.location.origin}/api/leaderboard/${props.contest.id}`
  );

  if (!response.ok) {
    throw new Error("there was an error");
  }

  return await response.json();
};

const { isPending, isError, data, error } = useQuery({
  queryKey: ["scores", { id: props.contest.id }],
  queryFn: () => fetchScores(),
});

const shownColumns = columns(props.contest.scoreType);

const table = useVueTable({
  get data() {
    return data.value ?? [];
  },
  columns: shownColumns,

  getCoreRowModel: getCoreRowModel(),
});
</script>

<template>
  <div class="border rounded-md">
    <ScoreForm
      :user="user"
      :contestId="contest.id"
      :rankingType="contest.rankingType"
      :rankingOrder="contest.rankingOrder"
    />
    <Table>
      <TableHeader>
        <TableRow
          v-for="headerGroup in table.getHeaderGroups()"
          :key="headerGroup.id"
        >
          <TableHead v-for="header in headerGroup.headers" :key="header.id">
            <FlexRender
              v-if="!header.isPlaceholder"
              :render="header.column.columnDef.header"
              :props="header.getContext()"
            />
          </TableHead>
        </TableRow>
      </TableHeader>
      <span v-if="isPending">Loading...</span>
      <span v-else-if="isError">Error: {{ error?.message }}</span>
      <TableBody v-else>
        <template v-if="table.getRowModel().rows?.length">
          <TableRow
            v-for="row in table.getRowModel().rows"
            :key="row.id"
            :data-state="row.getIsSelected() ? 'selected' : undefined"
          >
            <TableCell v-for="cell in row.getVisibleCells()" :key="cell.id">
              <FlexRender
                :render="cell.column.columnDef.cell"
                :props="cell.getContext()"
              />
            </TableCell>
          </TableRow>
        </template>
        <template v-else>
          <TableRow>
            <TableCell :colspan="columns.length" class="h-24 text-center">
              No results.
            </TableCell>
          </TableRow>
        </template>
      </TableBody>
    </Table>
  </div>
</template>
