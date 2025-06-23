<style lang="scss">
 @use "@/assets/global.scss" as *;
</style>


<template>
  <router-view />
</template>

<script setup>
  import { onMounted } from 'vue';
  import { useRouter } from 'vue-router';
  const router = useRouter();
  const token = localStorage.getItem('token');
  if (!token) {
    router.push('/');
  }
</script>
