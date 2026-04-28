export type Customer = {
  id: number;
  fullName: string;
  email: string | null;
  phoneNumber: string | null;
  notes: string | null;
  isActive: boolean;
};

export type CreateCustomerRequest = {
  fullName: string;
  email?: string | null;
  phoneNumber?: string | null;
  notes?: string | null;
};