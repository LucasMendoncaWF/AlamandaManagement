<style lang="scss" scoped>
  @use '@/assets/variables.scss' as *;
  thead {
    background-color: $secondary;
    margin: 0;
    position: sticky;
    top: 0;
  }
  th {
    padding: 8px 10px;
    text-transform: uppercase;
    font-size: 12px;
    font-weight: bold;
    letter-spacing: 1.5px;
    color: $white;

    &.sortable {
      cursor: pointer;

      &:hover {
        opacity: 0.6;
      }
    }

    .direction {
      &--up{
        color: red;
      }

      &--down{
        color: blue;
      }
    }
  }
</style>

<template>
  <thead>
    <th></th>
    <th v-if="imageField" class="image-td">
      {{ imageField }}
    </th>

    <th
      class="sortable"
      v-for="key in restFields"
      :key="key"
    >
      <span role="button" @click="() => sortHeader(key)">{{ key }}</span>
      <span v-if="sortBy === key" :class="`direction--${sortDirection === 'ascending' ? 'up' : 'down'}`">x</span>
    </th>
  </thead>
</template>

<script lang="ts" setup>
  import { computed } from 'vue';
  import { ApiResponseData, ResponseKeyType, SortDirection } from '@/api/defaultApi';

  interface Props<T = ApiResponseData> {
    item: T;
    sortHeader: (name: string) => void;
    sortBy: string;
    sortDirection: SortDirection;
  }

  const props = defineProps<Props>();

  const isImageUrl = (value: ResponseKeyType): boolean =>
    typeof value === 'string' && /\.(jpg|jpeg|png|webp|gif|bmp)$/i.test(value);

  const imageField = computed(() => {
    return Object.keys(props.item).find((key) => isImageUrl(props.item[key]) || key.toLowerCase() === 'picture');
  });

  const restFields = computed(() => {
    return Object.keys(props.item)
      .filter((key) => !key.toLowerCase().includes('id') && key !== imageField.value);
  });
</script>