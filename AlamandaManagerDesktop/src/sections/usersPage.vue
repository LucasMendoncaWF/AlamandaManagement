<template>
  <List
    title="Users"
    label="user"
    :showFieldsOnTable="['picture', 'userName', 'email', 'permission']"
    :searchFunction="getUsers"
    :addItemFunction="addUser"
    :updateItemFunction="updateUser"
    :deleteFunction="deleteUser"
  />
</template>

<script lang="ts" setup>
import { addUser, getUsers, updateUser, deleteUser } from '@/api/users';
import List from '@/components/lists/list.vue';
</script>