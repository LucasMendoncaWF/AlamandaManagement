import vuePlugin from 'eslint-plugin-vue'
import vueParser from 'vue-eslint-parser'
import tsParser from '@typescript-eslint/parser'
import tsPlugin from '@typescript-eslint/eslint-plugin'

export default [
  {
    files: ['src/**/*.{js,ts,vue}'],
    languageOptions: {
      parser: vueParser,
      parserOptions: {
        parser: tsParser,
        ecmaVersion: 'latest',
        sourceType: 'module',
        extraFileExtensions: ['.vue']
      }
    },
    plugins: {
      vue: vuePlugin,
      '@typescript-eslint': tsPlugin
    },
    rules: {
      quotes: ['error', 'single'],
      'comma-dangle': ['error', 'never'],
      indent: ['error', 2],
      'vue/html-indent': ['error', 2],
      'vue/max-attributes-per-line': ['warn', {
        singleline: 3,
        multiline: 1
      }],
      'vue/singleline-html-element-content-newline': 'off',
      'no-unused-vars': 'off',
      '@typescript-eslint/no-unused-vars': ['warn'],
      'no-console': ['warn'],
      eqeqeq: ['error', 'always'],
      curly: ['error', 'all'],
      'prefer-const': ['warn'],
      'no-debugger': ['error'],
      'object-curly-spacing': ['error', 'always']
    }
  },
]
