<template>
  <List
    title="Team"
    emptyMessage="No members were found with this search"
    errorMessage="An error occurred while searching for members"
    :maxImageSize="2000"
    :searchFunction="getMembers"
    :getFieldFunction="getMembersFields"
    :addItemFunction="addMember"
    :updateItemFunction="updateMember"
    :deleteFunction="deleteMember"
  />
</template>

<script setup>
  import List from '@/components/list/list.vue';
  import { getMembers, getMembersFields, addMember, updateMember, deleteMember } from '@/api/teamMembers';
</script>