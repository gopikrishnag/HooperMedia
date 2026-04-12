export interface Person {
  personId: number;
  name: string;
  dateOfBirth: string;
}

export interface CreatePersonRequest {
  name: string;
  dateOfBirth: string;
}

export type UpdatePersonRequest = CreatePersonRequest;