<script setup lang="ts">
import { ref } from "vue";
import { Button } from "./ui/button";
import { Input } from "./ui/input";
import { postForm } from "@/lib/api/http";

const responseMessage = ref<string>();

async function submit(e: Event) {
  e.preventDefault();
  const formData = new FormData(e.currentTarget as HTMLFormElement);
  const response = await postForm("/api/createContest", formData);

  const data = await response.json();
  responseMessage.value = data.message;

  window.location.href = `/leaderboard/${data.id}`;
}
</script>

<template>
  <form @submit="submit" class="flex flex-row m-5">
    <label>
      Name
      <Input type="text" id="name" name="name" required />
    </label>
    <label>
      description
      <textarea
        type="description"
        id="description"
        name="description"
        required
      />
    </label>
    <label>
      RankingType
      <select id="rankingType" name="rankingType" required>
        <option value="0">Incremental</option>
        <option value="1">Decremental</option>
        <option value="2">HighScore</option>
      </select>
    </label>
    <label>
      RankingOrder
      <select id="rankingOrder" name="rankingOrder" required>
        <option value="0">Ascending</option>
        <option value="1">Descending</option>
      </select>
    </label>
    <label>
      ScoreType
      <select id="scoreType" name="scoreType" required>
        <option value="0">Integer</option>
        <option value="1">Float</option>
      </select>
    </label>
    <Button>Send</Button>
    <p v-if="responseMessage">{{ responseMessage }}</p>
  </form>
</template>
