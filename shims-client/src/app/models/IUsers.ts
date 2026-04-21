
export interface ChangePasswordVm {
  oldPassword: string;
  newPassword: string;
  confirmPassword: string;
}

export interface IUsers {
  userName: string;
  title: string;
  password: string | null;
  usersID: string;
  id: string;
  email: string;
  fullName: string;
  confirmPassword: string;
}

export interface ILogin extends IUsers {
  role: string;
}

export interface IRoles {
  id: string;
  name: string;
}

export interface URoles {
  role: string;
  id: string;
}

export interface IUserRoles {
  id: number;
  userId: string;
  claimValue: string;
  claimType: string;
}

export interface LoginVm {
  email: string;
  password: string | null;
}

export interface RegisterVm {
  userName: string;
  password: string | null;
  confirmPassword: string;
  fullName: string;
  role: string;
}

export interface UsersDto {
  id: string,
  userName: string,
  fullName: string,
  role: string[]
}
