import { HttpErrorResponse } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({ providedIn: 'root' })
export class BsHandler {

  private snack = inject(MatSnackBar);

  onError(err: HttpErrorResponse): void {
    console.log('Error occurred', err);
    if (err instanceof HttpErrorResponse) {
      let message = '';
      if (err.statusText === 'Unknown Error') {
        message = 'Please check your internet connection and try again';
      }
      switch (err.status) {
        case 500:
          message = err.error.message || '';
          break;
        case 400:
          message = err.error.message || 'The server failed process your request';
          break;
        case 401:
          message = 'Your session has expired. Please login again';
          break;
        case 403:
          message = 'You are unauthorized to perform this action';
          break;
        case 404:
          message = err.error.message || 'The requested resource was not found';
          break;
        case 409:
          message = err.error.message || 'Conflict detected. Please check your input and try again';
      }
      if (message) {
        this.snack.open(message, 'Dismiss', {
          panelClass: 'snackbar-error-light'
        });
        return
      }
      else if (err.error.message || err.message) {
        message = err.error.message || err.message;
      }
      else if (err.status >= 500) {
        message = 'Fatal error. Please try again';
      }
      else if (!err.error) {
        message = 'Not yet completed';
      }
      else {
        message = err.statusText;
      }
      this.snack.open(message, 'Dismiss', {
        panelClass: 'snackbar-error',
        announcementMessage: 'OO'
      });
    }
  }
}
