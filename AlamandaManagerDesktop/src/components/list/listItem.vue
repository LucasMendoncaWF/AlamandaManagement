<style lang="scss" scoped>
  @use '@/assets/variables.scss' as *;
  td {
    padding: 10px;
    
    > div {
      min-width: 100px;
      max-width: 200px;
      max-height: 50px;
      overflow-y: auto;
      overflow-x: hidden;
    }

    &.image-td {
      width: 100px;
      display: flex;
      justify-content: center;
      align-items: center;
    }

    &.button-td {
      position: relative;
      padding: 0;
      width: 20px;
      padding-left: 10px;

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
        width: 22px;
      }
    }
  }
</style>

<template>
  
  <td class="button-td"><button @click="onClickEdit">
    <img :src="editIcon" alt="edit" />
  </button></td>

  <td class="image-td">
    <img 
      v-if="imageField && item[imageField]"
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
      {{ getValue(value) }}
    </div>
  </td>
</template>

<script lang="ts" setup>
  import { computed } from 'vue';
  const editIcon = new URL('@/assets/icons/icon_edit.svg', import.meta.url).href;
  import { ApiResponseData, ResponseKeyType } from '@/api/defaultApi';
  import { FormFieldOptionModel } from '@/models/formFieldModel';

  interface Props<T = ApiResponseData> {
    item: T;
    onClickEdit: () => void;
  }

  const props = defineProps<Props>();

  const isImageUrl = (value: ResponseKeyType): boolean =>
    typeof value === 'string' && /\.(jpg|jpeg|png|webp|gif|bmp)$/i.test(value);

  const getImage = (value: ResponseKeyType): string =>
    typeof value === 'string' ? value : '';

  const imageField = computed(() => {
    return Object.keys(props.item).find((key) => isImageUrl(props.item[key]) || key === "picture");
  });

  function getValue(value: any): string {
  if (Array.isArray(value)) {
    return value.map((item: FormFieldOptionModel) => item.name).join(', ');
  } 
  else if (value && typeof value === 'object') {
    return value.name ?? '';
  }
  return String(value ?? '');
}

  const restFields = computed(() => {
    return Object.entries(props.item)
      .filter(([key]) => !key.toLowerCase().includes('id') && key !== imageField.value)
      .reduce((acc, [key, value]) => {
        acc[key] = value;
        return acc;
      }, {} as Record<string, ResponseKeyType>);
  });
</script>