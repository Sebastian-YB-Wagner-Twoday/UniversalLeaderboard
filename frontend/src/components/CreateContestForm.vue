<script setup lang="ts">
import { ref } from "vue";

const responseMessage = ref<string>();

async function submit(e: Event) {
  e.preventDefault();
  const formData = new FormData(e.currentTarget as HTMLFormElement);
  const response = await fetch("/api/createContest", {
    method: "POST",
    body: formData,
  });
  const data = await response.json();
  responseMessage.value = data.message;
}
</script>

<template>
  <form @submit="submit">
    <label>
      Name
      <input type="text" id="name" name="name" required />
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
    <button>Send</button>
    <p v-if="responseMessage">{{ responseMessage }}</p>
  </form>
</template>
