import { Component, inject, ChangeDetectionStrategy, signal } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { EditPatientSchemeDto, InsuranceDetails, PatientDetailsDto } from '../../../models/registrations/IRegistrations';
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
import { PatientSchemesHttpService } from '../../patient-schemes-http.service';
import { filter, switchMap } from 'rxjs';
import { ConfirmationComponent } from '../../../components/confirmation/confirmation.component';

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
  data = inject<{ patient: PatientDetailsDto }>(MAT_DIALOG_DATA);
  patientSchemes = signal(this.data.patient.schemes);
  private diag = inject(MatDialog);
  isEdit = signal(false);
  schemes = inject(SchemesService);
  private diagRef = inject(MatDialogRef<InsuranceDetailComponent>);
  private snack = inject(MatSnackBar);
  private http = inject(PatientSchemesHttpService);
  val = new ValidatorMessages();
  private fmMdl = signal<EditPatientSchemeDto>({
    schemesID: '',
    cardID: '',
    expiryDate: '',
    patientSchemesID: null
  });

  form = form(this.fmMdl, PatientSchemeSchema, {
    submission: {
      action: async () => this.addScheme()
    }
  });

  close() {
    this.diagRef.close(this.patientSchemes());
  }

  addScheme() {
    if (this.isEdit()) {
      this.http.edit({ ...this.fmMdl(), patientsID: this.data.patient.patientsID }).subscribe({
        next: () => {
          this.patientSchemes.update(s => s.map(x => x.patientSchemesID === this.fmMdl().patientSchemesID
            ? {
              patientSchemesID: this.fmMdl().patientSchemesID!,
              coverage: this.schemes.schemes().filter(x => x.schemesID === this.fmMdl().schemesID)[0].coverage,
              schemeName: this.schemes.schemes().filter(x => x.schemesID === this.fmMdl().schemesID)[0].schemeName,
              schemesID: this.fmMdl().schemesID,
              cardID: this.fmMdl().cardID,
              expiryDate: this.fmMdl().expiryDate.toString()
            }
            : x));
          this.snack.open('Scheme updated');
          this.form().reset();
          this.fmMdl.set({
            cardID: '',
            expiryDate: '',
            patientSchemesID: '',
            schemesID: '',
            patientsID: ''
          })
          this.isEdit.update(() => false);
        }
      });
    }
    else if (this.patientSchemes().some(s => s.schemesID === this.fmMdl().schemesID)) {
      this.snack.open('This scheme is already added for the patient');
    }
    else {
      this.http.add({ ...this.form().value(), patientsID: this.data.patient.patientsID })
        .subscribe({
          next: id => {
            this.patientSchemes.update(s => [{
              patientSchemesID: id,
              coverage: this.schemes.schemes().filter(x => x.schemesID === this.fmMdl().schemesID)[0].coverage,
              schemeName: this.schemes.schemes().filter(x => x.schemesID === this.fmMdl().schemesID)[0].schemeName,
              schemesID: this.fmMdl().schemesID,
              cardID: this.fmMdl().cardID,
              expiryDate: this.fmMdl().expiryDate.toString()
            }, ...s]);
            this.snack.open('Scheme added');
            this.form().reset();
            this.fmMdl.set({
              cardID: '',
              expiryDate: '',
              patientSchemesID: '',
              schemesID: '',
              patientsID: ''
            })
          }
        });
    }
  }

  editScheme(scheme: InsuranceDetails) {
    if (scheme.schemeName === this.schemes.feePaying().schemeName) {
      this.snack.open('Fee Paying scheme cannot be edited');
    }
    else {
      this.isEdit.update(() => true);
      this.fmMdl.set({
        schemesID: scheme.schemesID,
        cardID: scheme.cardID!,
        expiryDate: scheme.expiryDate!,
        patientSchemesID: scheme.patientSchemesID
      });
    };
  }

  removeScheme(scheme: InsuranceDetails) {
    if (scheme.schemeName === this.schemes.feePaying().schemeName) {
      this.snack.open('Fee Paying scheme cannot be deleted');
    }
    else {
      this.diag.open<ConfirmationComponent, {}, boolean>(ConfirmationComponent, {
        data: 'Are you sure you want to delete this scheme?'
      })
        .afterClosed()
        .pipe(
          filter(x => !!x),
          switchMap(() => this.http.delete(scheme.patientSchemesID))
        )
        .subscribe({
          next: () => {
            this.patientSchemes.update(s => s.filter(x => x.patientSchemesID !== scheme.patientSchemesID));
            this.snack.open('Scheme deleted');
          }
        });
    }
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
