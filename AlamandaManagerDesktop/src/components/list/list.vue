<style lang="scss" scoped>
  @use '@/assets/variables.scss' as *;
  .list {
    padding-bottom: 50px;
    .list-scroll {
      width: 100%;
      margin-bottom: 15px;
      overflow-x: auto;
    }
  
    .list-table {
      width: 100%;
      border-collapse: collapse;
      border: none;
      min-width: 100px;
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
    <ListSearch :onClickCreate="onOpenForm" :onSearch="onSearch" :title="title" />
    <ListForm 
      :onCancel="onCancelEdit"
      :addItem="addItemFunction" 
      :updateItem="updateItemFunction"
      :onComplete="onCompleteUpdating"
      :isFormOpen="isFormOpen"
      :fields="fields" 
      :maxImageSize="maxImageSize"
      :data="data?.items.find(item => editId === item.id)"
    />
    <div class="list-scroll" v-if="!isError && !(data && !data.items.length)">
      <table class="list-table">
        <ListHeader v-if="data && data.items.length" :item="data.items[0]" />
        <tbody >
          <tr
            v-if="data && data.items.length" 
            v-for="item of data.items" 
            class="list-item"
          >
            <ListItem
              :key="item.id"
              :item="item"
              :onClickEdit="() => onClickEdit(item.id)"
              :onClickDelete="() => onClickDelete(item.id)"
            />
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
  import { ListResponse, ApiResponseData, QueryParams } from '@/api/defaultApi';
  import ListItem from './listItem.vue';
  import Loader from '../loader.vue';
  import ListHeader from './listHeader.vue';
  import ListForm from './listForm.vue';
  import { FormFieldModel } from '@/models/formFieldModel';

  interface Props<T extends ApiResponseData = ApiResponseData> {
    title: string;
    emptyMessage: string;
    errorMessage: string;
    maxImageSize: number;
    searchFunction: (params: QueryParams) => ListResponse<T>;
    addItemFunction: (data: Object) => void;
    getFieldFunction: () => FormFieldModel[];
    updateItemFunction: (data: Object) => void;
  }

  const props = defineProps<Props>();
  const currentPage = ref(1);
  const isFormOpen = ref(false);
  const queryString = ref('');
  const editId = ref<number | null>(null);
  const deleteId = ref<number | null>(null);

  const onSearch = (value: string) => {
    queryString.value = value;
    refetch();
  }

  const onClickEdit = (id?: number) => {
    if(!id) { return }
    editId.value = id;
    onOpenForm();
  }

  const onOpenForm = () => {
    isFormOpen.value = true;
  }

  const onClickDelete = (id?: number) => {
    if(!id) { return }
    deleteId.value = id;
  }

  const onCancelEdit = () => {
    editId.value = null;
    isFormOpen.value = false;
  }

  const onChangePage = (page: number) => {
    currentPage.value = page;
  }

  const onCompleteUpdating = () => {
    refetch();
    onCancelEdit();
  }

    const { isPending, isError, data, refetch } = useQuery({
    queryKey: [queryKeys.List, {title: props.title, queryString: queryString.value}],
    async queryFn() {
      return await props.searchFunction({queryString: queryString.value, page: currentPage.value});
    }
  });

    const { isPending: isFieldsPending, isError: isFieldsError, data: fields } = useQuery({
    queryKey: [queryKeys.Fields, {title: props.title}],
    async queryFn() {
      return await props.getFieldFunction();
    }
  });
</script>