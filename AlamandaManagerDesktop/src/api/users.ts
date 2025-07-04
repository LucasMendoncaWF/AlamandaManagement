import { FormFieldModel } from "@/models/formFieldModel";
import { ApiResponseData, ListResponse, restApi } from "./defaultApi";

export interface UserListData extends ApiResponseData {
  id: number;
  userName: string;
  email: string;
}

export async function addUser(body: UserListData) {
  return await restApi<UserListData>({url: 'user', method: 'POST', body});
}

export async function updateUser(body: UserListData) {
  return await restApi<UserListData>({url: 'user', method: 'PUT', body});
}

export async function getUsers(params: Record<string, unknown>) {
  return await restApi<ListResponse<UserListData>>({url: 'user', method: 'GET', params});
}

export async function getUserFields() {
  return await restApi<FormFieldModel>({url: 'user/fields', method: 'GET'});
}