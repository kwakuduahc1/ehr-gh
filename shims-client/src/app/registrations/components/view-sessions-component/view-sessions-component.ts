import { Component, ChangeDetectionStrategy, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBar } from '@angular/material/snack-bar';
import { SessionsHttpService } from '../../sessions-http.service';
import { AddPatientSession, PatientDetailsDto, VwSessions } from '../../../models/registrations/IRegistrations';
import { applyEach, form, FormField, FormRoot, required, submit } from '@angular/forms/signals';
import { Router } from '@angular/router';

@Component({
  selector: 'app-view-sessions-component',
  imports: [
    DatePipe,
    MatButtonModule,
    MatIconModule,
    MatDialogModule,
    MatFormFieldModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatInputModule,
    FormField,
    FormRoot,
  ],
  templateUrl: './view-sessions-component.html',
  styleUrl: './view-sessions-component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ViewSessionsComponent {
  private http = inject(SessionsHttpService);
  data = inject<{ patient: PatientDetailsDto }>(MAT_DIALOG_DATA);
  sessions = signal<VwSessions[]>([]);
  private snackBar = inject(MatSnackBar);
  private router = inject(Router);
  private diag = inject(MatDialogRef<ViewSessionsComponent>);

  fmMdl = signal<AddPatientSession>({
    patientsID: this.data.patient.patientsID,
    visitType: 'Acute',
  });

  form = form(this.fmMdl, path => {
    applyEach(path, required);
  }, {
    submission: {
      action: async (f) => this.addSession()
    }
  });

  constructor() {
    this.http.patientSessions(this.data.patient.patientsID)
      .subscribe((data) => {
        this.sessions.set(data);
      });
  }


  addSession(): void {
    submit(this.form, async () => {
      this.http.addSession(this.fmMdl()).subscribe({
        next: (id) => {

          this.sessions.update(list => [{
            patientAttendancesID: id,
            visitType: this.fmMdl().visitType,
            dateSeen: new Date().toISOString(),
            isActive: true,
          }, ...list.map(s => ({ ...s, isActive: false }))]);
          this.form().reset();
          this.snackBar.open('Session started.', 'Dismiss');
        }
      });
    });
  }

  endSession(session: VwSessions): void {
    this.http.endSession(session.patientAttendancesID).subscribe(() => {
      this.sessions.update(list =>
        list.map(s => s.patientAttendancesID === session.patientAttendancesID ? { ...s, isActive: false } : s)
      );
    });
  }

  vitals(): void {

    this.router.navigate(['vitals', this.data.patient.patientAttendancesID])
      .then(() => this.diag.close())
      .catch(() => this.snackBar.open('Failed to open vitals.', 'Dismiss'));
  }
}
