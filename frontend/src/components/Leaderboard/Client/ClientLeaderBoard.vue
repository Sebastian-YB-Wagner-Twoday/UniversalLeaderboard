<script setup lang="ts">
import type { Contest } from "@/model/contest/Contest.model";
import type { ScoreEntry } from "@/model/scores/ScoreEntry.model";
import type { LeaderBoardUser } from "@/model/user/LeaderBoardUser.model";
import LeaderBoardTable from "../LeaderBoardTable.vue";
import ScoreForm from "../ScoreForm.vue";
import { useQuery } from "@tanstack/vue-query";
import { useVueTable, getCoreRowModel } from "@tanstack/vue-table";
import { columns } from "../columns";
import { get } from "@/lib/api/http";

const props = defineProps<{
  contest: Contest;
  scores: ScoreEntry[] | undefined;
  user: LeaderBoardUser | null;
}>();

const shownColumns = columns(props.contest.scoreType);

const fetchScores = async (): Promise<ScoreEntry[]> => {
  const response = await get(
    `${window.location.origin}/api/leaderboard/${props.contest.id}`
  );

  if (!response.ok) {
    throw new Error("there was an error");
  }

  return await response.json();
};

const { isPending, isError, data, error, refetch } = useQuery({
  queryKey: ["scores", { id: props.contest.id }],
  queryFn: () => fetchScores(),
  initialData: props.scores ?? undefined,
});

const table = useVueTable({
  get data() {
    return data.value ?? [];
  },
  columns: shownColumns,

  getCoreRowModel: getCoreRowModel(),
});

refetch();
console.log(props.user);
</script>

<template>
  <ScoreForm
    v-if="props.user !== null"
    :user="props.user"
    :contestId="props.contest.id"
    :rankingType="props.contest.rankingType"
    :rankingOrder="props.contest.rankingOrder"
  />
  <LeaderBoardTable :table="table" :isPending :error></LeaderBoardTable>
</template>
