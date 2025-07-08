<style lang="scss" scoped>
@use '@/assets/variables.scss' as *;
.modal-body {
  padding: 20px;
  position: absolute;
  background-color: $white;
  z-index: 99;
  border: 2px solid $black;
  left: calc(10% + 10px);
  top: 5%;
  box-shadow: -7px 11px $black;
  animation: FormAppear 0.3s;
  max-height: calc(90% - 40px);
  overflow: auto;

  @keyframes FormAppear {
    0% {
      box-shadow: 0px 0px $black;
    }

    100% {
      box-shadow: -7px 11px $black;
    }
  }
}
</style>

<template>
  <div :style="{width, left: `calc(50% - ${width}/2)`}" class="modal-body">
    <slot />
  </div>
</template>

<script lang="ts" setup>
  interface Props {
    width?: string;
  }

withDefaults(defineProps<Props>(), {
  width: '80%'
});
</script>
