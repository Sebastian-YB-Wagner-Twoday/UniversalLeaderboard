<script setup lang="ts">
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { post } from "@/lib/api/http";

async function logout(e: Event) {
  e.preventDefault();

  const login = await post("/api/auth/logout", {});
  return login.json;
}
</script>

<template>
  <Card>
    <CardHeader>
      <CardTitle>Log out</CardTitle>
    </CardHeader>
    <CardContent>
      <div>
        <Button @click="logout"> Log out </Button>
      </div>
    </CardContent>
  </Card>
</template>
