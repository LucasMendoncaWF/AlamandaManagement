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
    <ErrorMessage message="Verifique os dados inseridos e tente novamente" v-if="hasError" />
    <form @submit.prevent="onSubmit">
      <div class="form-content">
        <img class="logo-image" :src="logo" alt="logo" />
        <FormInput id="email" required label="E-mail" type="email" v-model="loginData.email"/>
        <FormInput id="pass" required label="Password" type="password" v-model="loginData.password"/>
        <FormSubmit :disabled="!loginData.email || !loginData.password || isLoading" value="Entrar" />
      </div>
    </form>
  </div>
</template>

<script lang="ts" setup>
  import { ref } from 'vue';
  import { useRouter } from 'vue-router';
  import FormInput from '@/components/forms/formInput.vue';
  import FormSubmit from '@/components/forms/formSubmit.vue';
  import ErrorMessage from '@/components/errorMessage.vue';
  import { type LoginRequest, userLogIn, type LoginResponse } from '@/api/login';
  const logo = new URL('@/assets/images/logo.png', import.meta.url).href;

  const router = useRouter();
  const hasError = ref<boolean>(false);
  const isLoading = ref<boolean>(false);
  const loginData = ref<LoginRequest>({
    email: '',
    password: '',
  });

  const onSubmit = async () => {
    try {
      const tokens: LoginResponse = await userLogIn({email: loginData.value.email, password: loginData.value.password});
      localStorage.setItem('refreshToken', tokens.refreshToken);
      localStorage.setItem('token', tokens.token);
      router.push('/home');
    } catch (err) {
      hasError.value = true;
      setTimeout(() => {
        hasError.value = false;
      }, 5000);
    } finally {
      isLoading.value = false;
    }
    
  }

</script>
