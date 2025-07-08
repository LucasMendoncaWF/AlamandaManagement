<template>
  <List
    title="Category"
    label="category"
    :showFieldsOnTable="['name']"
    :searchFunction="getCategories"
    :addItemFunction="addCategory"
    :updateItemFunction="updateCategory"
    :deleteFunction="deleteCategory"
  />
</template>

<script lang="ts" setup>
import { addCategory, deleteCategory, updateCategory, getCategories } from '@/api/category';
import List from '@/components/lists/list.vue';
</script>