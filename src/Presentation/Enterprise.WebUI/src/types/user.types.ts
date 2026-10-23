export interface UserDto {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  role: string;
  createdAt: string;
}

export interface CreateUserCommand {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  role?: string;
}
