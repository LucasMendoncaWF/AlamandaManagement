<style lang="scss">
 @use "@/assets/global.scss" as *;
 .structure-body {
   width: calc(100% - 108px);
 }
</style>


<template>
  <div class="flex">
    <Sidebar v-if="route.path !== '/'" />
    <div v-bind:class="route.path === '/' ? '' : 'structure-body'">
      <router-view />
    </div>
  </div>
</template>

<script setup>
  import { useRouter, useRoute  } from 'vue-router';
  import Sidebar from '@/components/pageStructure/sidebar.vue';

  const router = useRouter();
  const route = useRoute();

  const token = localStorage.getItem('token');
  if (!token) {
    router.push('/');
  }
</script>
