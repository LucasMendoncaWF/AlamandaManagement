const API_URL = import.meta.env.VITE_API_URL;
type Method = 'POST' | 'GET' | 'PUT' | 'DELETE';

interface RestRequest {
  url: string;
  method?: Method;
  body?: object;
  params?: string;
  headers?: HeadersInit;
}

async function refreshAccessToken() {
  const refreshToken = localStorage.getItem('refreshToken');
  const response = await fetch(`${API_URL}/auth/refresh`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ refreshToken }),
  });

  if (!response.ok) throw new Error('Failed to refresh token');

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

export async function restApi<T>(request: RestRequest): Promise<T> {
  const token = localStorage.getItem('token');
    let headers = {
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json',
  };

  let response = await fetchApi({...request, headers});

   if (response.status === 401 && !request.url.includes('auth')) {
    try {
      const newToken = await refreshAccessToken();

      const headers = {
        'Authorization': `Bearer ${newToken}`,
        'Content-Type': 'application/json',
      };
      response = await fetchApi({...request, headers});
    } catch (error) {
      localStorage.removeItem('token');
      localStorage.removeItem('refreshToken');
      window.location.href = '/';
      throw error;
    }
  }

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(errorText || `Error ${response.status}`);
  }

  const data = await response.json();
  return data as T;
}
