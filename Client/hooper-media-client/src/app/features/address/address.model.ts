export interface Address {
  addressId: number;
  personId: number;
  addressLine1: string;
  addressLine2: string | null;
  townOrCity: string;
  zipOrPostCode: string;
  country: string;
}

export interface CreateAddressRequest {
  personId: number;
  addressLine1: string;
  addressLine2: string | null;
  townOrCity: string;
  zipOrPostCode: string;
  country: string;
}

export type UpdateAddressRequest = CreateAddressRequest;
