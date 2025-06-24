import { restApi } from "./defaultApi";

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


export function addMember(body: TeamMemberFormSubmit) {
  return restApi<TeamMemberForm>({url: 'team', method: 'POST', body});
}