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
      max-width: 150px;
      max-height: 150px;
      border: 2px solid $secondary;
      border-radius: 5px;
      margin: 5px;
      padding: 8px;

      img.preview {
        width: 100%;
        height: auto;
        border-radius: 3px;
        object-fit: contain;
      }
    }

    .edit-button, .delete-button {
      position: absolute;
      width: 24px;
      background-color: $white;
      padding: 3px 5px;
      border-radius: 50%;
      border: 2px solid $secondary;
      bottom: 5px;
      cursor: pointer;
      img {
        width: 100%;
        transform: translateY(1px);
      }
    }

    .delete-button {
      left: 5px;
    }

    .edit-button {
      right: 5px;
    }

    .images-wrapper {
      display: flex;
      flex-wrap: wrap;
      gap: 10px;
      margin-top: 5px;
    }

    .add-file {
      margin-top: 10px;
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

    <div class="images-wrapper" v-if="previewFiles.length">
      <div
        class="previous-image"
        v-for="(src, index) in previewFiles"
        :key="index"
      >
        <img
          class="preview"
          :src="src || placeholder"
          :alt="`${label} preview ${index + 1}`"
          @error="onImageError(index)"
        />
        <button type="button" class="delete-button" @click="removeImage(index)">
          <img :src="deleteIcon" alt="delete" />
        </button>
      </div>
    </div>

    <div class="add-file">
      <button type="button" @click="onClickUpload">
        Upload images
        <img :src="uploadIcon" alt="upload" />
      </button>
    </div>

    <input
      ref="fileInput"
      class="hidden"
      type="file"
      multiple
      :disabled="disabled"
      @change="onInput"
      :id="id"
      v-bind="attrs"
    />
  </div>
</template>

<script lang="ts" setup>
import { ref, watch, useAttrs } from 'vue';

const placeholder = new URL('@/assets/images/placeholder.webp', import.meta.url).href;
const uploadIcon = new URL('@/assets/icons/icon_upload.svg', import.meta.url).href;
const deleteIcon = new URL('@/assets/icons/icon_delete.svg', import.meta.url).href;

interface Props {
  modelValue?: (File | string)[];
  label: string;
  id: string;
  variant?: 'inverted';
  disabled?: boolean;
  onRemoveImage?: (index: number) => void;
}
const props = defineProps<Props>();
const emit = defineEmits<{
  (e: 'update:modelValue', value: (File | string)[]): void;
}>();

const fileInput = ref<HTMLInputElement | null>(null);
const attrs = useAttrs();

const previewFiles = ref<(string | null)[]>([]);

watch(
  () => props.modelValue,
  (newVal) => {
    if (!newVal) {
      previewFiles.value = [];
      return;
    }
    previewFiles.value = newVal.map((item) => {
      if (typeof item === 'string') {return item;}
      else if (item instanceof File) {return URL.createObjectURL(item);}
      else {return null;}
    });
  },
  { immediate: true }
);

const onClickUpload = () => {
  fileInput.value?.click();
};

const removeImage = (index: number) => {
  const arr = props.modelValue ? [...props.modelValue] : [];
  arr.splice(index, 1);
  emit('update:modelValue', arr);
  if (props.onRemoveImage) {props.onRemoveImage(index);}
};

const onImageError = (index: number) => {
  previewFiles.value[index] = placeholder;
};

const onInput = (event: Event) => {
  const input = event.target as HTMLInputElement;
  if (!input.files) {return;}

  const newFiles = Array.from(input.files);

  const current = props.modelValue ? [...props.modelValue] : [];
  const updated = current.concat(newFiles);

  emit('update:modelValue', updated);
  input.value = '';
};
</script>
