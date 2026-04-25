import { FieldTree, MaxLengthValidationError, MaxValidationError, MinLengthValidationError, MinValidationError, PathKind, SchemaPath } from "@angular/forms/signals";

export const validatePasswordHasUppercase = ({ value }: { value: () => string }) => {
    return value() && !/[A-Z]/.test(value()) ? { kind: 'passwordUppercase' } : null;
};

export const validatePasswordHasLowercase = ({ value }: { value: () => string }) => {
    return value() && !/[a-z]/.test(value()) ? { kind: 'passwordLowercase' } : null;
};

export const validatePasswordHasNumber = ({ value }: { value: () => string }) => {
    return value() && !/\d/.test(value()) ? { kind: 'passwordNumber' } : null;
};

export const validatePasswordMatch = ({ password, confirmPassword }: { password: () => string; confirmPassword: () => string }) => {
    return password() && confirmPassword() && password() !== confirmPassword() ? { kind: 'passwordMismatch' } : null;
};

export class ValidatorMessages {

    minLenMsg(f: MinLengthValidationError | any) {
        return (`min chars: ${(f as MinLengthValidationError).minLength}`);
    }

    maxLenMsg(f: MaxLengthValidationError | any) {
        return (`min chars: ${(f as MaxLengthValidationError).maxLength}`);
    }

    maxMsg(f: MaxValidationError | any) {
        return (`max: ${(f as MaxValidationError).max}`);
    }

    minMsg(f: MinValidationError | any) {
        return (`min: ${(f as MinValidationError).min}`);
    }
}
