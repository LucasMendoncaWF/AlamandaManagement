<template>
  <div ref="sortableList" class="sortable-wrapper">
    <slot></slot>
  </div>
</template>

<script setup lang="ts">
import Sortable from 'sortablejs'
import { onMounted, ref, watch } from 'vue'

type DragItem = string | File | null

const props = defineProps<{
  modelValue: DragItem[]
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: DragItem[]): void
}>()

const sortableList = ref<HTMLElement>()
const internalList = ref<DragItem[]>([...props.modelValue])

watch(
  () => props.modelValue,
  (val) => {
    internalList.value = [...val]
  }
)

onMounted(() => {
  if (!sortableList.value) {return}

  Sortable.create(sortableList.value, {
    animation: 150,
    onEnd: (evt) => {
      const updated = [...internalList.value]
      const [moved] = updated.splice(evt.oldIndex!, 1)
      updated.splice(evt.newIndex!, 0, moved)
      internalList.value = updated
      emit('update:modelValue', updated)
    }
  })
})
</script>

<style lang="scss" scoped>
.sortable-wrapper {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
  padding: 0;
  margin: 0;
  list-style: none;
  width: 100%;

  > * {
    cursor: grab;
  }
}
</style>
