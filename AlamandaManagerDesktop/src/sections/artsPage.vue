<template>
  <List
    title="Fan Arts"
    emptyMessage="No Fan Arts were found with this search"
    errorMessage="An error occurred while searching for Fan Arts"
    :maxImageSize="200"
    :searchFunction="getArts"
    :getFieldFunction="getArtFields"
    :addItemFunction="addArt"
    :updateItemFunction="updateArt"
    :deleteFunction="deleteArt"
  />
</template>

<script setup>
  import List from '@/components/list/list.vue';
  import { getArts, getArtFields, addArt, updateArt, deleteArt } from '@/api/fanArts';
</script>