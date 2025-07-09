<template>
  <div></div>
</template>

<script lang="ts" setup>
import { useRoute } from 'vue-router';

const route = useRoute();
const id = route.params.comicId;
console.log(id)
</script>