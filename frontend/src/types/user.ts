export type UserRole = "Receptionist" | "Management";

export type User = {
  id: number;
  username: string;
  role: UserRole;
  isActive: boolean;
  createdAtUtc: string;
};

export type CreateUserRequest = {
  username: string;
  password: string;
  role: UserRole;
};