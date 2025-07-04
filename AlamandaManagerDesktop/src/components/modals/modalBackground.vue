<style lang="scss" scoped>
@use '@/assets/variables.scss' as *;
.modal-background {
  position: absolute;
  width: 100vw;
  height: 100vh;
  background-color: rgba($white, 0.05);
  backdrop-filter: blur(5px);
  left: 0;
  top: 0;
  z-index: 9;
}
</style>

<template>
  <div class="modal-background">
    <slot />
  </div>
</template>

<script lang="ts" setup>

</script>
