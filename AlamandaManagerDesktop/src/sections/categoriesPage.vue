<template>
  <List
    title="Category"
    label="category"
    :searchFunction="getCategories"
    :getFieldFunction="getCategoriesFields"
    :addItemFunction="addCategory"
    :updateItemFunction="updateCategory"
    :deleteFunction="deleteCategory"
  />
</template>

<script lang="ts" setup>
  import { addCategory, deleteCategory, getCategoriesFields, updateCategory, getCategories} from '@/api/category';
  import List from '@/components/list/list.vue';
</script>