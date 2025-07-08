<template>
  <List
    title="Fan Arts"
    label="FanArt"
    :showFieldsOnTable="['picture', 'social']"
    :maxImageSize="1000"
    :searchFunction="getArts"
    :addItemFunction="addArt"
    :updateItemFunction="updateArt"
    :deleteFunction="deleteArt"
  />
</template>

<script lang="ts" setup>
import List from '@/components/lists/list.vue';
import { getArts, addArt, updateArt, deleteArt } from '@/api/fanArts';
</script>