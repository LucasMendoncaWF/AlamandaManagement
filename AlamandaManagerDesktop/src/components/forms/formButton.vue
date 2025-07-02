<style lang="scss" scoped>
  @use '@/assets/variables.scss' as *;
  .form-button {
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

    &--danger {
      background-color: $primary;
    }

    &--inverted {
      background-color: $white;
      outline: 2px solid $secondary;
      outline-offset: -2px;
      color: $black;

      &:not(:disabled) {
      &:hover {
        background-color: $white;
        color: $primary;
      }

      &:active {
        transform: scale(0.8);
      }
    }
    }
  }
</style>

<template>
  <button type="button" :disabled="disabled" :class="`form-button form-button--${variant}`" v-bind="attrs" @click="onClick"><slot/></button>
</template>

<script lang="ts" setup>
  import { useAttrs } from 'vue';
  interface Props {
    onClick?: () => void;
    variant?: 'inverted' | 'danger';
    disabled?: boolean;
  }

  defineProps<Props>();
  const attrs = useAttrs();
</script>