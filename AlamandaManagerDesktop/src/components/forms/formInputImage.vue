<style lang="scss" scoped>
  @use '@/assets/variables.scss' as *;
  .form-upload-input {
    color: $white;
    margin: 10px 0;

    input {
      width: 0;
      height: 0;
      opacity: 0;
    }

    button {
      background-color: $white;
      border: 2px solid $black;
      display: flex;
      justify-content: space-between;
      align-items: center;
      transform: translateY(21.5px);
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
</style>

<template>
  <div class="form-upload-input">
    <button @click.prevent="onClickUpload" role="button">
      {{fileName || 'Upload de arquivo'}}
      <img :src="uploadIcon" alt="upload" />
    </button>
    <input
      ref="fileInput"
      v-bind="attrs"
      :id="props.id"
      type="file"
      @input="onInput"
    />
  </div>
</template>

<script lang="ts" setup>
  import { ref, useAttrs } from 'vue';
  const uploadIcon = new URL('@/assets/icons/icon_upload.svg', import.meta.url).href;
  interface Props {
    modelValue: File | null;
    label: string;
    id: string;
  }
  
  const fileInput = ref<HTMLInputElement | null>(null);
  const fileName = ref('');
  const props = defineProps<Props>();
  const attrs = useAttrs();

  const emit = defineEmits<{
    (e: 'update:modelValue', value: File | null): void
  }>();

  const onClickUpload = () => {
    fileInput.value?.click();
  }

  const onInput = (event: Event) => {
    const target = event.target as HTMLInputElement;
    const file = target.files?.[0] || null;
    fileName.value = file?.name || '';
    emit('update:modelValue', file);
  }
</script>