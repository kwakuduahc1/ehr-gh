import { Component, input, output } from '@angular/core';
import { MatIconButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { MatDialogClose } from '@angular/material/dialog';

@Component({
  selector: 'app-close-button-component',
  imports: [
    MatIconButton,
    MatIcon,
    MatDialogClose,
  ],
  templateUrl: './close-button-component.html',
  styleUrl: './close-button-component.scss'
})
export class CloseButtonComponent {
  prop = input<CloseButtonComponentInputs>({
    ariaLabel: 'Close the dialog',
    class: '',
    title: 'Close the dialog'
  });

  onClose = output<void>({
    alias: 'close'
  });

  close() {
    this.onClose.emit();
  }
}

interface CloseButtonComponentInputs {
  ariaLabel: string;
  class: string;
  title: string;
}
