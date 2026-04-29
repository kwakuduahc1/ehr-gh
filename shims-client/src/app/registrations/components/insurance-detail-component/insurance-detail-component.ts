import { Component, inject, ChangeDetectionStrategy, signal } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { EditPatientSchemeDto, PatientDetailsDto } from '../../../models/registrations/IRegistrations';
import { MatIcon } from "@angular/material/icon";
import { SchemesDTO } from '../../../models/ISchemes';
import { DatePipe } from '@angular/common';
import { form, FormField, maxLength, minLength, required, schema, validate, FormRoot, submit } from '@angular/forms/signals';
import { MatNativeDateModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { ValidatorMessages } from '../../../components/auth-validators';
import { SchemesService } from '../../../providers/schemes-service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-insurance-detail-component',
  templateUrl: './insurance-detail-component.html',
  styleUrl: './insurance-detail-component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    MatDialogModule,
    MatButtonModule,
    MatIcon,
    MatNativeDateModule,
    MatDatepickerModule,
    FormField,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    DatePipe,
    FormRoot
  ]
})
export class InsuranceDetailComponent {
  data = inject<{ patient: PatientDetailsDto, form?: EditPatientSchemeDto }>(MAT_DIALOG_DATA);
  schemes = inject(SchemesService);
  private diag = inject(MatDialogRef<InsuranceDetailComponent>);
  private snack = inject(MatSnackBar);
  val = new ValidatorMessages();
  private fmMdl = signal<EditPatientSchemeDto>({
    schemesID: this.data.form?.schemesID || '',
    cardID: this.data.form?.cardID || '',
    expiryDate: this.data.form?.expiryDate || '',
    patientSchemesID: this.data.form?.patientSchemesID || null
  });

  form = form(this.fmMdl, PatientSchemeSchema, {
    submission: {
      action: async () => this.addScheme()
    }
  });

  close() {
    this.diag.close();
  }

  addScheme() {
    if (this.data.patient.schemes.some(s => s.patientSchemesID === this.fmMdl().patientSchemesID)) {
      this.snack.open('This scheme is already added for the patient');
      return;
    }
    else this.diag.close({
      patientsID: this.data.patient.patientsID,
      ...this.form().value()
    });
  }
}


const PatientSchemeSchema = schema<EditPatientSchemeDto>(path => {
  required(path.schemesID);
  minLength(path.schemesID, 5);
  maxLength(path.schemesID, 50);

  if (path.schemesID) {
    required(path.cardID);
    minLength(path.cardID, 5);
    maxLength(path.cardID, 30);

    required(path.expiryDate);
  }
});
