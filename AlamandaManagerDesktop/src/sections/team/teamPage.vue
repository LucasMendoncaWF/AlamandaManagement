<template>
  <List
    title="Time"
    emptyMessage="Nenhum membro encontrado com esses filtros"
    errorMessage="Ocorreu um erro durante a busca de membros"
    :searchFunction="getMembers"
    :getFieldFunction="getMembersFields"
    :addItemFunction="addMember"
  />
</template>

<script setup>
  import List from '@/components/list/list.vue';
  import { getMembers, getMembersFields, addMember } from '@/api/teamMembers';
</script>