<template>
  <List
    v-if="tab === 'comics'"
    title="Comics"
    label="comic"
    :maxImageSize="2000"
    :searchFunction="getComics"
    :getFieldFunction="getComicsFields"
    :addItemFunction="addComic"
    :updateItemFunction="updateComic"
    :deleteFunction="deleteComic"
  />
</template>

<script lang="ts" setup>
  import { ref } from 'vue';
  import List from '@/components/list/list.vue';
  import { getComics, getComicsFields, addComic, updateComic, deleteComic} from '@/api/comics';

  const tab = ref('comics');
</script>