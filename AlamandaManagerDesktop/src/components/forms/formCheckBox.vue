<style lang="scss" scoped>
  @use '@/assets/variables.scss' as *;
  .form-input {
    color: $white;
    margin-top: 10px;
    text-transform: capitalize;

    label {
      margin-bottom: 8px;
      display: block;
    }

    &--inverted {
      color: $secondary;
    }

    .checkbox-wrapper {
      all: unset;
      background-color: $primary;
      width: 60px;
      height: 30px;
      border-radius: 20px;
      display: flex;
      align-items: center;
      margin-bottom: 6px;
      cursor: pointer;
      transition: 0.4s;
      box-shadow: inset 1px 1px 14px 0px rgba($secondary, 0.4);

      .toggle {
        width: 24px;
        height: 24px;
        background-color: $white;
        border-radius: 50%;
        transition: 0.4s;
      }

      &--on {
        .toggle {
          transform: translateX(3px);
        }
      }
      &--off {
        background-color: $lightGray;
        .toggle {
          transform: translateX(33px);
        }
      }
    }

    &.input-checkbox {
      input {
        width: 0;
        height: 0;
      }
    }
  }
</style>

<template>
  <div :class="`form-input form-input--${variant} input-checkbox`">
    <label>{{ convertFieldNameToLabel(label) }}</label>
    <button @click="onChange" type="button" :class="`checkbox-wrapper checkbox-wrapper--${!!modelValue ? 'on' : 'off'}`">
      <div class="toggle"></div>
    </button>
  </div>
</template>

<script lang="ts" setup>
  import { FieldType } from '@/models/formFieldModel';
  import { convertFieldNameToLabel } from '@/utis/converter';

  interface Props {
    modelValue?: FieldType;
    label: string;
    id: string;
    variant?: 'inverted';
    disabled?: boolean;
  }

  const props = defineProps<Props>();
  const emit = defineEmits<{
    (e: 'update:modelValue', value: boolean): void;
  }>();

  const onChange = () => {
    emit('update:modelValue', !props.modelValue);
  };
</script>