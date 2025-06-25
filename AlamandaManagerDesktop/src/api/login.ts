import { restApi } from "./defaultApi";

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  refreshToken: string;
}

export function userLogIn(body: LoginRequest) {
  return restApi<LoginResponse>({url: 'auth/admin', method: 'POST', body});
}