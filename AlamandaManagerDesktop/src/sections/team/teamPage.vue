<template>
  <List
    title="Team"
    emptyMessage="No members were found with this search"
    errorMessage="An Error Occurred while searching for members"
    :maxImageSize="200"
    :searchFunction="getMembers"
    :getFieldFunction="getMembersFields"
    :addItemFunction="addMember"
    :updateItemFunction="updateMember"
  />
</template>

<script setup>
  import List from '@/components/list/list.vue';
  import { getMembers, getMembersFields, addMember, updateMember } from '@/api/teamMembers';
</script>