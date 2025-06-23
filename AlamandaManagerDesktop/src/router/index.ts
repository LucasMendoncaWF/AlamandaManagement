import { createRouter, createWebHistory, RouteRecordRaw } from 'vue-router';
import urls from './urls';
const routes: Array<RouteRecordRaw> = [
  {
    path: urls.login,
    name: 'login',
    component: () => import('@/pages/loginPage.vue'),
  },
  {
    path: urls.home,
    name: 'home',
    component: () => import('@/pages/ordersPage.vue'),
  },
  {
    path: urls.users,
    name: 'users',
    component: () => import('@/pages/usersPage.vue'),
  },
  {
    path: urls.team,
    name: 'tean',
    component: () => import('@/pages/teamPage.vue'),
  },
  {
    path: urls.arts,
    name: 'arts',
    component: () => import('@/pages/artsPage.vue'),
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