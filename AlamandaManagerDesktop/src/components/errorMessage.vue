<style lang="scss" scoped>
  @use '@/assets/variables.scss' as *;
  .error-message {
    position: fixed;
    z-index: 100;
    background-color: $primary;
    color: $white;
    padding: 20px;
    border-radius: 10px;
    width: 380px;
    left: calc(50% - 190px);
    top: 5px;
    animation: appearDown 1s;
    box-shadow: -4px 5px 1px 1px $black;
    padding-right: 45px;

    button {
      background-color: transparent;
      border: 0;
      position: absolute;
      top: 10px;
      right: 15px;
      cursor: pointer;

      img {
        width: 15px;
        filter: brightness(10);
      }
    }

    &--white {
      background-color: $white;
      color: $secondary;

      button img {
        filter: brightness(0);
      }
    }
  }

  @keyframes appearDown {
    0% {
      transform: translateY(-100px);
    }

    100% {
      transform: translateY(0);
    }
  }
  
</style>

<template>
  <div :class="`error-message error-message--${variant}`">
    {{message}} 
    <button @click="onClose"><img alt="close" :src="closeIcon" /></button>
  </div>
</template>

<script lang="ts" setup>
  const closeIcon = new URL('@/assets/icons/icon_close.svg', import.meta.url).href;
  interface Props {
    message: string;
    onClose: () => void;
    variant?: 'white';
  }
  const props = defineProps<Props>();
</script>