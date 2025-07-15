<script setup lang="ts">
import type { Contest } from "@/model/contest/Contest.model";
import type { ScoreEntry } from "@/model/scores/ScoreEntry.model";
import LeaderBoardTable from "../LeaderBoardTable.vue";
import { useQuery } from "@tanstack/vue-query";
import { useVueTable, getCoreRowModel } from "@tanstack/vue-table";
import { columns } from "../columns";
import { get } from "@/lib/api/http";
import { queryClient } from "@/lib/store/queryStore";

const props = defineProps<{
  contest: Contest;
  scores: ScoreEntry[] | undefined;
}>();

const shownColumns = columns(props.contest.scoreType);

const HOST = import.meta.env.VITE_ASTRO_HOST ?? window.location.origin;

console.log("checker: ", `${HOST}/api/leaderboard/${props.contest.id}`);

const fetchScores = async (): Promise<ScoreEntry[]> => {
  const response = await get(`${HOST}/api/leaderboard/${props.contest.id}`);

  if (!response.ok) {
    throw new Error("there was an error");
  }

  console.log("fetchScores");

  return await response.json();
};

const { isPending, data, error } = useQuery(
  {
    queryKey: ["scores", { id: props.contest.id }],
    queryFn: () => fetchScores(),
    initialData: props.scores ?? undefined,
  },
  queryClient.get()
);

const table = useVueTable({
  get data() {
    return data.value ?? [];
  },
  columns: shownColumns,

  getCoreRowModel: getCoreRowModel(),
});
</script>

<template>
  <LeaderBoardTable :table="table" :isPending :error></LeaderBoardTable>
</template>
