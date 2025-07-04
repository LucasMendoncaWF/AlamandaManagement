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

    .button-loading {
      width: 12px;
      height: 12px;
      border: 2px solid #FFF;
      border-bottom-color: transparent;
      border-radius: 50%;
      display: inline-block;
      box-sizing: border-box;
      animation: rotation 1s linear infinite;
      margin-right: 5px;
    }

    @keyframes rotation {
      0% {
          transform: rotate(0deg);
      }
      100% {
          transform: rotate(360deg);
      }
    } 
  }
</style>

<template>
  <button type="button" :disabled="disabled || isLoading" :class="`form-button form-button--${variant}`" v-bind="attrs" @click="onClick">
    <span v-if="isLoading" class="button-loading"></span>
    <slot/>
  </button>
</template>

<script lang="ts" setup>
  import { useAttrs } from 'vue';
  interface Props {
    onClick?: () => void;
    variant?: 'inverted' | 'danger';
    disabled?: boolean;
    isLoading?: boolean;
  }

  defineProps<Props>();
  const attrs = useAttrs();
</script>