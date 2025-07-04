<template>
  <List
    title="Users"
    label="user"
    :maxImageSize="500"
    :searchFunction="getUsers"
    :getFieldFunction="getUserFields"
    :addItemFunction="addUser"
    :updateItemFunction="updateUser"
    :deleteFunction="deleteUser"
  />
</template>

<script lang="ts" setup>
import { addUser, getUserFields, getUsers, updateUser, deleteUser } from '@/api/users';
  import List from '@/components/list/list.vue';
</script>