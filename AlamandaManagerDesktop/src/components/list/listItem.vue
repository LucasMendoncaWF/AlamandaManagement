<style lang="scss" scoped>
  td {
    padding: 4px 10px;
    
    > div {
      min-width: 100px;
      max-width: 200px;
      max-height: 50px;
      overflow-y: auto;
      overflow-x: hidden;
    }

    &.image-td {
      width: 100px;
    }

    &.button-td {
      padding: 0;
      width: 20px;

      &:first-of-type {
        padding-left: 10px;
      }

      button {
        padding: 0;
        margin: 0;
        background-color: transparent;
        border: 0;
        transform: translateY(3px);
        filter: brightness(3);
        cursor: pointer;

        &:hover {
          transform: translateY(3px) scale(1.1);
          filter: brightness(0);
        }
      }
      
      img {
        width: 18px;
      }
    }
  }
</style>

<template>
  
  <td class="button-td"><button @click="onClickEdit">
    <img :src="editIcon" alt="edit" />
  </button></td>
  <td class="button-td"><button @click="onClickDelete">
    <img :src="deleteIcon" alt="delete" />
  </button></td>

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
    :title="typeof value === 'string' ? value : key"
  >
    <div>
      {{ Array.isArray(value) ? value.map(item => (item as FormFieldOptionModel).name).join(', ') : value }}
    </div>
  </td>
</template>

<script lang="ts" setup>
  import { computed } from 'vue';
  const editIcon = new URL('@/assets/icons/icon_close.svg', import.meta.url).href;
  const deleteIcon = new URL('@/assets/icons/icon_close.svg', import.meta.url).href;
  import { ApiResponseData, ResponseKeyType } from '@/api/defaultApi';
  import { FormFieldOptionModel } from '@/models/formFieldModel';

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