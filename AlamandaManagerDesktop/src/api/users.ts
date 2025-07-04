import { FormFieldModel } from "@/models/formFieldModel";
import { ApiResponseData, ListResponse, restApi } from "./defaultApi";

export interface UserListData extends ApiResponseData {
  id: number;
  userName: string;
  email: string;
}
const endpoint = 'user';

export async function addUser(body: UserListData) {
  return await restApi<UserListData>({url: endpoint, method: 'POST', body});
}

export async function updateUser(body: UserListData) {
  return await restApi<UserListData>({url: endpoint, method: 'PUT', body});
}

export async function getUsers(params: Record<string, unknown>) {
  return await restApi<ListResponse<UserListData>>({url: endpoint, method: 'GET', params});
}

export async function deleteUser(id: number) {
  return await restApi({url: endpoint, method: 'DELETE', params : {id}});
}

export async function getUserFields() {
  return await restApi<FormFieldModel>({url: `${endpoint}/fields`, method: 'GET'});
}