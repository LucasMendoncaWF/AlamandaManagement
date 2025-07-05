import { FormFieldModel } from "@/models/formFieldModel";
import { ListResponse, ApiResponseData, restApi, QueryParams } from "./defaultApi";

interface CategoryForm {
  name: string;
}
export interface CategoryData extends ApiResponseData, CategoryForm {}
const endpoint = 'category';

export async function addCategory(body: CategoryForm) {
  return await restApi<CategoryData>({url: endpoint, method: 'POST', body});
}

export async function updateCategory(body: CategoryData) {
  return await restApi<CategoryData>({url: endpoint, method: 'PUT', body});
}

export async function getCategories(params: QueryParams) {
  return await restApi<ListResponse<CategoryData>>({url: endpoint, method: 'GET', params});
}

export async function deleteCategory(id: number) {
  return await restApi<void>({url: endpoint, method: 'DELETE', params : {id}});
}

export async function getCategoriesFields() {
  return await restApi<FormFieldModel[]>({url: `${endpoint}/fields`, method: 'GET'});
}