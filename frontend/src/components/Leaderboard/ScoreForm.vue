<script setup lang="ts">
import { ref } from "vue";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { useQueryClient, useMutation } from "@tanstack/vue-query";
import type { ScoreEntry } from "@/model/ScoreEntry.model";

const props = defineProps<{
  contestId: string;
}>();

const queryClient = useQueryClient();

const responseMessage = ref<string>();

async function postScore(variables: {
  score?: string | object;
  contestId: string;
  userName: string;
}) {
  const response = await fetch("/api/createScore", {
    method: "POST",
    body: JSON.stringify({
      score: variables.score,
      contestId: variables.contestId,
    }),
  });

  const data = await response.json();
  responseMessage.value = data.message;
}

async function submit(e: Event) {
  e.preventDefault();
  const formData = new FormData(e.currentTarget as HTMLFormElement);

  const score = formData.get("score")?.valueOf();

  mutate({ score, contestId: props.contestId, userName: "you" });
}

const { isPending, isError, error, isSuccess, mutate } = useMutation({
  mutationFn: postScore,
  // When mutate is called:
  onMutate: async (newScore) => {
    // Cancel any outgoing refetches
    // (so they don't overwrite our optimistic update)
    await queryClient.cancelQueries({ queryKey: ["scores"] });

    // Snapshot the previous value
    const previousScores = queryClient.getQueryData(["scores"]);

    queryClient.setQueryData(["scores"], (old: ScoreEntry[]) => {
      if (old) {
        return [...old, newScore];
      }
      return [newScore];
    });

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
