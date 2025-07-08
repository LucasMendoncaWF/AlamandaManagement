<style lang="scss" scoped>
  @use '@/assets/variables.scss' as *;
  .form-input {
    color: $white;
    margin-top: 10px;
    text-transform: capitalize;

    textarea {
      background-color: $white;
      padding: 10px;
      margin-top: 5px;
      font-size: 16px;
      width: calc(100% - 24px);
      border: 0;
      border-radius: 5px;
      resize: vertical;

      &:focus {
        outline: 2px solid $secondary;
      }
    }

    &--inverted {
      color: $secondary;

      textarea {
        border: 2px solid $secondary;
      }
    }
  }
</style>

<template>
  <div :class="`form-input form-input--${variant}`">
    <label :for="id">{{ convertFieldNameToLabel(label) }}</label>
    <textarea
      :id="id"
      v-bind="attrs"
      @input="onInput"
      :value="modelValue?.toString()"
      :disabled="disabled"
      rows="4"
    ></textarea>
  </div>
</template>

<script lang="ts" setup>
import { FieldType } from '@/models/formFieldModel';
import { convertFieldNameToLabel } from '@/utis/converter';
import { useAttrs } from 'vue';

  interface Props {
    modelValue?: FieldType;
    label: string;
    id: string;
    variant?: 'inverted';
    disabled?: boolean;
  }
 
defineProps<Props>();
const attrs = useAttrs();

const emit = defineEmits<{
    (e: 'update:modelValue', value: string): void
  }>();

const onInput = (event: Event) => {
  const target = event.target as HTMLInputElement;
  emit('update:modelValue', target.value);
}
</script>