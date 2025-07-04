<template>
  <List
    title="Roles"
    label="role"
    :searchFunction="getRoles"
    :getFieldFunction="getRolesFields"
    :addItemFunction="addRole"
    :updateItemFunction="updateRole"
    :deleteFunction="deleteRole"
  />
</template>

<script lang="ts" setup>
  import { addRole, deleteRole, getRoles, getRolesFields, updateRole } from '@/api/roles';
  import List from '@/components/list/list.vue';
</script>