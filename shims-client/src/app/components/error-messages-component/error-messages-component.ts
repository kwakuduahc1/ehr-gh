import { Component, input } from '@angular/core';
import { MaxLengthValidationError, MaxValidationError, MinLengthValidationError, MinValidationError, PatternValidationError, WithFieldTree } from '@angular/forms/signals';
import { MatError } from '@angular/material/form-field';

@Component({
  selector: 'app-error-messages',
  imports: [
    MatError
  ],
  templateUrl: './error-messages-component.html',
  styleUrl: './error-messages-component.scss',
})
export class ErrorMessagesComponent {
  errors = input.required<WithFieldTree<any>[]>();


  minLenMsg(f: MinLengthValidationError | any) {
    return (`min chars: ${(f as MinLengthValidationError)?.minLength}`);
  }

  maxLenMsg(f: MaxLengthValidationError | any) {
    return (`max chars: ${(f as MaxLengthValidationError)?.maxLength}`);
  }

  maxMsg(f: MaxValidationError | any) {
    return (`max: ${(f as MaxValidationError)?.max}`);
  }

  minMsg(f: MinValidationError | any) {
    return (`min: ${(f as MinValidationError)?.min}`);
  }

  patternMsg(f: PatternValidationError | any) {
    return (`pattern: ${(f as PatternValidationError)?.pattern}`);
  }
}
