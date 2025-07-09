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
    <DeleteModal v-if="deleteId"
                 :refetch="refetch"
                 :onCancel="onCancelDelete"
                 :deleteFunction="deleteFunction"
                 :id="deleteId">
      {{ getConfirmDeleteMessage() }}
    </DeleteModal>
    <FormModal
      v-if="isFormOpen && !deleteId"
      :onCancel="onCancelEdit"
      :onDelete="onClickDelete"
      :addItemFunction="addItemFunction"
      :updateItemFunction="updateItemFunction"
      :onComplete="onCompleteUpdating"
      :isFieldsError="isError"
      :isFieldsPending="isPending"
      :isFormOpen="isFormOpen"
      :fields="[...data?.fields]?.sort((a, b) => a.dataType - b.dataType)"
      :data="data?.items.find(item => editId === item.id)"
    />
    <div class="list-scroll">
      <table class="list-table">
        <ListHeader 
          :hasClickItem="!!onClickItem"
          :item="filteredItems && filteredItems[0]" 
        />
        <tbody >
          <tr
            v-for="item of filteredItems" 
            class="list-item"
          >
            <ListItem
              :key="item.id"
              :item="item"
              :onClickEdit="() => onClickEdit(item.id)"
              :onClickItem="onClickItem"
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
import Loader from '@/components/stateHandling/loader.vue';

import queryKeys from '@/api/queryKeys';
import { ApiResponseData, ListResponse, QueryParams, ResponseKeyType } from '@/api/defaultApi';
import DeleteModal from '@/components/modals/deleteModal.vue';
import FormModal from '@/components/modals/formModal/formModal.vue';

const emptyImg = new URL('@/assets/images/empty.webp', import.meta.url).href;

  interface Props {
    title: string;
    label: string;
    showFieldsOnTable: string[];
    searchFunction: (params: QueryParams) => Promise<ListResponse<TRes>>;
    addItemFunction: (data: TForm) => Promise<TRes>;
    updateItemFunction: (data: TForm) => Promise<TRes>;
    deleteFunction: (id: number) => Promise<void>;
    onClickItem?: (id: number) => void;
  }

const props = defineProps<Props>();

const currentPage = ref(1);
const isFormOpen = ref(false);
const queryString = ref('');
const editId = ref<number | null>(null);
const deleteId = ref<number | null>(null);
const filteredItems = ref<ApiResponseData[]>();

const queryParams = ref({
  queryString: queryString.value,
  page: currentPage.value
});

const onSearch = (value: string) => {
  queryString.value = value;
};

const onClickEdit = (id?: number) => {
  if (!id) {return;}
  editId.value = id;
  deleteId.value = null;
  onOpenForm();
};

const onOpenForm = () => {
  isFormOpen.value = true;
};

const onClickDelete = (id?: number) => {
  if (!id) {return;}
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

const listQueryKey = computed(() => [
  queryKeys.List,
  { title: props.title, ...debouncedQueryParams.value }
]);

const { isPending, isError, data, refetch } = useQuery({
  queryKey: [listQueryKey],
  queryFn: async () => await props.searchFunction(debouncedQueryParams.value)
});

const getErrorMessage = () => {
  return `An error occurred while searching for ${convertToPlural(props.label)}`;
};

const getEmptyMessage = () => {
  return `No ${convertToPlural(props.label)} were found with this search`;
};

const getConfirmDeleteMessage = () => {
  return `Are you sure you want to delete this ${props.label}?`;
};

const convertToPlural = (value: string) => {
  return value[value.length-1] === 'y' ? value.replace('y', 'ies') : value + 's';
}

watch(data, (newData) => {
  if (newData) {
    filteredItems.value = newData.items.map(item => {
      const filtered: Record<string, ResponseKeyType> = {};
      Object.keys(item).forEach(key => {
        if ([...props.showFieldsOnTable, 'id']?.includes(key)) {
          filtered[key] = item[key];
        }
      });
      return filtered as ApiResponseData;
    });
  }
})

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

watch([queryString, currentPage], () => {
  queryParams.value = {
    queryString: queryString.value,
    page: currentPage.value
  };
});

watch([queryString], () => {
  if (currentPage.value !== 1) {currentPage.value = 1;}
});
</script>