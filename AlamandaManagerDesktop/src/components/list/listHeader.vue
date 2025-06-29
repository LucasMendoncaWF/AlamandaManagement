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
  }
</style>

<template>
  <thead>
    <th></th>
    <th></th>
    <th v-if="imageField" class="image-td">
      {{ imageField }}
    </th>

    <th
      v-for="key in restFields"
      :key="key"
    >
      {{ key }}
    </th>
  </thead>
</template>

<script lang="ts" setup>
  import { computed } from 'vue';
  import { ApiResponseData, ResponseKeyType } from '@/api/defaultApi';

  interface Props<T = ApiResponseData> {
    item: T;
  }

  const props = defineProps<Props>();

  const isImageUrl = (value: ResponseKeyType): boolean =>
    typeof value === 'string' && /\.(jpg|jpeg|png|webp|gif|bmp)$/i.test(value);

  const imageField = computed(() => {
    return Object.keys(props.item).find((key) => isImageUrl(props.item[key]));
  });

  const restFields = computed(() => {
    return Object.keys(props.item)
      .filter((key) => key !== 'id' && key !== imageField.value);
  });
</script>