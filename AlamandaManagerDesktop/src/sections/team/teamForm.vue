<style lang="scss" scoped>
  .team-form {
    padding: 20px;
    align-items: end;
    justify-content: space-between;

    .team-input {
      width: calc(25% - 10px);
    }

    .team-submit {
      margin-bottom: 10px;
      width: calc(25% - 10px);
    }
  }
</style>

<template>
  <form @submit.prevent="onSubmit" class="flex team-form">
    <div class="team-input">
      <FormInput
        id="name"
        maxlength="100"
        label="Name"
        v-model="form.name"
        type="text"
        variant="inverted"
        required
      />
    </div>
    <div class="team-input">
      <FormInput
        id="social"
        maxlength="50"
        label="Social"
        v-model="form.social"
        type="text"
        variant="inverted"
      />
    </div>

    <div class="team-input">
      <FormInputImage
        id="picture"
        label="picture"
        v-model="form.picture"
        variant="inverted"
        accept="image/*"
      />
    </div>

    <div class="team-submit">
      <FormSubmit :value="data?.id? 'Salvar' : 'Criar'" />
      <FormButton @click="onComplete" >Cancelar</FormButton>
    </div>
    <Loader v-if="isLoading"/>
    <ErrorMessage variant="white" :onClose="onCloseErrorMessage" v-if="!!errorMessage" :message="errorMessage" />
  </form>
</template>

<script lang="ts" setup>
  import { reactive, ref } from 'vue';
  import FormInput from '@/components/forms/formInput.vue';
  import FormSubmit from '@/components/forms/formSubmit.vue';
  import FormInputImage from '@/components/forms/formInputImage.vue';
  import ErrorMessage from '@/components/errorMessage.vue';
  import { addMember, TeamMemberForm } from '@/api/teamMembers';
  import Loader from '@/components/loader.vue';
  import { fileToBase64 } from '@/utis/converter';
  import { getErrorMessage, ApiResponseData } from '@/api/defaultApi';
  import FormButton from '@/components/forms/formButton.vue';
  interface Props<T = ApiResponseData> {
    data?: T;
    onComplete: () => void;
  };

  const props = defineProps<Props>();

  let timeout: ReturnType<typeof setTimeout>;
  const form = reactive<TeamMemberForm>({
    name: '',
    social: '',
    picture: null,
  });
  const isLoading = ref(false);
  const errorMessage = ref<string | null>('');

  const onCloseErrorMessage = () => {
    errorMessage.value = null;
  }

  const onSubmit = async () => {
    const file = form.picture;
    errorMessage.value = null;
    if (file) {
      const sizeInBytes = file.size;
      const sizeInMB = sizeInBytes / (1024 * 1024);
      if (sizeInMB > 0.2) {
        errorMessage.value= "O máximo permitido para a foto é 200KB";
      }
    }

    isLoading.value = true;
    if(!errorMessage.value) {
      if(timeout) {
        clearTimeout(timeout);
      }
      try {
        let submitFile = null;
        if(file) {
          submitFile = await fileToBase64(file);
        }
        await addMember({...form, picture: submitFile});
        props.onComplete();
      } catch (error) {
        errorMessage.value = getErrorMessage(error);
        isLoading.value = false;
        timeout = setTimeout(() => {
          errorMessage.value = null;
        }, 5000);
      } finally {
        isLoading.value = false;
      }
    }
  }
</script>