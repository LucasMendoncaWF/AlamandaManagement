import { FormFieldOptionModel } from "@/models/formFieldModel";

const API_URL = import.meta.env.VITE_API_URL;
type Method = 'POST' | 'GET' | 'PUT' | 'DELETE';

export type ResponseKeyType = string | number | Date | string[] | FormFieldOptionModel[] | null | undefined;
export type ResponseObjectKeysTypes = Record<string, ResponseKeyType>;
export interface ApiResponseData extends ResponseObjectKeysTypes {
  id?: number;
}

interface RestRequest<P = Record<string, unknown>> {
  url: string;
  method?: Method;
  body?: object;
  params?: P;
  headers?: HeadersInit;
}

export interface ListResponse<T extends ApiResponseData>{
  items: Array<T>;
  totalPages: number;
  currentPage: number;
}

export interface QueryParams {
  page: number;
  queryString: string;
}

async function refreshAccessToken() {
  const refreshToken = localStorage.getItem('refreshToken');
  const response = await fetch(`${API_URL}/auth/refresh`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ refreshToken }),
  });

  if (!response.ok) {
    await fetch(`${API_URL}/auth/logout`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ refreshToken }),
    });
    throw new Error('Failed to refresh token');
  }

  const data = await response.json();
  const accessToken = data.token;
  const newRefreshToken = data.refreshToken;

  localStorage.setItem('token', accessToken);
  localStorage.setItem('refreshToken', newRefreshToken);

  return accessToken;
}

const fetchApi = ({ url, method = 'GET', body, params, headers }: RestRequest) => {
  const queryParams = params
  ? '?' + new URLSearchParams(
      Object.entries(params).reduce((acc, [key, value]) => {
        acc[key] = String(value);
        return acc;
      }, {} as Record<string, string>)
    ).toString()
  : '';

  return fetch(`${API_URL}/${url}${queryParams}`, {
    method,
    headers,
    body: method !== 'GET' ? JSON.stringify(body) : undefined,
  });
}

export class ApiError extends Error {
  constructor(public message: string) {
    super(message);
    this.name = "ApiError";
  }
}

export async function restApi<T>(request: RestRequest): Promise<T> {
  const token = localStorage.getItem('token');
  let headers = {
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json',
  };

  let response = await fetchApi({ ...request, headers });

  if (response.status === 401 && !request.url.includes('auth')) {
    try {
      const newToken = await refreshAccessToken();

      const headers = {
        'Authorization': `Bearer ${newToken}`,
        'Content-Type': 'application/json',
      };
      response = await fetchApi({ ...request, headers });
    } catch (error) {
      localStorage.removeItem('token');
      localStorage.removeItem('refreshToken');
      window.location.href = '/';
      throw error;
    }
  }

  if (!response.ok) {
    let errorMessage = `Error ${response.status}`;
    try {
      const errorData = await response.json();
      if (errorData && typeof errorData.message === 'string') {
        errorMessage = errorData.message;
      } else {
        const errorText = await response.text();
        if (errorText) errorMessage = errorText;
      }
    } catch {
      //
    }

    throw new ApiError(errorMessage);
  }

  const data = await response.json();
  return data as T;
}

interface ErrorMessage {
  message: string;
}

export function getErrorMessage(error: unknown): string {
  if (typeof error === 'string') return error;
  if (error && typeof (error as ErrorMessage).message === 'string') return (error as ErrorMessage).message;
  return 'Unknown error';
}