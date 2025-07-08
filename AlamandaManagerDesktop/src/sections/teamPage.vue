<template>
  <List
    title="Team"
    label="member"
    :showFieldsOnTable="['name', 'picture', 'social', 'roles', 'comics']"
    :maxImageSize="500"
    :searchFunction="getMembers"
    :addItemFunction="addMember"
    :updateItemFunction="updateMember"
    :deleteFunction="deleteMember"
  />
</template>

<script lang="ts" setup>
import List from '@/components/lists/list.vue'
import { getMembers, addMember, updateMember, deleteMember } from '@/api/teamMembers';
</script>