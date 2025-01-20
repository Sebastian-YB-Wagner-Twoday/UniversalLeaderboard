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

  const login = await post("/api/auth/login", { email, password });

  return login.json();
}
</script>

<template>
  <Card>
    <CardHeader>
      <CardTitle>Login</CardTitle>
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
          <Button> Login </Button>
        </form>
      </div>
    </CardContent>
  </Card>
</template>
