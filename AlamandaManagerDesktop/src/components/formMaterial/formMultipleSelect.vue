<style lang="scss" scoped>
@use '@/assets/variables.scss' as *;

.form-multi-input {
  color: $white;
  margin-top: 10px;
  text-transform: capitalize;
  position: relative;

  .input-box {
    display: flex;
    flex-wrap: wrap;
    gap: 5px;
    border: 2px solid $white;
    padding: 6.2px 10px;
    border-radius: 5px;
    background-color: $white;
    margin-top: 5px;
    min-height: calc(42px - 12.4px);

    input {
      border: none;
      outline: none;
      flex: 1;
      min-width: 100px;
      background: transparent;
      color: $secondary;
    }

    .pill {
      background-color: $secondary;
      color: $white;
      border-radius: 12px;
      padding: 5px 5px 5px 10px;
      display: flex;
      align-items: center;
      gap: 4px;
      font-size: 11px;

      button {
        transform: translateY(-1px);
        background: transparent;
        border: none;
        color: $white;
        font-weight: bold;
        cursor: pointer;
        line-height: 1;
      }
    }
  }

  .options-box {
    background-color: $white;
    border-radius: 5px;
    margin-top: 5px;
    max-height: 200px;
    overflow-y: auto;
    border: 2px solid transparent;
    position: absolute;
    width: calc(100% - 4px);
    top: calc(100% - 5px);
    z-index: 99;
  }

  &--inverted {
    color: $secondary;

    .input-box {
      border-color: $secondary;

      input {
        color: $secondary;
      }

      .pill {
        background-color: $primary;
      }
    }

    .options-box {
      border-color: $secondary;
    }
  }

  .option-item {
    width: 100%;
    text-align: left;
    background-color: transparent;
    border: 0;
    padding: 10px;
    cursor: pointer;

    &:hover {
      background-color: rgba($secondary, 0.1);
    }
  }
}
</style>

<template>
  <div v-on:mouseleave="onFocusOut" :class="`form-multi-input form-multi-input--${variant}`">
    <label>{{ convertFieldNameToLabel(label) }}</label>

    <div class="input-box" @click="focusInput">
      <span
        v-for="item in selectedOptions"
        :key="item.id"
        class="pill"
      >
        {{ item.name }}
        <button @click.stop="remove(item.id)">×</button>
      </span>
      <input
        ref="inputRef"
        type="text"
        v-model="search"
        placeholder="Digite para buscar..."
        @keydown.stop
        :disabled="disabled"
      />
    </div>

    <div class="options-box" v-if="filteredOptions.length && isOptionsOpen">
      <button
        :title="option.name"
        v-for="option in filteredOptions"
        :key="option.id"
        class="option-item"
        @click="select(option)"
        type="button"
      >
        {{ option.name }}
      </button>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { ref, computed, watch } from 'vue';
import { FieldType, FormFieldOptionModel } from '@/models/formFieldModel';
import { convertFieldNameToLabel } from '@/utis/converter';

interface Props {
  modelValue?: FieldType;
  label: string;
  id: string;
  options?: FormFieldOptionModel[];
  variant?: 'inverted';
  disabled?: boolean;
}

const props = defineProps<Props>();
const emit = defineEmits<{ (e: 'update:modelValue', value: string[]): void }>();

const search = ref('');
const isOptionsOpen = ref(false);
const inputRef = ref<HTMLInputElement | null>(null);

const selected = ref<string[]>(props.modelValue as string[] ?? []);

watch(
  () => props.modelValue,
  (newVal) => {
    if (newVal) {selected.value = newVal as string[];}
  },
  { immediate: true }
);

const selectedOptions = computed(() =>
  props.options?.filter((opt) => selected.value.includes(opt.id)) ?? []
);

const filteredOptions = computed(() =>
  props.options?.filter(
    (opt) =>
      !selected.value.includes(opt.id) &&
      opt.name?.toLowerCase().includes(search.value.toLowerCase())
  ) ?? []
);

const select = (option: FormFieldOptionModel) => {
  if (!selected.value.includes(option.id)) {
    selected.value.push(option.id);
    emit('update:modelValue', [...selected.value]);
  }
  search.value = '';
};

const remove = (id: string) => {
  selected.value = selected.value.filter((v) => v !== id);
  emit('update:modelValue', [...selected.value]);
};

const focusInput = () => {
  inputRef.value?.focus();
  isOptionsOpen.value = true;
};

const onFocusOut = () => {
  isOptionsOpen.value = false;
}
</script>