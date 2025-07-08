<style lang="scss" scoped>
  .buttons {
    display: flex;
    gap: 10px;
    margin-top: 20px;
    justify-content: end;
  }
</style>

<template>
  <ModalBackground>
    <ModalBody width="300px">
      <div>
        <slot />
      </div>
      <div class="buttons">
        <FormButton variant="inverted" :disabled="isLoading" @click="onCancel">Cancel</FormButton>
        <FormButton variant="danger":isLoading="isLoading" @click="() => onDelete(id)">Delete</FormButton>
      </div>
      <div>
        <ErrorMessage :onClose="onCloseError" v-if="hasError" message="An error occurred deleting the item." />
      </div>
    </ModalBody>
  </ModalBackground>
</template>

<script lang="ts" setup>
import { ref } from 'vue';
import ModalBody from '@/components/modals/modalBody.vue';
import ModalBackground from '@/components/modals/modalBackground.vue';
import FormButton from '@/components/formMaterial/formButton.vue';
import ErrorMessage from '@/components/stateHandling/errorMessage.vue';

  interface Props {
    onCancel: () => void;
    deleteFunction: (id: number) => void;
    refetch: () => void;
    id?: number;
  }
const props = defineProps<Props>();
const isLoading = ref(false);
const hasError = ref(false);

const onCloseError = () => {
  hasError.value = false;
}
  
const onDelete = async (id?: number) => {
  if(id) {
    isLoading.value = true;
    hasError.value = false;
    try {
      await props.deleteFunction(id);
    } catch {
      hasError.value = true;
      setTimeout(() => {
        hasError.value = false;
      }, 5000);
    } finally {
      await props.refetch();
      isLoading.value = false;
      props.onCancel();
    }
  } else {
    props.onCancel();
  }
}
</script>