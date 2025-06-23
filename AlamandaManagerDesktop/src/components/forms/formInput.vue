<style lang="scss" scoped>
  @use '@/assets/variables.scss' as *;
  .form-input {
    color: $white;
    margin: 10px 0;

    input {
      background-color: $white;
      padding: 10px;
      margin-top: 5px;
      font-size: 16px;
      width: calc(100% - 20px);
      border: 0;
      border-radius: 5px;

      &:focus {
        outline: 3px solid $secondary;
      }
    }
  }
</style>

<template>
  <div class="form-input">
    <label :for="props.id">{{ props.label }}</label>
    <input
      :id="props.id"
      v-bind="attrs"
      @input="onInput"
      :value="props.modelValue"
    />
  </div>
</template>

<script lang="ts" setup>
  import { useAttrs } from 'vue';

  interface Props {
    modelValue: string;
    label: string;
    id: string;
  }
 
  const props = defineProps<Props>();
  const attrs = useAttrs();

  const emit = defineEmits<{
    (e: 'update:modelValue', value: string): void
  }>();

  function onInput(event: Event) {
    const target = event.target as HTMLInputElement;
    emit('update:modelValue', target.value);
  }
</script>