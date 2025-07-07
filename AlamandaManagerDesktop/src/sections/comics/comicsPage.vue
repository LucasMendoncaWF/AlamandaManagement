<template>
  <List
    v-if="currentComicSelected === null"
    title="Comics"
    label="comic"
    :showFieldsOnTable="['name', 'categories', 'publish_Date']"
    :maxImageSize="2000"
    :searchFunction="getComics"
    :getFieldFunction="getComicsFields"
    :addItemFunction="addComic"
    :updateItemFunction="updateComic"
    :deleteFunction="deleteComic"
    :onClickItem="onClickChapter"
  />
</template>

<script lang="ts" setup>
  import { ref } from 'vue';
  import List from '@/components/list/list.vue';
  import { getComics, getComicsFields, addComic, updateComic, deleteComic} from '@/api/comics';

  const currentComicSelected = ref<number | null>(null)
  const onClickChapter = (id: number) => {
    currentComicSelected.value = id;
  }
</script>