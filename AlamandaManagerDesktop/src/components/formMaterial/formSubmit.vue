<style lang="scss" scoped>
  @use '@/assets/variables.scss' as *;
  .form-submit {
    background-color: $secondary;
    color: $white;
    padding: 13.5px;
    border: 0;
    font-weight: bold;
    border-radius: 5px;
    cursor: pointer;
    transition: 0.4s;

    &:not(:disabled) {
      &:hover {
        background-color: $black;
        color: $primary;
      }

      &:active {
        transform: scale(0.8);
      }
    }
 

    &:disabled {
      cursor: not-allowed;
      opacity: 0.6;
    }
  }
</style>

<template>
  <input 
    type="submit" 
    :disabled="disabled" 
    :value="value" 
    class="form-submit" 
    v-bind="attrs"
  />
</template>

<script lang="ts" setup>
  interface Props {
    disabled?: boolean;
    value: string;
  }
defineProps<Props>();
import { useAttrs } from 'vue';
const attrs = useAttrs();
</script>