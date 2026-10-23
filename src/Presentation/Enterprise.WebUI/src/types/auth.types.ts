export interface LoginCommand {
  email?: string;
  password?: string;
}

export interface LoginResponseDto {
  token: string;
  userId: string;
  email: string;
  firstName: string;
  lastName: string;
  role?: string;
}

export interface AuthUser {
  userId: string;
  email: string;
  firstName: string;
  lastName: string;
  role?: string;
}
