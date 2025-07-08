<template>
  <List
    title="Roles"
    label="role"
    :showFieldsOnTable="['name']"
    :searchFunction="getRoles"
    :addItemFunction="addRole"
    :updateItemFunction="updateRole"
    :deleteFunction="deleteRole"
  />
</template>

<script lang="ts" setup>
import { addRole, deleteRole, getRoles, updateRole } from '@/api/roles';
import List from '@/components/lists/list.vue';
</script>