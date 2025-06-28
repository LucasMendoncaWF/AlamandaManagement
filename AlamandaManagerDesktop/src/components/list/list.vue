<style lang="scss" scoped>
  @use '@/assets/variables.scss' as *;
  .list {
    .list-scroll {
      max-height: calc(100vh - 290px);
      overflow: auto;
      position: relative;
      margin-bottom: 15px;
    }
  
    .list-table {
      width: 100%;
      border-collapse: collapse;
      border: none;
    }

    .list-body {
      margin-bottom: 20px;
      width: 100%;
    }

    .list-item {
      padding: 10px 20px;
      &:nth-child(odd) {
        background-color: $lightGray;
      }
    }

    .state-ui {
      text-align: center;
      padding: 20px 0;
      background-color: $lightGray;
    }
  }
</style>

<template>
  <div class="list">
    <ListSearch :filters="filters" :onSearch="onSearch" :onFilter="onFilterChange" :title="title" />
    <component 
      :is="formComponent" 
      :onComplete="refetch"
    />
    <div class="list-scroll">
      <table class="list-table">
        <ListHeader v-if="data && data.items.length" :item="data.items[0]" />
        <tbody >
          <tr
            v-if="data && data.items.length" 
            v-for="item of data.items" 
            class="list-item"
          >
            <ListItem
              v-if="editId !== item.id"
              :key="item.id"
              :item="item"
              :onClickEdit="() => onClickEdit(item.id)"
              :onClickDelete="() => onClickDelete(item.id)"
            />
            <td colspan="10" v-if="editId === item.id">
              <component
                :data="data"
                :is="formComponent" 
                :onComplete="onCancelEdit"
              />
            </td>
          </tr>
        </tbody>
      </table>
    </div>
    <div v-if="isError || (data && !data.items.length)" class="state-ui">
      <div v-if="data && !data.items.length">
        {{ emptyMessage }}
      </div>
      <div v-if="isError">
        {{ errorMessage }}
      </div>
    </div>
    <Loader v-if="isPending" />

    <ListPagination :onChangePage="onChangePage" :totalPages="data?.totalPages" :currentPage="currentPage" />
  </div>
</template>

<script lang="ts" setup>
  import ListPagination from './listPagination.vue';
  import ListSearch from './listSearch.vue';
  import { ref, VueElement } from 'vue';
  import queryKeys from '@/api/queryKeys';
  import { useQuery } from '@tanstack/vue-query';
  import { ListResponse, ApiResponseData } from '@/api/defaultApi';
  import ListItem from './listItem.vue';
  import Loader from '../loader.vue';
import ListHeader from './listHeader.vue';

  interface Props<T extends ApiResponseData = ApiResponseData> {
    title: string;
    emptyMessage: string;
    errorMessage: string;
    formComponent: VueElement;
    filters?: VueElement;
    searchFunction: (filters?: T) => ListResponse<T>;
  }

  const props = defineProps<Props>();
  const { isPending, isError, data, refetch } = useQuery({
    queryKey: [queryKeys.TeamMembersList],
    async queryFn() {
      return await props.searchFunction(currentFilters.value);
    }
  });

  const currentPage = ref(1);
  const editId = ref<number | null>(null);
  const deleteId = ref<number | null>(null);
  const currentFilters = ref();

  const onFilterChange = () => {

  }

  const onSearch = () => {

  }

  const onClickEdit = (id: number) => {
    editId.value = id;
  }

  const onClickDelete = (id: number) => {
    deleteId.value = id;
  }

  const onCancelEdit = () => {
    editId.value = null;
  }

  const onChangePage = (page: number) => {
    currentPage.value = page;
  }
</script>