import { ListResponse, ApiResponseData, restApi, QueryParams } from './defaultApi';

export interface ArtSubmitForm {
  id?: number;
  social: string;
  picture: string | null;
}

export interface ArtResponse extends ApiResponseData {
  id: number;
  social: string;
  picture: string;
}
const endpoint = 'art';

export async function addArt(body: ArtSubmitForm) {
  return await restApi<ArtResponse>({ url: endpoint, method: 'POST', body });
}

export async function updateArt(body: ArtSubmitForm) {
  return await restApi<ArtResponse>({ url: endpoint, method: 'PUT', body });
}

export async function getArts(params: QueryParams) {
  return await restApi<ListResponse<ArtResponse>>({ url: endpoint, method: 'GET', params });
}

export async function deleteArt(id: number) {
  return await restApi<void>({ url: endpoint, method: 'DELETE', params : { id } });
}