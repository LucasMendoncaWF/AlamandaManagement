<template>
  <List
    title="Team"
    label="member"
    :maxImageSize="500"
    :searchFunction="getMembers"
    :getFieldFunction="getMembersFields"
    :addItemFunction="addMember"
    :updateItemFunction="updateMember"
    :deleteFunction="deleteMember"
  />
</template>

<script lang="ts" setup>
  import List from '@/components/list/list.vue'
  import { getMembers, getMembersFields, addMember, updateMember, deleteMember, TeamMemberResponse, TeamMemberFormSubmit } from '@/api/teamMembers';
</script>