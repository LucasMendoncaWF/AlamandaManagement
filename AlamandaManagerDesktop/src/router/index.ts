import { createRouter, createWebHistory, RouteRecordRaw } from 'vue-router';
import urls from './urls';
const routes: Array<RouteRecordRaw> = [
  {
    path: urls.login,
    name: 'login',
    component: () => import('@/sections/loginPage.vue'),
  },
  {
    path: urls.home,
    name: 'home',
    component: () => import('@/sections/ordersPage.vue'),
  },
  {
    path: urls.users,
    name: 'users',
    component: () => import('@/sections/usersPage.vue'),
  },
  {
    path: urls.team,
    name: 'team',
    component: () => import('@/sections/teamPage.vue'),
  },
  {
    path: urls.arts,
    name: 'arts',
    component: () => import('@/sections/artsPage.vue'),
  },
  {
    path: urls.roles,
    name: 'roles',
    component: () => import('@/sections/rolesPage.vue'),
  },
  {
    path: urls.comics,
    name: 'comics',
    component: () => import('@/sections/comicsPage.vue'),
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

router.beforeEach((to, _from, next) => {
  const token = localStorage.getItem('token');
  if (token && to.path === '/') {
    next('/home');
  } else {
    next();
  }
});

export default router;