<style lang="scss" scoped>
  @use '@/assets/variables.scss' as *;
  td {
    padding: 10px;
    
    > div {
      min-width: 100px;
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
  <td class="button-td" v-if="onClickItem">
    <button @click="onClickCurrentItem">
      <img :src="chaptersIcon" alt="edit" />
    </button>
  </td>
  <td class="button-td">
    <button @click="onClickEdit">
      <img :src="editIcon" alt="edit" />
    </button>
  </td>

  <td class="image-td" v-if="imageField">
    <img 
      v-if="imageField && item[imageField]"
      :src="imageField ? getImage(imageUrl) : ''"
      alt="imagem"
      @error="onImageError"
      style="max-width: 100px; max-height: 80px;"
    />
  </td>

  <td
    v-for="(value, key) in restFields"
    :key="key"
    :title="typeof value === 'string' ? value : key"
  >
    <div>
      {{ getValue(value as (ViewObject | ViewObject[])) }}
    </div>
  </td>
</template>

<script lang="ts" setup>
  import { computed, ref, watch } from 'vue';
  const editIcon = new URL('@/assets/icons/icon_edit.svg', import.meta.url).href;
  const chaptersIcon = new URL('@/assets/icons/icon_chapters.svg', import.meta.url).href;
  const placeholder = new URL('@/assets/images/placeholder.webp', import.meta.url).href;
  import { ApiResponseData, ResponseKeyType } from '@/api/defaultApi';
  import { FormFieldOptionModel } from '@/models/formFieldModel';

  interface ViewObject {
    name: string;
    id: string;
  }

  interface Props<T = ApiResponseData> {
    item: T;
    onClickEdit: () => void;
    onClickItem?: (id: number) => void;
  }

  const props = defineProps<Props>();
  const imageUrl = ref(props.item.picture || '');

  const onClickCurrentItem = () => {
    console.log(props.item)
    if(props.onClickItem && props.item?.id) {
      props.onClickItem(props.item.id);
    }
  }

  const onImageError = () => {
    imageUrl.value = placeholder;
  }

  const isImageUrl = (value: ResponseKeyType): boolean =>
    typeof value === 'string' && /\.(jpg|jpeg|png|webp|gif|bmp)$/i.test(value);

  const getImage = (value: ResponseKeyType): string =>
    typeof value === 'string' ? value : '';

  const imageField = computed(() => {
    return Object.keys(props.item).find((key) => isImageUrl(props.item[key]) || key === "picture");
  });

  function getValue(value: ViewObject | ViewObject[]): string {
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

  watch(() => props.item, (newData) => {
    imageUrl.value = newData.picture as string;
  });
</script>