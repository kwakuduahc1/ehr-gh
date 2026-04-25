export interface LoginVm {
    email: string;
    password: string;
}

export interface RegisterVM {
    email: string;
    userRole: string;
    password: string;
    confirmPassword: string;
    fullName: string;
    phoneNumber: string | null;
}
