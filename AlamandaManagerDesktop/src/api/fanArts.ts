import { FormFieldModel } from "@/models/formFieldModel";
import { ListResponse, ApiResponseData, restApi, QueryParams } from "./defaultApi";

export interface ArtForm {
  name: string;
  social: string;
  picture: File | null;
}

export interface ArtSubmitForm {
  name: string;
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
  return await restApi<ArtResponse>({url: endpoint, method: 'POST', body});
}

export async function updateArt(body: ArtSubmitForm) {
  return await restApi<ArtResponse>({url: endpoint, method: 'PUT', body});
}

export async function getArts(params: Record<string, unknown>) {
  return await restApi<ListResponse<ArtResponse>>({url: endpoint, method: 'GET', params});
}

export async function deleteArt(id: number) {
  return await restApi({url: endpoint, method: 'DELETE', params : {id}});
}

export async function getArtFields() {
  return await restApi<FormFieldModel>({url: `${endpoint}/fields`, method: 'GET'});
}