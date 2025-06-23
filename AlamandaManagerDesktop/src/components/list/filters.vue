<style lang="scss" scoped>
  @use '@/assets/variables.scss' as *;
  .filters {
    position: fixed;
    right: 0;
    top: 0;
    height: 100dvh;
    background-color: $secondary;
    padding: 20px;
    animation: slideFromRight 0.3s;
    box-shadow: 2px -2px 9px 3px $black;
    min-width: 200px
  }

  .filters-header {
    display: flex;
    flex-wrap: wrap;
    justify-content: space-between;
    align-items: center;

    h3 {
      margin: 0;
      color: $white;
      margin-right: 20px;
    }

    .close-button {
      background-color: transparent;
      border: 0;
      font-size: 18px;
      color: $white;
      cursor: pointer;

      &:hover {
        opacity: 0.6;
      }
    }
  }

  @keyframes slideFromRight {
    0% {
      transform: translateX(400px);
    }

    100% {
      transform: translateX(0);
    }
  }
</style>

<template>
  <div class="filters">
    <div  class="filters-header">
      <h3>Filters</h3>
      <button class="close-button" @click="props.onClickClose">x</button>
    </div>
  </div>
</template>

<script lang="ts" setup>
  interface Props {
    onClickClose: () => void;
  }

  const props = defineProps<Props>();
</script>