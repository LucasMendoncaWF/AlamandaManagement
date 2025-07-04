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
      padding: 50px 0;
    }

    .empty-area {
      img {
        margin-top: 30px;
      }
    }
  }
</style>

<template>
  <div class="list">
    <ListSearch :onClickCreate="onOpenForm" :onSearch="onSearch" :title="title" />
    <div v-if="deleteId">
      DELETE COMPONENT MODAL TODO
    </div>
    <ListForm 
      v-if="isFormOpen && !deleteId"
      :onCancel="onCancelEdit"
      :onDelete="onClickDelete"
      :addItem="addItemFunction"
      :updateItem="updateItemFunction"
      :onComplete="onCompleteUpdating"
      :isFieldsError="isFieldsError"
      :isFieldsPending="isFieldsPending"
      :isFormOpen="isFormOpen"
      :fields="fields" 
      :maxImageSize="maxImageSize"
      :data="data?.items.find(item => editId === item.id)"
    />
    <div class="list-scroll" v-if="!isError && !(data && !data.items.length)">
      <table class="list-table">
        <ListHeader :sortBy="sortBy" :sortDirection="sortDirection" :sortHeader="onClickHeader" v-if="data && data.items.length" :item="data.items[0]" />
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
            />
          </tr>
        </tbody>
      </table>
    </div>
    <div v-if="isError || (data && !data.items.length)" class="state-ui">
      <div class="empty-area" v-if="data && !data.items.length">
        <div>{{ emptyMessage }}</div>
        <img :src="emptyImg" alt="empty result" />
      </div>
      <div v-if="isError">
        {{ errorMessage }}
      </div>
    </div>
    <Loader v-if="isPending" isGlobal/>
    <ListPagination :onChangePage="onChangePage" :totalPages="data?.totalPages" :currentPage="currentPage" />
  </div>
</template>

<script lang="ts" setup>
  const emptyImg = new URL('@/assets/images/empty.webp', import.meta.url).href;
  import ListPagination from './listPagination.vue';
  import ListSearch from './listSearch.vue';
  import { computed, ref } from 'vue';
  import queryKeys from '@/api/queryKeys';
  import { useQuery } from '@tanstack/vue-query';
  import { ListResponse, ApiResponseData, QueryParams, SortDirection } from '@/api/defaultApi';
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
    deleteFunction: (id: number) => void;
  }

  const props = defineProps<Props>();
  const currentPage = ref(1);
  const isFormOpen = ref(false);
  const sortBy = ref('');
  const sortDirection = ref<SortDirection>('descending');
  const queryString = ref('');
  const editId = ref<number | null>(null);
  const deleteId = ref<number | null>(null);

  const onSearch = (value: string) => {
    queryString.value = value;
  }

  const onClickEdit = (id?: number) => {
    if(!id) { return }
    editId.value = id;
    deleteId.value = null;
    onOpenForm();
  }

  const onOpenForm = () => {
    isFormOpen.value = true;
  }

  const onClickDelete = (id?: number) => {
    if(!id) { return }
    deleteId.value = id;
  }

  const confirmDelete = async () => {
    if(deleteId.value) {
      await props.deleteFunction(deleteId.value);
      await refetch();
    }
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

  const onClickHeader = (name: string) => {
    if(sortBy.value === name) {
      if(sortDirection.value === 'ascending') {
        sortDirection.value = 'descending';
        sortBy.value = '';
        return;
      }
      sortDirection.value = 'ascending';
      return;
    }

    sortBy.value = name;
  }

  const listQueryKey = computed(() => [
    queryKeys.List,
    { title: props.title, queryString: queryString.value, sortBy: sortBy.value, sortDirection: sortDirection.value, page: currentPage.value }
  ]);

  const { isPending, isError, data, refetch } = useQuery({
    queryKey: [listQueryKey],
    async queryFn() {
      return await props.searchFunction({queryString: queryString.value, sortBy: sortBy.value, sortDirection: sortDirection.value, page: currentPage.value});
    },
    
  });

    const { isPending: isFieldsPending, isError: isFieldsError, data: fields } = useQuery({
    queryKey: [queryKeys.Fields, {title: props.title}],
    async queryFn() {
      return await props.getFieldFunction();
    },
    gcTime: 1000 * 60 * 10,
  });
</script>