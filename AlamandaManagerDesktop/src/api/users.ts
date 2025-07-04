import { FormFieldModel } from "@/models/formFieldModel";
import { ApiResponseData, ListResponse, QueryParams, restApi } from "./defaultApi";

export interface UserFormData {
  id?: number;
  userName: string;
  email: string;
  picture: File | null;
}

export interface UserListData extends ApiResponseData {
  id: number;
  userName: string;
  email: string;
  picture: string;
}
const endpoint = 'user';

export async function addUser(body: UserFormData) {
  return await restApi<UserListData>({url: endpoint, method: 'POST', body});
}

export async function updateUser(body: UserFormData) {
  return await restApi<UserListData>({url: endpoint, method: 'PUT', body});
}

export async function getUsers(params: QueryParams) {
  return await restApi<ListResponse<UserListData>>({url: endpoint, method: 'GET', params});
}

export async function deleteUser(id: number) {
  return await restApi<void>({url: endpoint, method: 'DELETE', params : {id}});
}

export async function getUserFields() {
  return await restApi<FormFieldModel[]>({url: `${endpoint}/fields`, method: 'GET'});
}