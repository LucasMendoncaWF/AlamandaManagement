import { createApp } from 'vue';
import { createPinia } from 'pinia';
import router from './router';
import App from './app.vue';
import { VueQueryPlugin } from '@tanstack/vue-query';

const app = createApp(App);
const pinia = createPinia();

app.use(VueQueryPlugin);
app.use(pinia);
app.use(router);
app.mount('#app');