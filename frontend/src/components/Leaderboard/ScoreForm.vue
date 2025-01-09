<script setup lang="ts">
import { ref } from "vue";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import type { Contest } from "@/model/Contest.model";

const props = defineProps<{
  contest: Contest;
}>();

const responseMessage = ref<string>();

async function submit(e: Event) {
  e.preventDefault();
  const formData = new FormData(e.currentTarget as HTMLFormElement);
  console.log("contest: ", props.contest);

  const response = await fetch("/api/createScore", {
    method: "POST",
    body: JSON.stringify({
      score: formData.get("score")?.valueOf(),
      contestId: props.contest.id,
    }),
  });

  const data = await response.json();
  responseMessage.value = data.message;
}
</script>

<template>
  <form @submit="submit">
    <label>
      Score
      <Input id="score" type="text" name="score" placeholder="new score" />
    </label>
    <Button> Add new Score </Button>
  </form>
  <p v-if="responseMessage">{{ responseMessage }}</p>
</template>
