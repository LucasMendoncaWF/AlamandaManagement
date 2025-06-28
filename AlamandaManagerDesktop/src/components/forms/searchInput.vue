<style lang="scss" scoped>
  @use '@/assets/variables.scss' as *;
  .search-input {
    color: $white;
    width: 220px;
    position: relative;

    input {
      background-color: $white;
      padding: 8px 15px;
      padding-right: 35px;
      font-size: 14px;
      width: calc(100% - 35px - 15px);
      border: 0;
      border-radius: 5px;

      &:focus {
        outline: 3px solid $secondary;
      }
    }

    .search-icon {
      position: absolute;
      right: 8px;
      top: 6px;

      width: 20px;
    }
  }
</style>

<template>
  <div class="search-input">
    <img class="search-icon" :src="searchIcon" alt="search" />
    <input
      :id="`search-${id}`"
      @input="onInput"
      :value="modelValue"
    />
  </div>
</template>

<script lang="ts" setup>
  const searchIcon = new URL('@/assets/icons/icon_search.svg', import.meta.url).href;
  interface Props {
    modelValue: string;
    id: string;
  }
 
  const props = defineProps<Props>();

  const emit = defineEmits<{
    (e: 'update:modelValue', value: string): void
  }>();

  const onInput = (event: Event) => {
    const target = event.target as HTMLInputElement;
    emit('update:modelValue', target.value);
  }
</script>