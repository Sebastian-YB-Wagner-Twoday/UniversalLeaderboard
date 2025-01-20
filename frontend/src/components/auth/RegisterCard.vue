<script setup lang="ts">
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { post } from "@/lib/api/http";

async function submit(e: Event) {
  e.preventDefault();

  const formData = new FormData(e.currentTarget as HTMLFormElement);

  const email = formData.get("email")?.valueOf();
  const password = formData.get("password")?.valueOf();

  await post("/api/register", { email, password });
}
</script>

<template>
  <Card>
    <CardHeader>
      <CardTitle>Register</CardTitle>
    </CardHeader>
    <CardContent>
      <div>
        <form @submit="submit" class="flex flex-row">
          <label>
            Score
            <Input id="email" type="email" name="email" placeholder="email" />
          </label>
          <label>
            Score
            <Input
              id="password"
              type="password"
              name="password"
              placeholder="password"
            />
          </label>
          <Button> Register </Button>
        </form>
      </div>
    </CardContent>
  </Card>
</template>
