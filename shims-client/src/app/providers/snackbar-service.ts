import { inject, Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({ providedIn: 'root' })
export class SnackBarService {
    private snackBar = inject(MatSnackBar);

    open(message: string, action: string = "Okay", duration: number = 3000, css: string | string[] = '', afterDismissed?: () => void) {
        this.snackBar.open(message, action, {
            duration,
            panelClass: css
        }).afterDismissed().subscribe(afterDismissed);
    }
}
