<style lang="scss" scoped>
  @use '@/assets/variables.scss' as *;

  .list-header {
    width: 100%;
    padding: 20px;
    background-color: $primary;
    display: flex;
    justify-content: space-between;
    align-items: center;
    height: 40px;

    h1 {
      margin: 0;
      color: $white;
    }

    .filter-icon {
      width: 20px;
      height: 20px;
    }

    .search-area {
      display: flex;

      button {
        background-color: transparent;
        border: none;
        margin-left: 5px;
        cursor: pointer;

        &:hover {
          transform: scale(1.05);
        }
      }
    }
  }
</style>

<template>
  <div class="list-header">
    <h1>{{ title }}</h1>

    <div class="search-area">
      <SearchInput :id="title.toLowerCase()" v-model="searchQuery" />
      <button @click="handleFilter">
        <img class="filter-icon" :src="filterIcon" alt="filter" />
      </button>
    </div>
  </div>
  <Filters :onClickClose="closeFilter" v-if="isFilterOpen" />
</template>

<script lang="ts" setup>
  import Filters from './filters.vue';
  import SearchInput from '@/components/forms/searchInput.vue';
  import { defineProps, ref, watch } from 'vue';
  const filterIcon = new URL('@/assets/icons/icon_filter.svg', import.meta.url).href;
  interface Props {
    title: string;
    onSearch: (search: string) => void;
    onFilter: () => void;
  }

  const isFilterOpen = ref(false);
  const searchQuery = ref('');
  const props = defineProps<Props>();
  let debounceTimer: ReturnType<typeof setTimeout> | null = null;

  function handleFilter() {
    isFilterOpen.value = !isFilterOpen.value;
  }

  function closeFilter() {
    isFilterOpen.value = false;
  }

  watch(searchQuery, (newVal) => {
    if (debounceTimer) {
      clearTimeout(debounceTimer);
    }

    debounceTimer = setTimeout(() => {
      props.onSearch(newVal);
    }, 100);
  });
</script>
