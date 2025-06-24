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
  <input type="submit" class="form-submit" v-bind="attrs" @click="props.onClick"/>
</template>

<script lang="ts" setup>
  import { useAttrs } from 'vue';
  interface Props {
    onClick?: () => void;
  }

  const props = defineProps<Props>();
  const attrs = useAttrs();
</script>