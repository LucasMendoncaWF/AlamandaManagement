<style lang="scss" scoped>
  @use '@/assets/variables.scss' as *;
  .login-page {
    width: 100vw;
    height: 100vh;
    background-color: $background;
    display: flex;
    justify-content: left;

    form {
      width: 50%;
      max-width: 400px;
      padding: 20px;
      z-index: 1;
      background-color: $primary;
      box-shadow: 6px 1px 4px 1px rgb(0 0 0 / 50%);
      display: flex;
      align-items: center;
      justify-content: center;
      
      .signin-button {
        margin-top: 15px;
      }
    }
  }
  .background-container {
    background-image: url('@/assets/images/background.png');
    background-size: cover;
    background-position: center;
    position: absolute;
    z-index: 0;
    width: 100vw;
    height: 100vh;
    overflow: hidden;
    filter: hue-rotate(0deg);
    animation: colorChange infinite 20s;

    img {
      width: 100%;
      min-height: 100%;
    }
  }

  @keyframes colorChange {
    0% { 
      filter: hue-rotate(0deg);
    }

    50% { 
      filter: hue-rotate(180deg);
    }

    100% { 
      filter: hue-rotate(360deg);
    }
  }

  .logo-image {
    width: 140px;
    filter: brightness(3);
    margin: 10px 0;
    margin-left: calc(50% - 70px);
  }
</style>

<template>
  <div class="background-container">
  </div>
  <div class="login-page">
    <ErrorMessage :onClose="onCloseErrorMessage" message="Verify your credential and try again" v-if="hasError" />
    <form @submit.prevent="onSubmit">
      <div>
        <img class="logo-image" :src="logo" alt="logo" />
        <FormInput 
          id="email"
          required
          label="E-mail"
          type="email"
          v-model="loginData.email"/>
        <FormInput 
          id="pass"
          required
          label="Password"
          type="password"
          v-model="loginData.password"/>
        <FormSubmit class="signin-button" :disabled="!loginData.email || !loginData.password || isLoading" value="Sign In" />
      </div>
    </form>
  </div>
  <Loader isGlobal v-if="isLoading" />
</template>

<script lang="ts" setup>
import { reactive, ref } from 'vue';
import { useRouter } from 'vue-router';
import FormInput from '@/components/formMaterial/formInput.vue';
import FormSubmit from '@/components/formMaterial/formSubmit.vue';
import ErrorMessage from '@/components/stateHandling/errorMessage.vue';
import { type LoginRequest, userLogIn, type LoginResponse } from '@/api/login';
import Loader from '@/components/stateHandling/loader.vue';
const logo = new URL('@/assets/images/logo.png', import.meta.url).href;
let timeout: ReturnType<typeof setTimeout>;
const router = useRouter();
const hasError = ref<boolean>(false);
const isLoading = ref<boolean>(false);
const loginData = reactive<LoginRequest>({
  email: '',
  password: ''
});

const onSubmit = async () => {
  if(timeout) {
    clearTimeout(timeout);
  }
  isLoading.value = true;
  try {
    const tokens: LoginResponse = await userLogIn(loginData);
    localStorage.setItem('refreshToken', tokens.refreshToken);
    localStorage.setItem('token', tokens.token);
    router.push('/home');
  } catch {
    isLoading.value = false;
    hasError.value = true;
    timeout = setTimeout(() => {
      hasError.value = false;
    }, 5000);
  } finally {
    isLoading.value = false;
  }
}

const onCloseErrorMessage = () => {
  hasError.value = false;
}

</script>
