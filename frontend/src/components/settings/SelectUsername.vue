<script setup lang="ts">
import { ref } from "vue";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { postForm } from "@/lib/api/http";

const responseMessage = ref<string>();

async function submit(e: Event) {
  e.preventDefault();
  const formData = new FormData(e.currentTarget as HTMLFormElement);
  const response = await postForm("/api/registerUsername", formData);

  const data = await response.json();
  responseMessage.value = data.message;
}
</script>

<template>
  <form @submit="submit" class="flex flex-row m-5">
    <label>
      new UserName
      <Input type="text" id="username" name="username" required />
    </label>
    <Button>Send</Button>
    <p v-if="responseMessage">{{ responseMessage }}</p>
  </form>
</template>
