import { ListResponse, ApiResponseData, restApi, QueryParams } from './defaultApi';

interface RoleForm {
  name: string;
}
export interface RoleData extends ApiResponseData, RoleForm {}
const endpoint = 'role';

export async function addRole(body: RoleData) {
  return await restApi<RoleData>({ url: endpoint, method: 'POST', body });
}

export async function updateRole(body: RoleData) {
  return await restApi<RoleData>({ url: endpoint, method: 'PUT', body });
}

export async function getRoles(params: QueryParams) {
  return await restApi<ListResponse<RoleData>>({ url: endpoint, method: 'GET', params });
}

export async function deleteRole(id: number) {
  return await restApi<void>({ url: endpoint, method: 'DELETE', params : { id } });
}