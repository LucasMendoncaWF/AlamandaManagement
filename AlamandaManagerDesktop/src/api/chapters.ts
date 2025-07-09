import { ChapterFormModel, ChapterModel, ChaptersListModel } from '@/models/comicsModel';
import { restApi, QueryParams } from './defaultApi';

export interface ChaptersQueryParams extends QueryParams {
  id?: number;
}

const endpoint = 'chapters';

export async function addChapter(body: ChapterFormModel) {
  return await restApi<ChapterModel>({ url: endpoint, method: 'POST', body });
}

export async function updateChapter(body: ChapterFormModel) {
  return await restApi<ChapterModel>({ url: endpoint, method: 'PUT', body });
}

export async function getChapters(params: QueryParams) {
  return await restApi<ChaptersListModel>({ url: endpoint, method: 'GET', params });
}

export async function deleteChapter(id: number) {
  return await restApi<void>({ url: endpoint, method: 'DELETE', params : { id } });
}