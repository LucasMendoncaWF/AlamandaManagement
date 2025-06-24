

<template>
  <List :totalPages="10" :currentPage="1" title="Time">
    test
    <TeamForm />
  </List>
</template>

<script setup>
  import List from '@/components/list/list.vue';
  import TeamForm from './teamForm.vue';
</script>