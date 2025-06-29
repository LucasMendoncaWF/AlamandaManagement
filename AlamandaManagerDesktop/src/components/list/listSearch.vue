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
    </div>
  </div>
</template>

<script lang="ts" setup>
  import SearchInput from '@/components/forms/searchInput.vue';
  import { defineProps, ref, watch } from 'vue';
  interface WithId {
    id: string | number;
  }

  interface Props<T extends WithId = WithId> {
    title: string;
    onSearch: (search: string) => void;
    onFilter: (data: T) => void;
  };

  const props = defineProps<Props>();
  const searchQuery = ref('');
  let debounceTimer: ReturnType<typeof setTimeout> | null = null;
  watch(searchQuery, (newVal) => {
    if (debounceTimer) {
      clearTimeout(debounceTimer);
    }

    debounceTimer = setTimeout(() => {
      props.onSearch(newVal);
    }, 100);
  });
</script>
