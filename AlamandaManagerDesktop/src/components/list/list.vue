<style lang="scss" scoped>
  @use '@/assets/variables.scss' as *;
  .list {

  }
</style>

<template>
  <div class="list">
    <ListHeader :onSearch="onSearch" :onFilter="onFilterChange" :title="title" />
    <div class="list-body">
      <slot />
    </div>
    <ListPagination :onChangePage="onChangePage" :totalPages="10" :currentPage="currentPage" />
  </div>
</template>

<script lang="ts" setup>
  import ListPagination from './listPagination.vue';
  import ListHeader from './listHeader.vue';
import { ref } from 'vue';

  interface Props {
    title: string;
  }

  const props = defineProps<Props>();
  const currentPage = ref(1);
  const currentFilters = ref(null);

  const onFilterChange = () => {

  }

  const onSearch = () => {

  }

  const onChangePage = (page: number) => {
    currentPage.value = page;
  }
</script>