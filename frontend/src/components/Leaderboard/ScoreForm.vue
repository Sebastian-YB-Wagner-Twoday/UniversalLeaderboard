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
    await queryClient.cancelQueries({
      queryKey: ["scores", { id: props.contestId }],
    });

    // Snapshot the previous value
    let previousScores: ScoreEntry[] | undefined = queryClient.getQueryData([
      "scores",
      { id: props.contestId },
    ]);

    if (previousScores) {
      let newScores = [...previousScores];

      const previousEntry = previousScores.find(
        (scoreEntry) => scoreEntry.userId === newScoreEntry.userId
      );

      if (previousEntry) {
        const index = previousScores.indexOf(previousEntry);
        const customEntry = {
          id: "",
          score: newScoreEntry.score,
          userId: props.user.id,
          contestId: props.contestId,
          userName: props.user.userName,
          relatedScoreEntries: [],
          date: new Date(),
        };

        switch (props.rankingType) {
          case RankingType.HighScore:
            if (props.rankingOrder === RankingOrder.Ascending) {
              if (previousEntry.score < newScoreEntry.score) {
                newScores[index] = newScoreEntry;
              }
            } else if (props.rankingOrder === RankingOrder.Descending) {
              if (previousEntry.score > newScoreEntry.score) {
                newScores[index] = newScoreEntry;
              }
            }
            break;

          case RankingType.Incremental:
            customEntry.score =
              previousScores[index].score + newScoreEntry.score;

            newScores.splice(index, 1, customEntry);

            break;

          case RankingType.Decremental:
            customEntry.score =
              previousScores[index].score - newScoreEntry.score;

            newScores.splice(index, 1, customEntry);
            break;

          default:
            break;
        }
      }

      console.log(newScores);

      queryClient.setQueryData(
        ["scores", { id: props.contestId }],
        () => newScores
      );
    } else {
      queryClient.setQueryData(
        ["scores", { id: props.contestId }],
        (old: ScoreEntry[]) => {
          if (old.length > 0) {
            return [...old, newScoreEntry];
          }
          return [newScoreEntry];
        }
      );
    }

    // Return a context object with the snapshotted value
    return { newScoreEntry };
  },
  // If the mutation fails,
  // use the context returned from onMutate to roll back
  onError: (err, newScoreEntry, context) => {
    console.log("error ", err);
    queryClient.setQueryData(
      ["scores", { id: props.contestId }],
      context?.newScoreEntry
    );
  },
  // Always refetch after error or success:
  onSettled: () => {
    queryClient.invalidateQueries({
      queryKey: ["scores", { id: props.contestId }],
    });
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
