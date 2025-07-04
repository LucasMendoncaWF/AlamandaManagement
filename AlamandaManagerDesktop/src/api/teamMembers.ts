import { FormFieldModel } from "@/models/formFieldModel";
import { ListResponse, ApiResponseData, restApi, QueryParams } from "./defaultApi";

export interface TeamMemberFormSubmit {
  id?: number;
  name: string;
  social: string;
  picture: string | null | File;
}

export interface TeamMemberResponse extends ApiResponseData {
  id: number;
  name: string;
  social: string;
  picture: string | null;
}

const endpoint = 'team';

export async function addMember(body: TeamMemberFormSubmit) {
  return await restApi<TeamMemberResponse>({url: endpoint, method: 'POST', body});
}

export async function updateMember(body: TeamMemberFormSubmit) {
  return await restApi<TeamMemberResponse>({url: endpoint, method: 'PUT', body});
}

export async function getMembers(params: QueryParams) {
  return await restApi<ListResponse<TeamMemberResponse>>({url: endpoint, method: 'GET', params});
}

export async function deleteMember(id: number) {
  return await restApi<void>({url: endpoint, method: 'DELETE', params : {id}});
}

export async function getMembersFields() {
  return await restApi<FormFieldModel[]>({url: `${endpoint}/fields`, method: 'GET'});
}