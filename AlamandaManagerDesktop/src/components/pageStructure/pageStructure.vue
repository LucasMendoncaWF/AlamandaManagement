<style lang="scss">
 @use "@/assets/global.scss" as *;
 .structure-body {
  width: calc(100% - 80px);
  max-height: 100vh;
  overflow-y: auto;
 }
</style>

<template>
  <div class="flex">
    <Sidebar v-if="route.path !== '/'" />
    <div v-bind:class="route.path === '/' ? '' : 'structure-body'">
      <slot/>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { useRouter, useRoute  } from 'vue-router';
import Sidebar from '@/components/pageStructure/sidebar.vue';

const router = useRouter();
const route = useRoute();

const token = localStorage.getItem('token');
if (!token) {
  router.push('/');
}
</script>