
<template>
  <List
    title="Users"
    emptyMessage="No members were found with this search"
    errorMessage="An error occurred while searching for members"
    :maxImageSize="2000"
    :searchFunction="getUsers"
    :getFieldFunction="getUserFields"
    :addItemFunction="addUser"
    :updateItemFunction="updateUser"
    :deleteFunction="deleteUser"
  />
</template>

<script setup>
import { addUser, getUserFields, getUsers, updateUser, deleteUser } from '@/api/users';
  import List from '@/components/list/list.vue';
</script>