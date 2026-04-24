
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
  password: string;
}

export interface RegisterVm {
  email: string;
  password: string;
  confirmPassword: string;
  fullName: string;
  userRole: string;
  phoneNumber: string;
}

export interface UsersDto {
  id: string,
  userName: string,
  fullName: string,
  role: string[]
}
