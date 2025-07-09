<style lang="scss" scoped>
  @use '@/assets/variables.scss' as *;
  .form-input {
    color: $white;
    margin-top: 10px;
    text-transform: capitalize;
    position: relative;

    .date-view {
      all: unset;
      position: absolute;
      top: calc(50% - 6px);
      font-size: 14px;
      padding: 10px 20px 10px 5px;
      left: 10px;
      background-color: $white;
    }

    input {
      background-color: $white;
      padding: 10px;
      margin-top: 5px;
      font-size: 16px;
      width: calc(100% - 24px);
      border: 0;
      border-radius: 5px;

      &:focus {
        outline: 2px solid $secondary;
      }
    }

    &--inverted {
      color: $secondary;

      input {
        border: 2px solid $secondary;
      }
    }
  }
</style>

<template>
  <div :class="`form-input form-input--${variant}`">
    <label :for="id">{{ convertFieldNameToLabel(label) }}</label>
    <button
      @click="focusInput"
      type="button"
      v-if="attrs.type === 'date'" 
      class="date-view"
    >
      {{ modelValue ? formatDateWithoutTime(modelValue, 1) : 'Select a date' }}
    </button>
    <input
      @click="focusInput"
      v-bind="attrs"
      ref="inputRef"
      @input="onInput"
      :id="id"
      :value="modelValue?.toString()"
      :disabled="disabled"
    />
  </div>
</template>

<script lang="ts" setup>
import { FieldType } from '@/models/formFieldModel';
import { convertFieldNameToLabel, formatDateWithoutTime } from '@/utis/converter';
import { ref, useAttrs } from 'vue';

  interface Props {
    modelValue?: FieldType;
    label: string;
    id: string;
    variant?: 'inverted';
    disabled?: boolean;
  }
 
defineProps<Props>();
const attrs = useAttrs();
const inputRef = ref<HTMLInputElement | null>(null);

const emit = defineEmits<{
    (e: 'update:modelValue', value: string): void
  }>();

const onInput = (event: Event) => {
  const target = event.target as HTMLInputElement;
  emit('update:modelValue', target.value);
}

const focusInput = () => {
  if (!inputRef.value || attrs.type !== 'date') { return; }
  inputRef.value.focus();
  inputRef.value.showPicker();
} 
</script>