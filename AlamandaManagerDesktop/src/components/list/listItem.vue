<style lang="scss" scoped>
  td {
    padding: 4px 10px;

    &.image-td {
      width: 100px;
    }

    &.button-td {
      width: 50px;
    }
  }
</style>

<template>
  <td>{{ item.id }}</td>

  <td class="image-td">
    <img 
      :src="imageField ? getImage(item[imageField]) : ''"
      alt="imagem"
      style="max-width: 100px; max-height: 80px;"
    />
  </td>

  <td
    v-for="(value, key) in restFields"
    :key="key"
  >
    {{ Array.isArray(value) ? value.map(item => item.name).join(', ') : value }}
  </td>

  <td class="button-td"><button @click="onClickEdit">Edit</button></td>
  <td class="button-td"><button @click="onClickDelete">Delete</button></td>
</template>

<script lang="ts" setup>
  import { computed } from 'vue';
  import { ApiResponseData, ResponseKeyType } from '@/api/defaultApi';

  interface Props<T = ApiResponseData> {
    item: T;
    onClickEdit: () => void;
    onClickDelete: () => void;
  }

  const props = defineProps<Props>();

  const isImageUrl = (value: ResponseKeyType): boolean =>
    typeof value === 'string' && /\.(jpg|jpeg|png|webp|gif|bmp)$/i.test(value);

  const getImage = (value: ResponseKeyType): string =>
    typeof value === 'string' ? value : '';

  const imageField = computed(() => {
    return Object.keys(props.item).find((key) => isImageUrl(props.item[key]) || key === "picture");
  });

  const restFields = computed(() => {
    return Object.entries(props.item)
      .filter(([key]) => key !== 'id' && key !== imageField.value)
      .reduce((acc, [key, value]) => {
        acc[key] = value;
        return acc;
      }, {} as Record<string, ResponseKeyType>);
  });
</script>