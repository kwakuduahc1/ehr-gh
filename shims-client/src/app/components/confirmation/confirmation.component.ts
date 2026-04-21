import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatIcon } from '@angular/material/icon';

@Component({
  selector: 'bs-confirmation',
  templateUrl: './confirmation.component.html',
  styleUrls: ['./confirmation.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule,
    MatIcon,
    MatButtonModule
  ]
})
export class ConfirmationComponent {
  private diag = inject(MatDialogRef<ConfirmationComponent, boolean>);
  data = inject<string>(MAT_DIALOG_DATA)

  close(act: boolean = false) {
    this.diag.close(act);
  }
}
