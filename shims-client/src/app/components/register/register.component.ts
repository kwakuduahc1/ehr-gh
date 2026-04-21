import { Component, inject, input } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, FormsModule, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';

import { MatSnackBar } from "@angular/material/snack-bar";
import { CommonModule } from '@angular/common';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { Router, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatDialog } from '@angular/material/dialog';
import { filter, map, switchMap } from 'rxjs';
import { LoginHttpService } from '../../http/login-http-service';
import { RegisterVm } from '../../models/IUsers';
import { ActivityProvider } from '../../providers/ActivityProvider';
import { StatusProvider } from '../../providers/StatusProvider';
import { ConfirmationComponent } from '../confirmation/confirmation.component';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
  imports: [
    FormsModule,
    ReactiveFormsModule,
    CommonModule,
    MatInputModule,
    MatFormFieldModule,
    MatButtonModule,
    MatSelectModule,
    RouterLink
  ]
})
export class RegisterComponent {
  private snack = inject(MatSnackBar);
  private status = inject(StatusProvider);
  private diag = inject(MatDialog);
  private router = inject(Router)
  act = inject(ActivityProvider);
  http = inject(LoginHttpService);
  roles = input<string[]>();


  form = new FormGroup({
    fullName: new FormControl<string>('', [Validators.required, Validators.minLength(6), Validators.maxLength(50)]),
    userName: new FormControl<string>('', [Validators.required, Validators.minLength(6), Validators.maxLength(30)]),
    password: new FormControl<string>('',
      {
        validators: [Validators.required, Validators.minLength(6), Validators.maxLength(15)]
      }),
    confirmPassword: new FormControl<string>('', {
      validators: [Validators.required, Validators.minLength(6), Validators.maxLength(15)],
    }),
    title: new FormControl<string>('', [Validators.required, Validators.minLength(2), Validators.maxLength(8)]),
    phoneNumber: new FormControl<string>('', [Validators.required, Validators.minLength(10), Validators.maxLength(10)]),
  }, { validators: passwordMatchValidator });

  register(form: Partial<{
    userName: string | null;
    password: string | null;
    confirmPassword: string | null;
    title: string | null;
    phoneNumber: string | null;
    fullName: string | null;
  }> | null) {
    this.diag.open<ConfirmationComponent, {}, boolean>(ConfirmationComponent, {
      data: 'Are you sure you want to register?'
    })
      .afterClosed()
      .pipe(
        filter(x => !!x),
        map(() => (form as RegisterVm)),
        switchMap(x => this.http.register(x)),
        switchMap(() => this.status.login({ email: form?.userName!, password: form!.password! }))
      )
      .subscribe(() => {
        this.form.reset();
        this.snack.open('Welcome to AG-Amuasi Conference Room', 'Close');
        this.router.navigate(['/']);
      })
  }
}

export const passwordMatchValidator: ValidatorFn = (
  control: AbstractControl
): ValidationErrors | null => {
  const password = control.get('password');
  const confirm = control.get('confirmPassword');

  if (!password || !confirm) {
    return null; // controls not yet initialized
  }

  return password.value === confirm.value
    ? null
    : { passwordMismatch: true };
};

// export function passwordComplexityValidator {
//   return (control: AbstractControl): ValidationErrors | null => {
//     if (!control.value)
//       return null;
//     const value = control.value || '';
//     const hasUppercase = /[A-Z]/.test(value);
//     const hasLowercase = /[a-z]/.test(value);
//     const hasNumber = /\d/.test(value);
//     const hasSpecialChar = /[!@#$%^&*(),.?":{}|<>]/.test(value);
//     const isValid = hasUppercase && hasLowercase && hasNumber && hasSpecialChar && value.length >= 8;
//     if (isValid)
//       return null;
//     else {
//       if (!hasNumber)
//         return { passwordCheck: 'number' };
//       else if (!hasUppercase)
//         return { passwordCheck: 'uppercase' };
//       else if (!hasLowercase)
//         return { passwordCheck: 'lowercase' };
//       else if (!hasSpecialChar)
//         return { passwordCheck: 'specialCharacter' };
//     }
//     return null;
//   }
// }
