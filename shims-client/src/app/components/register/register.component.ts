import { Component, inject, input, signal } from '@angular/core';
import { MatSnackBar } from "@angular/material/snack-bar";
import { CommonModule } from '@angular/common';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatDialog } from '@angular/material/dialog';
import { filter, map, switchMap } from 'rxjs';
import { LoginHttpService } from '../../http/login-http-service';
import { RegisterVm } from '../../models/IUsers';
import { ActivityProvider } from '../../providers/ActivityProvider';
import { StatusProvider } from '../../providers/StatusProvider';
import { ConfirmationComponent } from '../confirmation/confirmation.component';
import { form, required, schema, minLength, maxLength, validate, email, FormRoot, FormField, ValidationError, MinLengthValidationError, WithFieldTree } from '@angular/forms/signals';
import { validatePasswordHasLowercase, validatePasswordHasNumber, validatePasswordHasUppercase, ValidatorMessages } from '../auth-validators';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
  imports: [
    CommonModule,
    MatInputModule,
    MatFormFieldModule,
    MatButtonModule,
    MatSelectModule,
    FormRoot,
    FormField
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
  regMod = signal<RegisterVm>({
    confirmPassword: '',
    fullName: '',
    password: '',
    userRole: '',
    email: '',
    phoneNumber: ''
  });

  validators = new ValidatorMessages();

  form = form<RegisterVm>(this.regMod, RegisterSchema);

  register(form: Partial<RegisterVm> | null) {
    this.diag.open<ConfirmationComponent, {}, boolean>(ConfirmationComponent, {
      data: 'Are you sure you want to register?'
    })
      .afterClosed()
      .pipe(
        filter(x => !!x),
        map(() => (form as RegisterVm)),
        switchMap(x => this.http.register(x)),
        switchMap(() => this.status.login(this.form().value())
        )
      )
      .subscribe(() => {
        this.form().reset();
        this.snack.open('Welcome', 'Close');
      })
  }

  // minLenMsg(f: any | undefined) {
  //   return (`min chars: ${(f as MinLengthValidationError).minLength}`);
  // }
}


const RegisterSchema = schema<RegisterVm>((path) => {
  // Email validations
  required(path.email);
  minLength(path.email, 10);
  maxLength(path.email, 100);
  email(path.email);

  // Full Name validations
  required(path.fullName);
  minLength(path.fullName, 5);
  maxLength(path.fullName, 50);

  // Phone Number validations
  required(path.phoneNumber);
  minLength(path.phoneNumber, 10);
  maxLength(path.phoneNumber, 10);

  // Password validations
  required(path.password);
  minLength(path.password, 6);
  maxLength(path.password, 15);
  validate(path.password, validatePasswordHasUppercase);
  validate(path.password, validatePasswordHasLowercase);
  validate(path.password, validatePasswordHasNumber);

  // Confirm Password validations
  required(path.confirmPassword);
  minLength(path.confirmPassword, 6);
  maxLength(path.confirmPassword, 15);
  validate(path.confirmPassword, ({ value, valueOf }) => {
    const confirmPassword = value();
    const password = valueOf(path.password);
    if (confirmPassword !== password) {
      return {
        kind: 'passwordMismatch',
        message: 'Passwords do not match',
      };
    }
    return null;
  });

  // User Role validations
  required(path.userRole);
  minLength(path.userRole, 4);
  maxLength(path.userRole, 30);
});
