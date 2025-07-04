import { FormFieldModel } from "@/models/formFieldModel";
import { ListResponse, ApiResponseData, restApi, QueryParams } from "./defaultApi";

export interface TeamMemberForm {
  name: string;
  social: string;
  picture: File | null;
}

export interface TeamMemberFormSubmit {
  name: string;
  social: string;
  picture: string | null;
}

export interface TeamMemberResponse extends ApiResponseData {
  id: number;
  name: string;
  social: string;
  picture: string;
}
const endpoint = 'team';

export async function addMember(body: TeamMemberFormSubmit) {
  return await restApi<TeamMemberResponse>({url: endpoint, method: 'POST', body});
}

export async function updateMember(body: TeamMemberFormSubmit) {
  return await restApi<TeamMemberResponse>({url: endpoint, method: 'PUT', body});
}

export async function getMembers(params: Record<string, unknown>) {
  return await restApi<ListResponse<TeamMemberResponse>>({url: endpoint, method: 'GET', params});
}

export async function deleteMember(id: number) {
  return await restApi({url: endpoint, method: 'DELETE', params : {id}});
}

export async function getMembersFields() {
  return await restApi<FormFieldModel>({url: `${endpoint}/fields`, method: 'GET'});
}