<style lang="scss" scoped>
  @use '@/assets/variables.scss' as *;

  .list-header {
    width: calc(100% - 40px);
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
      <button @click="handleFilter" v-if="filters" >
        <img class="filter-icon" :src="filterIcon" alt="filter" />
      </button>
    </div>
  </div>
  <component v-if="filters" :is="filters" :onClose="closeFilter" :onUpdate="onFilter"/>
</template>

<script lang="ts" setup>
  import SearchInput from '@/components/forms/searchInput.vue';
  import { defineProps, ref, VueElement, watch } from 'vue';
  const filterIcon = new URL('@/assets/icons/icon_filter.svg', import.meta.url).href;
  interface WithId {
    id: string | number;
  }

  interface Props<T extends WithId = WithId> {
    title: string;
    filters?: VueElement;
    onSearch: (search: string) => void;
    onFilter: (data: T) => void;
  };

  const props = defineProps<Props>();
  const isFilterOpen = ref(false);
  const searchQuery = ref('');
  let debounceTimer: ReturnType<typeof setTimeout> | null = null;

  const handleFilter = () => {
    isFilterOpen.value = !isFilterOpen.value;
  }

  const closeFilter = () => {
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
