<script setup lang="ts" generic="TValue">
import { columns } from "./columns";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import type { ScoreEntry } from "@/model/scores/ScoreEntry.model";
import { FlexRender } from "@tanstack/vue-table";
import { type Table as TableType } from "@tanstack/vue-table";

const props = defineProps<{
  table: TableType<ScoreEntry>;
  isPending: boolean;
  error: Error | null;
}>();
</script>

<template>
  <div class="border rounded-md">
    <Table>
      <TableHeader>
        <TableRow
          v-for="headerGroup in props.table.getHeaderGroups()"
          :key="headerGroup.id"
        >
          <TableHead v-for="header in headerGroup.headers" :key="header.id">
            <FlexRender
              v-if="!header.isPlaceholder"
              :render="header.column.columnDef.header"
              :props="header.getContext()"
            />
          </TableHead>
        </TableRow>
      </TableHeader>
      <span v-if="props.isPending">Loading...</span>
      <span v-else-if="props.error">Error: {{ props.error?.message }}</span>
      <TableBody v-else>
        <template v-if="props.table.getRowModel().rows?.length">
          <TableRow
            v-for="row in props.table.getRowModel().rows"
            :key="row.id"
            :data-state="row.getIsSelected() ? 'selected' : undefined"
          >
            <TableCell v-for="cell in row.getVisibleCells()" :key="cell.id">
              <FlexRender
                :render="cell.column.columnDef.cell"
                :props="cell.getContext()"
              />
            </TableCell>
          </TableRow>
        </template>
        <template v-else>
          <TableRow>
            <TableCell :colspan="columns.length" class="h-24 text-center">
              No results.
            </TableCell>
          </TableRow>
        </template>
      </TableBody>
    </Table>
  </div>
</template>
