<script setup lang="ts">
import { ref } from "vue";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { useQueryClient, useMutation } from "@tanstack/vue-query";
import type { ScoreEntry } from "@/model/ScoreEntry.model";
import type { LeaderBoardUser } from "@/model/LeaderBoardUser.model";
import { RankingType } from "@/model/RankingType.model";
import { RankingOrder } from "@/model/RankingOrder.model";
import { post } from "@/lib/api/http";

const props = defineProps<{
  contestId: string;
  rankingType: RankingType;
  rankingOrder: RankingOrder;
  user: LeaderBoardUser;
}>();

const queryClient = useQueryClient();

const responseMessage = ref<string>();

async function postScore(variables: ScoreEntry) {
  const response = await post("/api/createScore", {
    score: variables.score,
    contestId: variables.contestId,
  });

  const data = await response.json();
  responseMessage.value = data.message;
}

async function submit(e: Event) {
  e.preventDefault();
  const formData = new FormData(e.currentTarget as HTMLFormElement);

  const score = formData.get("score")?.valueOf();

  const scoreEntry = {
    id: "",
    score: Number(score),
    userId: props.user.id,
    contestId: props.contestId,
    userName: props.user.userName,
    relatedScoreEntries: [],
    date: new Date(),
  };
  mutate(scoreEntry);
}

const { isPending, isError, error, isSuccess, mutate } = useMutation({
  mutationFn: postScore,
  // When mutate is called:
  onMutate: async (newScoreEntry) => {
    // Cancel any outgoing refetches
    // (so they don't overwrite our optimistic update)
    await queryClient.cancelQueries({ queryKey: ["scores"] });

    // Snapshot the previous value
    const previousScores: ScoreEntry[] | undefined = queryClient.getQueryData([
      "scores",
    ]);

    if (previousScores) {
      const previousEntry = previousScores.find(
        (scoreEntry) => scoreEntry.userId === newScoreEntry.userId
      );

      console.log(previousEntry);

      if (previousEntry) {
        const index = previousScores.indexOf(previousEntry);

        if (props.rankingType === RankingType.HighScore) {
          if (props.rankingOrder === RankingOrder.Ascending) {
            if (previousEntry.score < newScoreEntry.score) {
              previousScores[index] = newScoreEntry;
            }
          } else if (props.rankingOrder === RankingOrder.Descending) {
            if (previousEntry.score > newScoreEntry.score) {
              previousScores[index] = newScoreEntry;
            }
          }
        }

        if (props.rankingType === RankingType.Incremental) {
          previousScores[index].score =
            previousScores[index].score + newScoreEntry.score;
        }

        if (props.rankingType === RankingType.Decremental) {
          previousScores[index].score =
            previousScores[index].score - newScoreEntry.score;
        }
      } else {
        queryClient.setQueryData(["scores"], (old: ScoreEntry[]) => {
          if (old) {
            return [...old, newScoreEntry];
          }
          return [newScoreEntry];
        });
      }
    }

    // Return a context object with the snapshotted value
    return { previousScores };
  },
  // If the mutation fails,
  // use the context returned from onMutate to roll back
  onError: (err, newScore, context) => {
    console.log("error ", err);
    queryClient.setQueryData(["scores"], context?.previousScores);
  },
  // Always refetch after error or success:
  onSettled: () => {
    queryClient.invalidateQueries({ queryKey: ["scores"] });
  },
});
</script>

<template>
  <form @submit="submit" class="flex flex-row">
    <label>
      Score
      <Input id="score" type="text" name="score" placeholder="new score" />
    </label>
    <Button> Add new Score </Button>
  </form>
  <p v-if="responseMessage">{{ responseMessage }}</p>
</template>
