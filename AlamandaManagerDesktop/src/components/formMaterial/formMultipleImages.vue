<style lang="scss" scoped>
  @use '@/assets/variables.scss' as *;

  .form-upload-input {
    margin-top: 15px;
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
      border: 2px solid $secondary;
      border-radius: 5px;
      padding: 8px;
      background-position: center;
      background-size: contain;
      background-repeat: no-repeat;
      width: calc(25% - 28px);
      max-width: 200px;
      height: 15vw;
      max-height: 200px;
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
      display: flex;
      justify-content: start;
      margin-top: 10px;
      width: 100%;
      button {
        width: calc(20% + 20px);
        background-color: $white;
        border: 2px solid $black;
        display: flex;
        justify-content: space-between;
        align-items: center;
        padding: 9px 15px;
        border-radius: 5px;
        cursor: pointer;
        transition: 0.3ms;

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
    <label>
      {{ label }} 
      ({{ (internalFiles?.length || 0) + '/' + (maxNumberOfFiles || defaultMaxNumberOfFiles) }})
    </label>

    <div class="images-wrapper" v-if="internalFiles?.length">
      <DragAndDrop v-model="internalFiles" v-if="internalFiles?.length">
        <div
          :key="getBackgroundImageUrl(src)"
          v-for="(src, index) in internalFiles"
          class="previous-image"
          :style="{ backgroundImage: getBackgroundImageUrl(src) }"
        > 
          <button type="button" @click="() => onClickUpload(index)" class="edit-button"><img :src="editIcon" alt="edit button" /></button>
          <button type="button" class="delete-button" @click="removeImage(index)">
            <img :src="deleteIcon" alt="delete" />
          </button>
        </div>
      </DragAndDrop>
    </div>

    <div class="add-file" v-if="!internalFiles || (internalFiles as string[])?.length < (maxNumberOfFiles || defaultMaxNumberOfFiles)">
      <button type="button" @click="() => onClickUpload()">
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
      accept=""
      @change="onInput"
      :id="id"
      v-bind="attrs"
    />
  </div>
  <ErrorMessage
    v-if="errorMessage"
    :message="errorMessage"
    :onClose="onCloseErrorMessage"
  />
</template>

<script lang="ts" setup>
import { ref, watch, useAttrs } from 'vue';
import ErrorMessage from '../stateHandling/errorMessage.vue';
import DragAndDrop from '../dragAndDrop/dragAndDrop.vue';

const editIcon = new URL('@/assets/icons/icon_edit.svg', import.meta.url).href;
const uploadIcon = new URL('@/assets/icons/icon_upload.svg', import.meta.url).href;
const deleteIcon = new URL('@/assets/icons/icon_delete.svg', import.meta.url).href;

interface Props {
  modelValue?: (string | File)[];
  label: string;
  id: string;
  variant?: 'inverted';
  disabled?: boolean;
  maxNumberOfFiles?: number;
  onRemoveImage?: (index: number) => void;
}
const props = defineProps<Props>();
const emit = defineEmits<{
  (e: 'update:modelValue', value: (File | string)[]): void;
}>();

const fileInput = ref<HTMLInputElement | null>(null);
const errorMessage = ref<string | null>(null);
const editingImgIndex = ref<number | null>(null);
const attrs = useAttrs();
const defaultMaxNumberOfFiles = 5;

const internalFiles = ref<(string | File)[]>(props.modelValue ?? []);

const isEqual = (a: any, b: any) => {
  return JSON.stringify(a) === JSON.stringify(b);
}

watch(() => props.modelValue, (val) => {
  if (val && !isEqual(internalFiles.value, val)) {
    internalFiles.value = [...val];
  }
});

watch(internalFiles, (val) => {
  if (!isEqual(props.modelValue, val)) {
    emit('update:modelValue', val);
  }
});

const onClickUpload = (index?: number) => {
  if (index !== undefined && index !== null) {
    editingImgIndex.value = index;
  }
  fileInput.value?.click();
};

const onCloseErrorMessage = () => {
  errorMessage.value = null;
}

const removeImage = (index: number) => {
  internalFiles.value.splice(index, 1);
  editingImgIndex.value = null;
  if (props.onRemoveImage) {
    props.onRemoveImage(index);
  }
};

const onInput = (event: Event) => {
  if (editingImgIndex.value !== null) {
    removeImage(editingImgIndex.value);
  }
  const input = event.target as HTMLInputElement;
  if (!input.files) {return};

  const max = props.maxNumberOfFiles ?? defaultMaxNumberOfFiles;
  if ((input.files.length + internalFiles.value.length) > max) {
    errorMessage.value = `You can only select up to ${max} files.`;
    input.value = '';
    return;
  }

  const newFiles = Array.from(input.files);
  internalFiles.value = internalFiles.value.concat(newFiles);
  input.value = '';
};

const getBackgroundImageUrl = (value: string | File): string => {
  if (typeof value === 'string') {
    return `url(${value})`;
  } else if (value instanceof File) {
    return `url(${URL.createObjectURL(value)})`;
  }
  return '';
}
</script>
