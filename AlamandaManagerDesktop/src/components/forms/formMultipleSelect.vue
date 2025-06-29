<style lang="scss" scoped>
@use '@/assets/variables.scss' as *;

.form-input {
  color: $white;
  margin: 10px 0;

  .options-box {
    background-color: $white;
    border-radius: 5px;
    padding: 10px;
    margin-top: 5px;
    max-height: 150px;
    overflow-y: auto;
    border: 2px solid transparent;
  }

  &--inverted {
    color: $secondary;

    .options-box {
      border-color: $secondary;
    }
  }

  label.checkbox-label {
    display: flex;
    align-items: center;
    margin-bottom: 5px;
    font-size: 15px;

    input {
      margin-right: 10px;
      cursor: pointer;
    }
  }
}
</style>

<template>
  <div :class="`form-input form-input--${variant}`">
    <label>{{ label }}</label>
    <div class="options-box">
      <label
        v-for="option in options"
        :key="option.id"
        class="checkbox-label"
      >
        <input
          type="checkbox"
          :value="option.id"
          :checked="modelValue?.includes(option.id)"
          @change="onCheck(option.id, $event)"
        />
        {{ option.name }}
      </label>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { FormFieldOptionModel } from '@/models/formFieldModel';

interface Props {
  modelValue?: string[];
  label: string;
  id: string;
  options?: FormFieldOptionModel[];
  variant?: 'inverted';
}

const props = defineProps<Props>();
const emit = defineEmits<{
  (e: 'update:modelValue', value: string[]): void;
}>();

const onCheck = (id: string, event: Event) => {
  const checked = (event.target as HTMLInputElement).checked;
  const current = props.modelValue ? [...props.modelValue] : [];

  if (checked && !current.includes(id)) {
    emit('update:modelValue', [...current, id]);
  } else if (!checked && current.includes(id)) {
    emit('update:modelValue', current.filter((v) => v !== id));
  }
};
</script>
