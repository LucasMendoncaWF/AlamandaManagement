<template>
  <List
    title="Comics"
    label="comic"
    :showFieldsOnTable="['name', 'categories', 'publish_Date']"
    :searchFunction="getComics"
    :addItemFunction="addComic"
    :updateItemFunction="updateComic"
    :deleteFunction="deleteComic"
    :onClickItem="onClickChapter"
  />
</template>

<script lang="ts" setup>
import List from '@/components/lists/list.vue';
import { getComics, addComic, updateComic, deleteComic } from '@/api/comics';
import router from '@/router';
import urls from '@/router/urls';

const onClickChapter = (id: number) => {
  router.push(urls.comicChapters(id));
}
</script>