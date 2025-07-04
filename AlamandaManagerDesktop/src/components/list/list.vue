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
    <DeleteModal v-if="deleteId" :refetch="refetch" :onCancel="onCancelDelete" :deleteFunction="deleteFunction" :id="deleteId">
      {{ getConfirmDeleteMessage() }}
    </DeleteModal>
    <ListForm 
      v-if="isFormOpen && !deleteId"
      :onCancel="onCancelEdit"
      :onDelete="onClickDelete"
      :addItemFunction="addItemFunction"
      :updateItemFunction="updateItemFunction"
      :onComplete="onCompleteUpdating"
      :isFieldsError="isFieldsError"
      :isFieldsPending="isFieldsPending"
      :isFormOpen="isFormOpen"
      :fields="[...fields]?.sort((a, b) => a.dataType - b.dataType)" 
      :maxImageSize="maxImageSize"
      :data="data?.items.find(item => editId === item.id)"
    />
    <div class="list-scroll">
      <table class="list-table">
        <ListHeader :sortBy="sortBy" :sortDirection="sortDirection" :sortHeader="onClickHeader" :item="data?.items[0]" />
        <tbody >
          <tr
            v-for="item of data?.items" 
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
        <div>{{ getEmptyMessage() }}</div>
        <img :src="emptyImg" alt="empty result" />
      </div>
      <div v-if="isError">
        {{ getErrorMessage() }}
      </div>
    </div>
    <Loader v-if="isPending" isGlobal/>
    <ListPagination :onChangePage="onChangePage" :totalPages="data?.totalPages" :currentPage="currentPage" />
  </div>
</template>

<script lang="ts" setup generic="TRes extends ApiResponseData, TForm">
  import { computed, ref, watch } from 'vue';
  import { useQuery } from '@tanstack/vue-query';

  import ListPagination from './listPagination.vue';
  import ListSearch from './listSearch.vue';
  import ListItem from './listItem.vue';
  import ListHeader from './listHeader.vue';
  import ListForm from './listForm.vue';
  import DeleteModal from './deleteModal.vue';
  import Loader from '../loader.vue';

  import queryKeys from '@/api/queryKeys';
  import { ApiResponseData, ListResponse, QueryParams, SortDirection } from '@/api/defaultApi';
  import { FormFieldModel } from '@/models/formFieldModel';

  const emptyImg = new URL('@/assets/images/empty.webp', import.meta.url).href;

  interface Props {
    title: string;
    maxImageSize?: number;
    label: string;
    searchFunction: (params: QueryParams) => Promise<ListResponse<TRes>>;
    addItemFunction: (data: TForm) => Promise<TRes>;
    updateItemFunction: (data: TForm) => Promise<TRes>;
    getFieldFunction: () => Promise<FormFieldModel[]>;
    deleteFunction: (id: number) => Promise<void>;
  }

  const props = defineProps<Props>();

  const currentPage = ref(1);
  const isFormOpen = ref(false);
  const sortBy = ref<null | string>(null);
  const sortDirection = ref<SortDirection>('ascending');
  const queryString = ref('');
  const editId = ref<number | null>(null);
  const deleteId = ref<number | null>(null);

  const queryParams = ref({
    queryString: queryString.value,
    sortBy: sortBy.value,
    sortDirection: sortDirection.value,
    page: currentPage.value,
  });

  const onSearch = (value: string) => {
    queryString.value = value;
  };

  const onClickEdit = (id?: number) => {
    if (!id) return;
    editId.value = id;
    deleteId.value = null;
    onOpenForm();
  };

  const onOpenForm = () => {
    isFormOpen.value = true;
  };

  const onClickDelete = (id?: number) => {
    if (!id) return;
    deleteId.value = id;
  };

  const onCancelDelete = () => {
    deleteId.value = null;
    editId.value = null;
    isFormOpen.value = false;
  };

  const onCancelEdit = () => {
    editId.value = null;
    isFormOpen.value = false;
  };

  const onChangePage = (page: number) => {
    currentPage.value = page;
  };

  const onCompleteUpdating = () => {
    refetch();
    onCancelEdit();
  };

  const debouncedQueryParams = ref({ ...queryParams.value });

  const onClickHeader = (name: string) => {
    if (sortBy.value === name) {
      if (sortDirection.value === 'descending') {
        sortDirection.value = 'ascending';
        sortBy.value = null;
        return;
      }
      sortDirection.value = 'descending';
      return;
    }

    sortBy.value = name;
  };

  const listQueryKey = computed(() => [
    queryKeys.List,
    { title: props.title, ...debouncedQueryParams.value },
  ]);

  const { isPending, isError, data, refetch } = useQuery({
    queryKey: [listQueryKey],
    queryFn: async () => await props.searchFunction(debouncedQueryParams.value),
  });

  const {
    isPending: isFieldsPending,
    isError: isFieldsError,
    data: fields,
  } = useQuery({
    queryKey: [queryKeys.Fields, { title: props.title }],
    queryFn: async () => await props.getFieldFunction(),
    gcTime: 1000 * 60 * 10,
  });

  const getErrorMessage = () => {
    return `No ${props.label}s were found with this search`;
  };

  const getEmptyMessage = () => {
    return `An error occurred while searching for ${props.label}s`;
  };

  const getConfirmDeleteMessage = () => {
    return `Are you sure you want to delete this ${props.label}?`;
  };

  watch(
    queryParams,
    (newVal) => {
      const timeout = setTimeout(() => {
        debouncedQueryParams.value = { ...newVal };
      }, 300);
      return () => clearTimeout(timeout);
    },
    { deep: true }
  );

  watch([queryString, sortBy, sortDirection, currentPage], () => {
    queryParams.value = {
      queryString: queryString.value,
      sortBy: sortBy.value,
      sortDirection: sortBy.value ? sortDirection.value : 'descending',
      page: currentPage.value,
    };
  });

  watch([queryString, sortBy], () => {
    if (currentPage.value !== 1) currentPage.value = 1;
  });
</script>