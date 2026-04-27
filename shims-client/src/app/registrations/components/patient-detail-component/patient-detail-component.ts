import { Component, inject, ChangeDetectionStrategy } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { PatientDetailsDto } from '../../../models/registrations/IRegistrations';

@Component({
  selector: 'app-patient-detail-component',
  templateUrl: './patient-detail-component.html',
  styleUrl: './patient-detail-component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [MatDialogModule, MatButtonModule]
})
export class PatientDetailComponent {
  data = inject<PatientDetailsDto>(MAT_DIALOG_DATA);
  private diag = inject(MatDialogRef<PatientDetailComponent>);

  close() {
    this.diag.close();
  }
}
