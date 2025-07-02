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
      align-items: center;

      button {
        background-color: transparent;
        border: none;
        margin-left: 5px;
        cursor: pointer;
        border-radius: 40px;
        width: 30px;
        padding: 0;
        margin-right: 10px;

        &:hover {
          transform: scale(1.05);
          outline: 4px solid rgba($secondary, 0.2);
        }

        img {
          width: 100%;
          display: flex;
          align-items: center;
        }
      }
    }
  }
</style>

<template>
  <div class="list-header">
    <h1>{{ title }}</h1>

    <div class="search-area">
      <button @click="onClickCreate"><img :src="addIcon" alt="add" /></button>
      <SearchInput placeholder="Search..." :id="title.toLowerCase()" v-model="searchQuery" />
    </div>
  </div>
</template>

<script lang="ts" setup>
  const addIcon = new URL('@/assets/icons/icon_add.svg', import.meta.url).href;
  import SearchInput from '@/components/forms/searchInput.vue';
  import { defineProps, ref, watch } from 'vue';

  interface Props {
    title: string;
    onSearch: (search: string) => void;
    onClickCreate: () => void;
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
    }, 500);
  });
</script>
