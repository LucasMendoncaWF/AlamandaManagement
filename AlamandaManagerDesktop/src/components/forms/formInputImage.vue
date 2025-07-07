<style lang="scss" scoped>
  @use '@/assets/variables.scss' as *;
  .form-upload-input {
    label {
      color: $white;
      text-transform: capitalize;
    }

    &--inverted {
      label {
        color: $secondary;
      }
    }

    .previous-image {
      position: relative;
      max-width: 50%;
      max-height: 100%;
      min-width: 150px;
      border: 2px solid $secondary;
      border-radius: 5px;
      margin-top: 5px;
      padding: 12px;
      img.preview {
        width: 100%;
      }
    }

    .edit-button, .delete-button {
      position: absolute;
      width: 30px;
      background-color: $white;
      padding: 5px 6px;
      border-radius: 50%;
      border: 2px solid $secondary;
      right: 5px;
      bottom: 5px;
      cursor: pointer;
      img {
        width: 100%;
        transform: translateY(2px);
      }
    }

    .delete-button {
      right: unset;
      left: 5px;
    }

    .add-file {
      margin-top: 5px;
      margin-bottom: 5px;
      button {
        background-color: $white;
        border: 2px solid $black;
        display: flex;
        justify-content: space-between;
        align-items: center;
        padding: 9px 15px;
        border-radius: 5px;
        cursor: pointer;
        transition: 0.3ms;
        width: 100%;

        &:hover {
          background-color: $black;
          color: $white;

          img {
            filter: invert(0);
          }
        }
        &:focus {
          outline: 2px solid $secondary;
        }

        img {
          margin-left: 10px;
          width: 20px;
          filter: invert(1);
        }
      }
    }

    .hidden {
      height: 0;
      width: 0;
      position: absolute;
    }
  }
</style>

<template>
  <div :class="`form-upload-input form-upload-input--${variant}`">
    <label>{{ label }}</label>
    <div class="previous-image" v-if="previewFile">
      <img @error="onImageError" class="preview" :src="previewFile" :alt="label"/>
      <button type="button" @click="onClickUpload" class="edit-button"><img :src="editIcon" alt="edit button" /></button>
      <button type="button" @click="onDeleteImage" class="delete-button"><img :src="deleteIcon" alt="delete button" /></button>
    </div>
    <div class="add-file" v-if="!previewFile">
      <button @click="onClickUpload" type="button">
        {{fileName || 'Upload image'}}
        <img :src="uploadIcon" alt="upload" />
      </button>
    </div>
    <input
      ref="fileInput"
      v-bind="attrs"
      class="hidden"
      :id="id"
      type="file"
      @input="onInput"
      :disabled="disabled"
    />
  </div>
</template>

<script lang="ts" setup>
  import { FieldType } from '@/models/formFieldModel';
  import { ref, useAttrs } from 'vue';
  const placeholder = new URL('@/assets/images/placeholder.webp', import.meta.url).href;
  const uploadIcon = new URL('@/assets/icons/icon_upload.svg', import.meta.url).href;
  const editIcon = new URL('@/assets/icons/icon_edit.svg', import.meta.url).href;
  const deleteIcon = new URL('@/assets/icons/icon_delete.svg', import.meta.url).href;
  interface Props {
    modelValue?: FieldType;
    label: string;
    id: string;
    previousImage?: string | null;
    variant?: 'inverted';
    disabled?: boolean;
    onRemoveImage: (key: string) => void;
  }
  
  const props = defineProps<Props>();
  const fileInput = ref<HTMLInputElement | null>(null);
  const fileName = ref('');
  const attrs = useAttrs();
  const previewFile = ref<null | string | undefined>(props.previousImage);

  const emit = defineEmits<{
    (e: 'update:modelValue', value: File | null): void
  }>();

  const onClickUpload = () => {
    fileInput.value?.click();
  }

  const onDeleteImage = () => {
    props.onRemoveImage(props.id);
    previewFile.value = null;
    fileInput.value = null;
  }

  const onImageError = () => {
    previewFile.value = placeholder;
  }

  const onInput = (event: Event) => {
    const target = event.target as HTMLInputElement;
    const file = target.files?.[0] || null;
    fileName.value = file?.name || '';
    if (file) {
      const reader = new FileReader();
      reader.onload = () => {
        previewFile.value = reader.result as string;
      };
      reader.readAsDataURL(file);
    } else {
      previewFile.value = '';
    }
    emit('update:modelValue', file);
  }
</script>