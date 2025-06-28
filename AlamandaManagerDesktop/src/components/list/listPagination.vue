<style lang="scss" scoped>
  @use '@/assets/variables.scss' as *;

  .pagination {
    position: relative;
    width: 100%;
    display: flex;
    justify-content: center;
  }

  .pagination-buttons {
    display: flex;
    align-items: center;
  }

  .current-page {
    background-color: $primary;
    color: $white;
    border: 0;
    padding: 5px 15px;
    text-align: center;
    border-radius: 5px;
    display: flex;
    flex-wrap: wrap;
    cursor: pointer;
    margin-left: 4px;
    margin-right: 2px;

    &:hover {
      outline: 2px solid $secondary;
    }
  }

  .triangle {
    width: 5px;
    height: 5px;
    border-right: 2px solid $white;
    border-bottom: 2px solid $white;
    margin-left: 8px;
    margin-top: 2px;

    &--down {
      transform: rotate(45deg);
    }

    &--up {
      transform: rotate(-135deg);
      margin-top: 4px;
    }

    &--left {
      margin-top: 0;
      border-color: $primary;
      transform: rotate(135deg);
    }

    &--right {
      margin-top: 0;
      border-color: $primary;
      transform: rotate(-45deg);
    }
  }
  
  .page-option {
    background-color: $white;
    color: $secondary;
    border: 0;
    width: 100%;
    cursor: pointer;

    &:hover {
      background-color: $secondary;
      color: $white;
    }
  }

  .page-select-list {
    box-shadow: -2px 3px 4px rgba($secondary, 0.3);
    display: flex;
    flex-direction: column-reverse;
    background-color: $white;
    width: 60px;
    position: absolute;
    bottom: 26px;
    max-height: 80px;
    overflow: auto;
    border-radius: 5px;
  }

  .arrow-button {
    background-color: transparent;
    border: 0;
    cursor: pointer;
    width: 34px;

    &--left {
      :first-child {
        margin-right: -8px;
      }
    }

    &--right {
      :first-child {
        margin-right: -8px;
      }
    }
  }
</style>

<template>
  <div v-if="totalPages && totalPages > 1" class="pagination">
    <div class="pagination-buttons">
      <button 
        @click="() => onChangePage(1)" 
        class="arrow-button arrow-button--left flex">
        <div class="triangle triangle--left"></div>
        <div class="triangle triangle--left"></div>
      </button>
      <button 
        @click="() => onChangePage(currentPage, -1)" 
        class="arrow-button">
        <div class="triangle triangle--left"></div>
      </button>
      <button class="current-page" @click="toggleMenu">
        {{ currentPage }} 
        <div :class="`triangle triangle--${isMenuOpen ? 'up' : 'down'}`"></div>
      </button>
      <button 
        @click="() => onChangePage(currentPage, 1)" 
        class="arrow-button">
        <div class="triangle triangle--right"></div>
      </button>
      <button 
        @click="() => onChangePage(totalPages)" 
        class="arrow-button arrow-button--right flex">
        <div class="triangle triangle--right"></div>
        <div class="triangle triangle--right"></div>
      </button>
    </div>
    <div class="page-select-list" v-on:mouseleave="toggleMenu" v-if="isMenuOpen">
      <button
        v-for="page in totalPages"
        :key="page"
        class="page-option"
        :value="page"
        :selected="page === currentPage"
        @click="() => onChangePage(page)"
      >
        {{ page }}
      </button>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { ref } from 'vue';

  interface Props {
    totalPages?: number;
    currentPage?: number;
    onChangePage: (page: number) => void;
  }
  const props = defineProps<Props>();
  const isMenuOpen = ref(false);

  const onChangePage = (page?: number, modifier?: number) => {
    isMenuOpen.value = false;
    if(page && props.totalPages) {
      const newPage = page + (modifier || 0);
      if(newPage <= props.totalPages && !!newPage) {
        props.onChangePage(newPage);
      }
    }
  }

  const toggleMenu = () => {
    isMenuOpen.value = !isMenuOpen.value
  }
  
</script>
