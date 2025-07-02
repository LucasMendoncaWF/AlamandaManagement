<style lang="scss" scoped>
  @use '@/assets/variables.scss' as *;
  .form-select {
    color: $white;
    margin-top: 10px;
    text-transform: capitalize;
    width: 100%;

    select {
      background-color: $white;
      padding: 10px;
      margin-top: 5px;
      font-size: 16px;
      border: 0;
      border-radius: 5px;
      width: 100%;

      &:focus {
        outline: 2px solid $secondary;
      }
    }

    &--inverted {
      color: $secondary;

      select {
        border: 2px solid $secondary;
      }
    }
  }
</style>

<template>
  <div :class="`form-select form-select--${variant}`">
    <label :for="id">{{ label.replace('id', '') }}</label>
    <select
      :id="id"
      v-bind="attrs"
      @input="onInput"
      :value="modelValue"
      :disabled="disabled"
    >
      <option v-for="option of options" :value="option.id">
        {{ option.name }}
      </option>
    </select>
  </div>
</template>

<script lang="ts" setup>
  import { FormFieldOptionModel } from '@/models/formFieldModel';
import { useAttrs } from 'vue';

  interface Props {
    modelValue?: string;
    label: string;
    id: string;
    variant?: 'inverted';
    options: FormFieldOptionModel[];
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