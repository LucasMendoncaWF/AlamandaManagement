import { ListResponse, ApiResponseData, restApi, QueryParams } from './defaultApi';

export interface ComicsSubmitForm {
  id?: number;
  social: string;
  picture: string | null;
}

export interface ComicResponse extends ApiResponseData {
  id: number;
  name: string;
  picture: string;
}
const endpoint = 'comic';

export async function addComic(body: ComicsSubmitForm) {
  return await restApi<ComicResponse>({ url: endpoint, method: 'POST', body });
}

export async function updateComic(body: ComicsSubmitForm) {
  return await restApi<ComicResponse>({ url: endpoint, method: 'PUT', body });
}

export async function getComics(params: QueryParams) {
  return await restApi<ListResponse<ComicResponse>>({ url: endpoint, method: 'GET', params });
}

export async function deleteComic(id: number) {
  return await restApi<void>({ url: endpoint, method: 'DELETE', params : { id } });
}