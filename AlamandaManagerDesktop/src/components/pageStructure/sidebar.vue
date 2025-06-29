<style lang="scss" scoped>
  @use '@/assets/variables.scss' as *;
  .sidebar {
    width: 68px;
    background-color: $secondary;
    height: 100vh;
    padding: 0 20px;
    color: $white;
    display: flex;
    flex-direction: column;
    justify-content: center;
    gap: 4%;

    .icon {
      text-align: center;
      transition: 0.4s;
      text-decoration: none;

      &:hover {
        opacity: 0.7;
        transform: scale(1.05);
      }

      label {
        display: block;
        font-size: 12px;
        color: $white;
      }

      img {
        width: 40px;
      }
    }
  }
</style>

<template>
  <div class="sidebar">
    <a :href="icon.url" class="icon" v-for="icon in icons">
      <img :src='icon.image' :alt="icon.label"/>
      <label>{{ icon.label }}</label>
    </a>
  </div>
</template>

<script lang="ts" setup>
import urls from '@/router/urls';

  interface IconModel {
    label: string;
    url: string;
    image: string;
  }
  const icons: IconModel[] = [
    {
      label: 'Orders',
      url: urls.home,
      image: new URL('@/assets/icons/icon_orders.svg', import.meta.url).href
    },
    {
      label: 'Comics',
      url: '/',
      image: new URL('@/assets/icons/icon_library.svg', import.meta.url).href
    },
    {
      label: 'Users',
      url: urls.users,
      image: new URL('@/assets/icons/icon_users.svg', import.meta.url).href
    },
    {
      label: 'Team',
      url: urls.team,
      image: new URL('@/assets/icons/icon_team.svg', import.meta.url).href
    },
    {
      label: 'Arts',
      url: urls.arts,
      image: new URL('@/assets/icons/icon_art.svg', import.meta.url).href
    }
  ];

</script>