<style lang="scss" scoped>
  @use '@/assets/variables.scss' as *;
  .list {
    .list-scroll {
      max-height: calc(100vh - 400px);
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
    <ListSearch :onSearch="onSearch" :onFilter="onFilterChange" :title="title" />
    <ListForm 
      :onCancel="onCancelEdit" 
      :addItem="addItemFunction" 
      :fields="fields" 
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
              <ListForm 
                :onCancel="onCancelEdit" 
                :addItem="addItemFunction" 
                :fields="fields" 
                :data="item" 
                :onComplete="refetch"
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
    <Loader v-if="isPending"/>

    <ListPagination :onChangePage="onChangePage" :totalPages="data?.totalPages" :currentPage="currentPage" />
  </div>
</template>

<script lang="ts" setup>
  import ListPagination from './listPagination.vue';
  import ListSearch from './listSearch.vue';
  import { ref } from 'vue';
  import queryKeys from '@/api/queryKeys';
  import { useQuery } from '@tanstack/vue-query';
  import { ListResponse, ApiResponseData } from '@/api/defaultApi';
  import ListItem from './listItem.vue';
  import Loader from '../loader.vue';
  import ListHeader from './listHeader.vue';
  import ListForm from './listForm.vue';
  import { FormFieldModel } from '@/models/formFieldModel';

  interface Props<T extends ApiResponseData = ApiResponseData> {
    title: string;
    emptyMessage: string;
    errorMessage: string;
    searchFunction: (queryString: string) => ListResponse<T>;
    addItemFunction: (data: T) => void;
    getFieldFunction: () => FormFieldModel[];
  }

  const props = defineProps<Props>();
  const currentPage = ref(1);
  const queryString = ref('');
  const editId = ref<number | null>(null);
  const deleteId = ref<number | null>(null);

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

    const { isPending, isError, data, refetch } = useQuery({
    queryKey: [queryKeys.List, {title: props.title, queryString: queryString.value}],
    async queryFn() {
      return await props.searchFunction(queryString.value);
    }
  });

    const { isPending: isFieldsPending, isError: isFieldsError, data: fields } = useQuery({
    queryKey: [queryKeys.Fields, {title: props.title}],
    async queryFn() {
      return await props.getFieldFunction();
    }
  });
</script>