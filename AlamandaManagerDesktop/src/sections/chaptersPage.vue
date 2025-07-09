<template>
  {{ data?.currentPage }}
</template>

<script lang="ts" setup>
import { getChapters } from '@/api/chapters';
import queryKeys from '@/api/queryKeys';
import { useQuery } from '@tanstack/vue-query';
import { computed, ref } from 'vue';
import { useRoute } from 'vue-router';

const route = useRoute();
const id = route.params.comicId;

const currentPage = ref(1);
const queryString = ref('');
const queryParams = ref({
  queryString: queryString.value,
  page: currentPage.value
});
const debouncedQueryParams = ref({ ...queryParams.value, id: parseInt(id.toString()) });

const listQueryKey = computed(() => [
  queryKeys.Chapters,
  { ...debouncedQueryParams.value }
]);

const { isPending, isError, data, refetch } = useQuery({
  queryKey: [listQueryKey],
  queryFn: async () => await getChapters(debouncedQueryParams.value)
});

</script>