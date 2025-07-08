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
    text-transform: capitalize;
    font-size: 12px;
    font-weight: bold;
    letter-spacing: 1.5px;
    color: $white;
    text-align: left;
  }
</style>

<template>
  <thead>
    <th></th>
    <th v-if="hasClickItem"></th>
    <th v-if="imageField" class="image-td">
      {{ imageField }}
    </th>

    <th
      v-for="key in showFields"
      :key="key"
    >
      <span>{{ key.replace('_', ' ') }}</span>
    </th>
  </thead>
</template>

<script lang="ts" setup>
import { computed } from 'vue';
import { ApiResponseData, ResponseKeyType } from '@/api/defaultApi';

  interface Props<T = ApiResponseData> {
    item?: T;
    hasClickItem: boolean;
  }

const props = defineProps<Props>();

const isImageUrl = (value: ResponseKeyType): boolean =>
  typeof value === 'string' && /\.(jpg|jpeg|png|webp|gif|bmp)$/i.test(value);

const imageField = computed(() => {
  return props.item && Object.keys(props.item).find((key) => props.item && isImageUrl(props.item[key]) || key.toLowerCase() === 'picture');
});

const showFields = computed(() => {
  return props.item && Object.keys(props.item)
    .filter((key) => !key.toLowerCase().includes('id') && !key.toLowerCase().includes('translations') && key !== imageField.value);
});
</script>