import { defineConfig } from 'vite';
import vue from '@vitejs/plugin-vue';
import path from 'path';
import tsconfigPaths from 'vite-tsconfig-paths';

export default defineConfig({
  plugins: [vue(), tsconfigPaths()],
  resolve: {
    alias: {
      '@': path.resolve(__dirname, 'src/*'),
    },
  },
});