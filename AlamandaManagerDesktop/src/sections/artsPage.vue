<template>
  <List
    title="Fan Arts"
    label="FanArt"
    :maxImageSize="1000"
    :searchFunction="getArts"
    :getFieldFunction="getArtFields"
    :addItemFunction="addArt"
    :updateItemFunction="updateArt"
    :deleteFunction="deleteArt"
  />
</template>

<script lang="ts" setup>
  import List from '@/components/list/list.vue';
  import { getArts, getArtFields, addArt, updateArt, deleteArt } from '@/api/fanArts';
</script>