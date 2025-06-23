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
  <div class="error-message">{{props.message}}</div>
</template>

<script lang="ts" setup>
  interface Props {
    message: string;
  }
  const props = defineProps<Props>();
</script>