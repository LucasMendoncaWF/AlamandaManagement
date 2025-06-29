import { FormFieldModel } from "@/models/formFieldModel";
import { ListResponse, ApiResponseData, restApi } from "./defaultApi";

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

export interface TeamMemberFilters {
  page: number;
  queryString: string;
}


export async function addMember(body: TeamMemberFormSubmit) {
  return await restApi<TeamMemberForm>({url: 'team', method: 'POST', body});
}

export async function getMembers(filters: TeamMemberFilters) {
  return await restApi<ListResponse<TeamMemberResponse>>({url: 'team', method: 'GET', params: filters});
}

export async function getMembersFields() {
  return await restApi<FormFieldModel>({url: 'team/fields', method: 'GET'});
}