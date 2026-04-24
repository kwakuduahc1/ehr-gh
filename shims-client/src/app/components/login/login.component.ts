import { Component, inject, ChangeDetectionStrategy, signal } from '@angular/core';
import { form, FormField, required, email, minLength, maxLength, validate, schema, FormRoot } from '@angular/forms/signals';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIcon } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { IUsers, LoginVm } from '../../models/IUsers';
import { ActivityProvider } from '../../providers/ActivityProvider';
import { MatDialogRef } from '@angular/material/dialog';
import { validatePasswordHasLowercase, validatePasswordHasNumber, validatePasswordHasUppercase } from '../auth-validators';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    FormField,
    MatFormFieldModule,
    MatIcon,
    MatInputModule,
    MatButtonModule,
    FormRoot
  ]
})
export class LoginComponent {
  readonly act = inject(ActivityProvider);
  readonly diag = inject(MatDialogRef);
  private readonly storedUser = JSON.parse(localStorage.getItem('user') as string) as IUsers | null;

  loginModel = signal<LoginVm>({
    email: this.storedUser?.email ?? '',
    password: this.storedUser?.password ?? ''
  });

  form = form(this.loginModel, LoginSchema, {
    name: 'add-participant',
    submission: {
      action: async (f) => this.login({
        ...f().value(),
      })
    }
  });

  login(user: LoginVm): void {
    if (!this.form().valid()) return;
    this.diag.close(user);
  }
}


const LoginSchema = schema<LoginVm>(path => {
  // Email validations
  required(path.email);
  email(path.email);
  minLength(path.email, 5);
  maxLength(path.email, 75);

  // Password validations
  required(path.password);
  minLength(path.password, 6);
  maxLength(path.password, 15);
});
