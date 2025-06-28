<template>
  <List
    title="Time"
    emptyMessage="Nenhum membro encontrado com esses filtros"
    errorMessage="Ocorreu um erro durante a busca de membros"
    :formComponent="TeamForm"
    :searchFunction="getMembers"
  />
</template>

<script setup>
  import List from '@/components/list/list.vue';
  import TeamForm from './teamForm.vue';
  import { getMembers } from '@/api/teamMembers';
</script>